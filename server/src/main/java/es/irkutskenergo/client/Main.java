/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.client;
import es.irkutskenergo.server.netty.NettyServer;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author admin
 */
public class Main {
        /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        try {
            NettyServer NT = new NettyServer(6500);
        } catch (Exception ex) {
            Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
