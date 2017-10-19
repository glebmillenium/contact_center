package es.irkutskenergo.server.ftp;

import es.irkutskenergo.other.ExceptionServer;
import es.irkutskenergo.other.Logging;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.Arrays;

public class SenderData extends Thread {

    private Socket socket = null;
    private int error_num = 0;
    byte[] message;
    private int tcpConnectionTimeout = 300000;
    static int query = 0;
    private int numberConnect;

    /**
     * A session to read from input stream and pars binary data.
     *
     * @param socket the socket Handle of incoming connection
     */
    public SenderData(Socket socket, byte[] message)
    {
        this.socket = socket;
        this.message = message;
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
                    Logging.log("Началась отправка данных клиенту: "
                            + socket.getInetAddress() + " Номер сеанса: " + numberConnect, 2);
                    OutputStream outputStream;
                    outputStream = socket.getOutputStream();
                    try
                    {
                        if (this.message.length < 50 * 1024)
                        {
                            outputStream.write(this.message, 0, this.message.length);
                            outputStream.flush();
                        } else
                        {
                            InputStream inputStream = socket.getInputStream();
                            int sendCapacity = 0;
                            int fixedSize = 50 * 1024;
                            byte[] shortAnswer = new byte[1024];
                            byte[] packageMessage;
                            while (sendCapacity < this.message.length)
                            {
                                packageMessage = Arrays.copyOfRange(this.message,
                                        sendCapacity, sendCapacity + fixedSize);
                                outputStream.write(packageMessage, 0, fixedSize);
                                int result = inputStream.read(shortAnswer);
                                int downLoadResult = shortAnswer[0];
                                if (result == -1)
                                {
                                    throw new ExceptionServer(
                                            "Ошибка передачи данных. "
                                            + "Связь с клиентом "
                                            + socket.getInetAddress()
                                            + " была потеряна. Номер сеанса: "
                                            + numberConnect);
                                }
                                if (downLoadResult != 49)
                                {
                                    throw new ExceptionServer(
                                            "Ошибка передачи данных. "
                                            + "Клиенту "
                                            + socket.getInetAddress()
                                            + " не удалось получить весь "
                                            + "необходимый объём. "
                                            + "Номер сеанса: "
                                            + numberConnect);
                                }
                                sendCapacity += fixedSize;
                            }
                            sendCapacity -= fixedSize;
                            packageMessage = Arrays.copyOfRange(this.message,
                                    sendCapacity, this.message.length);
                            outputStream.write(packageMessage, 0,
                                    packageMessage.length);
                        }
                        Logging.log("Отправка данных клиенту: "
                                + socket.getInetAddress()
                                + " успешно завершена!", 2);
                    } catch (ExceptionServer e)
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
