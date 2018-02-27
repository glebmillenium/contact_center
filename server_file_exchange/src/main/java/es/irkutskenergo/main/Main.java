/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.main;

import es.irkutskenergo.other.Logging;
import es.irkutskenergo.server.ftp.FtpServer;
import java.nio.file.Files;
import java.nio.file.Paths;

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
            int portFtp = 6502;
            FtpServer fs = new FtpServer(portFtp);
            String version = new String(Files.readAllBytes(Paths.get("version")));
            Logging.log("Версия сервера: " + version);
            Logging.log(" Сервер ЕЭИСЦ успешно запущен:"
                    + "\r\nПорт файлового обмена - " + portFtp + "\r\n", 0);
            fs.start();
        } catch (Exception ex)
        {
            System.out.println("Сервер не удалось запустить " + ex.getMessage());
        }
    }
}
