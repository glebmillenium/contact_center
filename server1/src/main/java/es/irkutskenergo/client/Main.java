/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.client;

import es.irkutskenergo.other.Logging;
import es.irkutskenergo.server.netty.fast.FastServer;
import es.irkutskenergo.server.ftp.FtpServer;

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
            int portFast = 6500;
            int portFtp = 6502;
            FastServer NT = new FastServer(portFast);
            FtpServer fs = new FtpServer(portFtp);
            fs.start();

            Logging.log("Сервер ЕЭИСЦ успешно запущен:\n"
                    + "\nПорт обмена сообщений - " + portFast
                    + "\nПорт файлового обмена - " + portFtp, 0);
        } catch (Exception ex)
        {
            System.out.println("Сервер не удалось запустить");
        }
    }
}
