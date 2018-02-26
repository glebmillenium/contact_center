/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.main;

import es.irkutskenergo.other.Logging;
import es.irkutskenergo.server.netty.fast.*;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
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
    public static void main(String[] args)
    {
        try
        {
            Logging.clear();
            Logging.writeToConsole = true;
            int portFast = 6500;
            
            String version = new String(Files.readAllBytes(Paths.get("version")));
            Logging.log("Версия сервера: " + version);
            Logging.log(" Сервер ЕЭИСЦ успешно запущен:"
                    + "\r\nПорт обмена сообщений - " + portFast + "\r\n", 0);
            FastServer NT = new FastServer(portFast);
            NT.run();
        } catch (Exception ex)
        {
            System.out.println("Сервер не удалось запустить " + ex.getMessage());
        }
    }
}
