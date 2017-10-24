package es.irkutskenergo.server.ftp;

import es.irkutskenergo.other.Logging;
import es.irkutskenergo.other.Quadro;
import es.irkutskenergo.other.Storage;
import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import org.codehaus.jackson.map.ObjectMapper;

/**
 * Сервер, предназначенный для осуществления отправки большого объема информации
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
     * Вспомогательный объект, осуществлящий сериализацию данных
     * 
     * @param ObjectMapper
     */
    private static ObjectMapper mapper = new ObjectMapper();
    
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
                    byte[] b = new byte[1024];
                    while (this.canWork)
                    {
                        Socket socket = server.accept();
                        Logging.log("Новое соединение: " + socket.getInetAddress(), 2);
                        InputStream inputStream = socket.getInputStream();
                        inputStream.read(b);
                        String query = new String(b, "UTF-8");
                        int key = Integer.parseInt(getObjectFromJson(query)
                                                                       .param2);
                        if (Storage.containsKey(key))
                        {
                            Quadro<Boolean, String, byte[], byte[]> obj = Storage.cut(key);
                            byte[] message = obj.param3;
                            if (obj.param1)
                            {
                                Logging.log("Запрос на прием данных от клиента: "
                                    + socket.getInetAddress() + " По запросу №"
                                    + key + " Объем файла: "
                                    + message.length + "байт", 2);

                                SenderData tcpSessionLocal = new SenderData(socket, 
                                        new String(obj.param3, "UTF-8"), 
                                        Integer.parseInt(new String(obj.param4, 
                                                "UTF-8")));
                                tcpSessionLocal.start();
                            } else
                            {
                                Logging.log("Запрос на отправку данных клиенту: "
                                        + socket.getInetAddress() + " По запросу №"
                                        + key + " Объем файла: "
                                        + message.length + "байт", 2);
                                ReceiveData rdSession = new ReceiveData(socket, message);
                                rdSession.start();
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

    /**
     * getObjectFromJson - сериализует объект из строки в ObjectForSerialization
     * 
     * @param jsonInString
     * @return
     * @throws IOException 
     */
    private ObjectForSerialization getObjectFromJson(String jsonInString)
            throws IOException
    {
        return mapper.readValue(jsonInString,
                ObjectForSerialization.class);
    }

    
    /**
     * storage - временное хранилище для, поступающих заявок из вне.
     *
     * @param_1 String - Информация о заявке;
     * @param_1_1 Tuple<Integer,> - Номер заявки по которой сервер выдает данные;
     * @param_1_2 Tuple<,Date> - Время создания заявки, максимальное время жизни
     *      такой заявки определяется в граббере, который запускается 
     *      периодически;
     * @param_2 Tuple<Boolean, byte[], byte[]> - содержимое заявки, заполняется 
     * внешним модулем (обычно это осуществляет fast server);
     * @param_2_1 Tuple<Boolean, , > - Тип заявки, которая определяет поведение 
     *      обмена сервера с клиентом:
     *          false - имеющиеся данные необходимо отправить клиенту
     *                  (пакетная отправка данных по 50 кб);
     *          true - данные необходимо принять на стороне сервера;
     * @param_2_2 Tuple< , byte[], > - Основная информация о заявке: путь к 
     *      файлу если это отправка
     * @param_2_3 Tuple< , , byte[]> - дополнительная информация прикрепляемая 
     *                  к заявке для дальнейшего анализа
     * серверу
     */
    //private static Map<Integer, Quadro<Date, Boolean, byte[], byte[]>> 
    //        storage = new HashMap<Integer, Quadro<Date, Boolean, byte[], 
    //                                                                byte[]>>();
    /**
     * Метод для чистки данных в хранилище
     * (Не поддерживается!)
     * @param maxUptime 
     */
    /*private void clear(int maxUptime)
    {
        long currentDate = new Date().getTime();
        Logging.log("Начало чистки \"старых заявок\" с FTP сервера", 2);
        storage.forEach((key, value) -> {
            if(value.param1.getTime() > maxUptime + currentDate)
            {
                storage.remove(key);
                Logging.log("Чистка в FTP сервере удалена заявка: " 
                    + key.toString() + " Содержимое: " 
                    + value.toString(), 2);
            }
        });

        Logging.log("Чистка \"старых заявок\" с FTP сервера завершена", 2);
    }*/
}
