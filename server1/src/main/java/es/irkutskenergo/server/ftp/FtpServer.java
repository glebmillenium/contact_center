package es.irkutskenergo.server.ftp;

import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.InetSocketAddress;
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
    private InetAddress host;
    private InetSocketAddress address;
    private int port;
    private boolean canWork = false;
    private static Map<String, byte[]> storage = new HashMap<String, byte[]>();
    private static ObjectMapper mapper = new ObjectMapper();

    public FtpServer(int port)
    {
        try
        {
            this.port = port;
            this.server = new ServerSocket(this.port);
//            this.host = InetAddress.getLocalHost();
//            this.address = new InetSocketAddress(this.host, this.port);
//            server.bind(address);
//            System.out.println("Ftp server: " + this.host.toString());
            this.canWork = true;
        } catch (IOException ex)
        {
            System.out.println("Error create socket port: " + this.port
                    + " message: " + ex.getMessage());
        }
    }

    @Override
    public void finalize()
    {
        this.canWork = false;
        System.out.println("Ftp server: " + this.port + " is destroyed");
    }

    @Override
    public void run()
    {
        if (this.canWork)
        {
            try
            {
                // Starting the listener
                System.out.println("Starting FTP Server, port: " + this.port);
                if (server.isBound())
                {
                    System.out.println("Wait connection...");

                    /* Continue receiving clients while canWork stays TRUE */
                    byte[] b = new byte[1024];
                    while (this.canWork)
                    {
                        Socket socket = server.accept();
                        InputStream inputStream = socket.getInputStream();
                        inputStream.read(b);
                        String query = new String(b, "UTF-8");
                        String key = getObjectFromJson(query).param2;
                        if (storage.containsKey(key))
                        {
                            System.out.println(
                                    "Отправка сообщения клиенту, по запросу " + query);
                            byte[] message = storage.get(key);
                            TCPSession tcpSessionLocal = new TCPSession(socket, message);
                            tcpSessionLocal.start();
                        } else
                        {
                            System.out.println("Ошибочный запрос");
                        }
                    }
                }
            } catch (IOException ex)
            {
                System.out.println("tcp receiver main try-catch error "
                        + ex.getMessage());
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
            storage.put(key, message);
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
