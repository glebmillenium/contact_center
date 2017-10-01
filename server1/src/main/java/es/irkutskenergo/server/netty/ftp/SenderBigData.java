/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.server.netty.ftp;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
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

    public SenderBigData(Channel channel, String message) {
        super();
        setName("cmdProcessor");
        this.channel = channel;
        this.responseToClient = responseToClient;
        this.successfull = true;
    }

    public SenderBigData(Channel channel, String message, boolean successfull) {
        super();
        setName("cmdProcessor");
        this.channel = channel;
        this.responseToClient = responseToClient;
        this.successfull = successfull;
    }

    @Override
    public void run() {
        if (this.successfull) {
            sendToClient(responseToClient);
        } else {
            sendToClient("ERROR_404");
        }
    }

    private void sendToClient(String response) {
        this.channel.write(response + "\0");
    }
}
