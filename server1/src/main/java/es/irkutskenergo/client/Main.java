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

            Logging.log("Сервер ЕЭИСЦ успешно запущен:"
                    + "\r\nПорт обмена сообщений - " + portFast
                    + "\r\nПорт файлового обмена - " + portFtp + "\r\n", 0);
        } catch (Exception ex)
        {
            System.out.println("Сервер не удалось запустить");
        }
    }
}