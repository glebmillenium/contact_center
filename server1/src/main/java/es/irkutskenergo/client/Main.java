/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.client;

import es.irkutskenergo.server.netty.fast.swap.NettyServer;
import es.irkutskenergo.serialization.ObjectForSerialization;
import es.irkutskenergo.server.netty.ftp.FtpServer;
import java.io.IOException;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.codehaus.jackson.map.ObjectMapper;

/**
 *
 * @author admin
 */
public class Main {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args)
    {
        try
        {
            NettyServer NT = new NettyServer(6500);
            FtpServer ftp = new FtpServer(7000);
            // debug
            //serialize();
        } catch (Exception ex)
        {
            Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
