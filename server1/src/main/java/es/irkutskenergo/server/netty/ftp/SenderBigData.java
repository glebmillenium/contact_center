/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.server.netty.ftp;

import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.codehaus.jackson.map.ObjectMapper;
import org.jboss.netty.channel.Channel;

/**
 *
 * @author Глеб
 */
public class SenderBigData extends Thread {

    private Channel channel;
    private String responseToClient;
    private boolean successfull;
    private static ObjectMapper mapper = new ObjectMapper();

    public SenderBigData(Channel channel, String responseToClient)
    {
        super();
        setName("Ftp Server");
        this.channel = channel;
        this.responseToClient = responseToClient;
        this.successfull = true;
    }

    public SenderBigData(Channel channel, String responseToClient, boolean successfull)
    {
        super();
        setName("Ftp Server");
        this.channel = channel;
        this.successfull = successfull;
        this.responseToClient = responseToClient;
    }

    @Override
    public void run()
    {
        if (this.successfull)
        {
            String response = this.responseToClient;
                    //.replaceAll("[\\\\]+\\\\0", "\\0") + "\0";
            System.out.println("Реальный размер данных:"
                    + response.getBytes().length);
            sendToClient(response);
        } else
        {
            sendToClient("ERROR_404");
        }
    }

    private void sendToClient(String response)
    {
        this.channel.write(response + "\0");
    }

}
