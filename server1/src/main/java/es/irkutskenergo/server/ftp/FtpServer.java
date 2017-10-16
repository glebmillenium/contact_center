package es.irkutskenergo.server.ftp;

import es.irkutskenergo.other.Logging;
import es.irkutskenergo.other.Triple;
import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import org.codehaus.jackson.map.ObjectMapper;

/**
 *
 * @author admin
 */
public class FtpServer extends Thread {

    private ServerSocket server;
//    private InetAddress host;
//    private InetSocketAddress address;
    private int port;
    private boolean canWork = false;

    /**
     * storage - временное хранилище для, поступающих заявок из вне.
     *
     * @param_1 String - Номер заявки
     * @param_2 Tuple<String, byte[]> - содержимое заявки, заполняется внешним
     * модулем (обычно это осуществляет fast server)
     * @param_2.1 String - Тип заявки 0, которую должен выполнить сервер 0 -
     * отправить данные, 1 - принять данные
     * @param_2.2 byte[] - массив байтов, которые должен отправить клиент
     * серверу
     */
    private static ObjectMapper mapper = new ObjectMapper();
    private static Map<String, Triple<Date, String, byte[]>> storage = 
            new HashMap<String, Triple<Date, String, byte[]>>();
    
    public FtpServer(int port) throws IOException
    {

        this.port = port;
        this.server = new ServerSocket(this.port);
//            this.host = InetAddress.getLocalHost();
//            this.address = new InetSocketAddress(this.host, this.port);
//            server.bind(address);
        this.canWork = true;

    }

    @Override
    public void finalize()
    {
        this.canWork = false;
        Logging.log("Сервер открытый по порту: " + this.port + " был закрыт", 2);
    }

    @Override
    public void run()
    {
        if (this.canWork)
        {
            try
            {
                Logging.log("Сервер запущен по порту: " + this.port, 2);
                if (server.isBound())
                {
                    /* Continue receiving clients while canWork stays TRUE */
                    byte[] b = new byte[1024];
                    while (this.canWork)
                    {
                        Socket socket = server.accept();
                        Logging.log("Новое соединение: " + socket.getInetAddress(), 2);
                        InputStream inputStream = socket.getInputStream();
                        inputStream.read(b);
                        String query = new String(b, "UTF-8");
                        String key = getObjectFromJson(query).param2;
                        if (storage.containsKey(key))
                        {
                            byte[] message = storage.get(key).param3;
                            String typeQuery = storage.get(key).param2;
                            if (typeQuery.equals("1"))
                            {
                                
                            } else if (typeQuery.equals("0"))
                            {
                                Logging.log("Запрос на отправку данных клиенту: "
                                        + socket.getInetAddress() + " По запросу №"
                                        + key + " Объем файла: "
                                        + message.length + "байт", 2);
                                TCPSession tcpSessionLocal = new TCPSession(socket, message);
                                tcpSessionLocal.start();
                            }
                        } else
                        {
                            OutputStream outputStream;
                            outputStream = socket.getOutputStream();
                            byte[] failPackage = new byte[]
                            {
                            };
                            outputStream.write(failPackage, 0, failPackage.length);
                            Logging.log("Такого запроса не существует. "
                                    + "Возможно, он был удален по причине "
                                    + "длительного застоя в системе" + 
                                        "Клиент: "
                                            + socket.getInetAddress(), 2);
                        }
                    }
                }
            } catch (IOException ex)
            {
                Logging.log("Сервер при выполнении запроса "
                        + "получил неизвестную ошибку: " + ex.getMessage(), 2);
            }
        }
    }

    public static boolean addHashKeyIdentificator(String key, byte[] message,
            String typeQuery)
    {
        if (storage.containsKey(key))
        {
            return false;
        } else
        {
            storage.put(key, new Triple<Date, String, byte[]>
                                (new Date(), typeQuery, message));
            return true;
        }
    }

    private ObjectForSerialization getObjectFromJson(String jsonInString)
            throws IOException
    {
        return mapper.readValue(jsonInString,
                ObjectForSerialization.class);
    }
    
    private void clear(int maxUptime)
    {
        
            long currentDate = new Date().getTime();
            Logging.log("Начало чистки \"старых заявок\" с FTP сервера", 2);
            storage.forEach((key, value) -> {
                if(value.param1.getTime() > maxUptime + currentDate)
                {
                    storage.remove(key);
                    Logging.log("Чистка в FTP сервере удалена заявка: " 
                        + key + " Содержимое: " 
                        + value.toString(), 2);
                }
            });
            
            // Альтернативный метод, используется когда поступающих заявок, 
            // которые находятся в спящем режиме слишком много
            /*
            for(Map.Entry<String, Triple<Date, String, byte[]>> pair : storage.entrySet())
            {
                if(pair.getValue().param1.getTime() > maxUptime + currentDate)
                {
                    storage.remove(pair.getKey());

                }
            }
            */
            Logging.log("Чистка \"старых заявок\" с FTP сервера завершена", 2);
    }
}
