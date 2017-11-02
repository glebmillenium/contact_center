package es.irkutskenergo.server.ftp;

import es.irkutskenergo.other.ExceptionServer;
import es.irkutskenergo.other.Logging;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.Arrays;

public class SenderData extends Thread {

    private Socket socket = null;
    private int error_num = 0;
    String pathToFile;
    int expectedSize;
    private int tcpConnectionTimeout = 300000;
    static int query = 0;
    private int numberConnect;

    /**
     * A session to read from input stream and pars binary data.
     *
     * @param socket the socket Handle of incoming connection
     */
    public SenderData(Socket socket, String pathToFile, int expectedSize)
    {
        this.socket = socket;
        this.pathToFile = pathToFile;
        this.expectedSize = expectedSize;
        query++;
        if (query >= 65536)
        {
            query = 1;
        }
        numberConnect = query;
    }

    public void run()
    {
        try
        {
            if (socket != null)
            {
                if (!socket.isClosed())
                {
                    int fixedSize = 50 * 1024;
                    Logging.log("Начался прием данных от клиента: "
                            + socket.getInetAddress() + " Номер сеанса: " + numberConnect, 2);
                    OutputStream outputStream;
                    outputStream = socket.getOutputStream();
                    InputStream inputStream;
                    inputStream = socket.getInputStream();
                    
                    byte[] mess = new byte[]{49};
                    
                    byte[] shortAnswer = new byte[fixedSize];
                    FileOutputStream fos = new FileOutputStream(this.pathToFile);
                    outputStream.write(mess, 0, mess.length);
                    try
                    {
                        if (expectedSize < fixedSize)
                        {
                            Logging.log("Цельный прием данных от клиента: "
                            + socket.getInetAddress() + " Номер сеанса: " + numberConnect, 2);
                            shortAnswer = new byte[expectedSize];
                            inputStream.read(shortAnswer);
                            fos.write(shortAnswer);
                            fos.close();
                        } else
                        {
                            int sendCapacity = fixedSize;
                            Logging.log("Кусочный прием данных от клиента: "
                            + socket.getInetAddress() + " Номер сеанса: " + numberConnect, 2);
                            while (sendCapacity < expectedSize)
                            {
                                
                                int result = inputStream.read(shortAnswer);
                                fos.write(shortAnswer);
                                sendCapacity += fixedSize;
                                outputStream.write(mess, 0, mess.length);
                                outputStream.flush();
                                Logging.log("Получено " + (sendCapacity - fixedSize)
                                        + " байт: " + socket.getInetAddress() + 
                                        " Номер сеанса: " + numberConnect, 2);
                            }
                            sendCapacity = expectedSize - (sendCapacity - fixedSize);
                            shortAnswer = new byte[sendCapacity];
                            inputStream.read(shortAnswer);
                            fos.write(shortAnswer);
                            outputStream.write(mess, 0, mess.length);
                            
                            fos.close();
                            Logging.log("Получена последняя пачка данных от клиента: "
                            + socket.getInetAddress() + " Номер сеанса: " + numberConnect, 2);
                        }
                        Logging.log("Получение данных от клиента: "
                                + socket.getInetAddress()
                                + " успешно завершено!", 2);
                    } catch (IOException e)
                    {
                        byte[] failPackage = new byte[]
                        {
                        };
                        outputStream.write(failPackage, 0, failPackage.length);
                        Logging.log(e.toString(), 2);
                    }
                }
            }

            if (this.socket != null && !this.socket.isClosed())
            {
                try
                {
                    this.socket.close();
                } catch (IOException ex)
                {
                    Logging.log("Сокет для файлового обмена закрыт: код ошибки "
                            + error_num + " Номер сеанса: " + numberConnect, 2);
                }
            }
        } catch (IOException ioe)
        {
            Logging.log("Сессия передачи файлов была завершена с клиентом "
                    + socket.getInetAddress() + " Номер сеанса: "
                    + numberConnect, 2);
        }
    }
}
