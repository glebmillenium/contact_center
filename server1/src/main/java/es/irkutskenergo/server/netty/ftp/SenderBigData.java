/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.server.netty.ftp;

import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
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
    private byte[] responseToClient;
    private boolean successfull;
    private static ObjectMapper mapper = new ObjectMapper();

    public SenderBigData(Channel channel, byte[] responseToClient) {
        super();
        setName("Ftp Server");
        this.channel = channel;
        this.responseToClient = responseToClient;
        this.successfull = true;
    }

    public SenderBigData(Channel channel, byte[] responseToClient, boolean successfull) {
        super();
        setName("Ftp Server");
        this.channel = channel;
        this.successfull = successfull;
        this.responseToClient = responseToClient;
    }

    @Override
    public void run() {
        if (this.successfull) {
            //.replaceAll("[\\\\]+\\\\0", "\\0") + "\0";
            sendToClient(this.responseToClient);
        } else {
            try {
                sendToClient("ERROR_404".getBytes("UTF-8"));
            } catch (UnsupportedEncodingException ex) {
                Logger.getLogger(SenderBigData.class.getName()).log(Level.SEVERE, null, ex);
            }
        }
    }

    private void sendToClient(byte[] response) {
        this.channel.write(response);
    }
}
