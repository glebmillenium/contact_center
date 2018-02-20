package es.irkutskenergo.file_sharing_service;

import es.irkutskenergo.logging.Logging;
import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;

/**
 * Сервер, предназначенный для осуществления отправки большого объема информации
 *
 * @author admin
 */
public class FtpServer extends Thread {

    private ServerSocket serverSocket;
    private int port;

    public FtpServer(int port) throws IOException
    {
        this.port = port;
        this.serverSocket = new ServerSocket(this.port);
    }
    
    public FtpServer() throws IOException
    {
        this.port = 6503;
        this.serverSocket = new ServerSocket(this.port);
    }

    @Override
    public void finalize() throws Throwable
    {
        try
        {
            Logging.log("Сервер открытый по порту: " + this.port + " был закрыт", 2);
        } finally
        {
            super.finalize();
        }
    }

    
    @Override
    public void run()
    {
        try
        {
            Logging.log("Сервер запущен по порту: " + this.port, 2);
            if (serverSocket.isBound())
            {
                while (this.isAlive())
                {
                    Socket socket = serverSocket.accept();
                    PrimaryQueryTreatmenter answer = new PrimaryQueryTreatmenter(socket);
                    Logging.log("Thread: Main FTP", 4);
                    answer.start();
                }
            }
        } catch (IOException ex)
        {
            Logging.log("Работа сервера приостановлена по неизвестной "
                    + "причине" + ex.getMessage(), 2);
        }
    }
}
