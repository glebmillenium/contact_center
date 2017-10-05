package es.irkutskenergo.server.ftp;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.net.SocketException;
import java.util.Arrays;

public class TCPSession extends Thread {

    // Local variables
    private Socket socket = null;
    private int error_num = 0;
    byte[] message;
    private int tcpConnectionTimeout = 300000;

    /**
     * A session to read from input stream and pars binary data.
     *
     * @param socket the socket Handle of incoming connection
     */
    public TCPSession(Socket socket, byte[] message)
    {
        this.socket = socket;
        this.message = message;
    }

    public void run()
    {

        try
        {
            if (socket != null)
            {
                if (!socket.isClosed())
                {
                    // Read data from stream
//                    InputStream inputStream = socket.getInputStream();
                    OutputStream outputStream;
                    outputStream = socket.getOutputStream();
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
                            if(result == -1 || downLoadResult != 49)
                            {
                                throw new IOException();
                            }
                            sendCapacity += fixedSize;
                        }
                        sendCapacity -= fixedSize;
                        packageMessage = Arrays.copyOfRange(this.message, 
                                    sendCapacity, this.message.length);
                        outputStream.write(packageMessage, 0, 
                                packageMessage.length);
                    }
                }
            }
        } catch (IOException ioe)
        {
            System.out.println("Session exception in ERR:" + ioe.getMessage()
                    + "\n" + ioe.getLocalizedMessage());
        }

        // Close socket
        if (this.socket != null && !this.socket.isClosed())
        {
            try
            {
                this.socket.close();
            } catch (IOException ex)
            {
                System.out.println("Socket close in ERR:" + error_num);
            }
        }

    }

}
