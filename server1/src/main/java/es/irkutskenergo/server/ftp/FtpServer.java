package es.irkutskenergo.server.ftp;

import es.irkutskenergo.other.Logging;
import es.irkutskenergo.other.Tuple;
import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import java.io.InputStream;
import java.net.ServerSocket;
import java.net.Socket;
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
    private static Map<String, Tuple<String, byte[]>> storage = new HashMap<String, Tuple<String, byte[]>>();
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
                            byte[] message = storage.get(key).y;
                            String typeQuery = storage.get(key).x;
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
                            Logging.log("Ошибочный запрос. "
                                    + "Запрос на отправку данных клиенту: "
                                    + socket.getInetAddress() + " По запросу №",
                                    2);
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

    public static boolean addHashKeyIdentificator(String key, byte[] message)
    {
        if (storage.containsKey(key))
        {
            return false;
        } else
        {
            storage.put(key, new Tuple<String, byte[]>("", message));
            return true;
        }
    }

    private ObjectForSerialization getObjectFromJson(String jsonInString)
            throws IOException
    {
        return mapper.readValue(jsonInString,
                ObjectForSerialization.class);
    }
}
