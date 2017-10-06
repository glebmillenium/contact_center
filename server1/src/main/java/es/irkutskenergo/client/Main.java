/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.client;

import es.irkutskenergo.server.netty.fast.FastServer;
import es.irkutskenergo.serialization.ObjectForSerialization;
import es.irkutskenergo.server.ftp.FtpServer;
import es.irkutskenergo.server.netty.data.DataServer;
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
            FastServer NT = new FastServer(6500);
            DataServer ds = new DataServer(6501);
            FtpServer fs = new FtpServer(6502);
            fs.start();
        } catch (Exception ex)
        {
            Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
