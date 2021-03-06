package es.irkutskenergo.main;

import es.irkutskenergo.other.Logging;
import es.irkutskenergo.server.fast.FastServer2;
import es.irkutskenergo.server.netty.fast.FastServer;
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
            int portFast = 6500;
            int portFtp = 6502;

            FtpServer fs = new FtpServer(portFtp);

            String version = new String(Files.readAllBytes(Paths.get("version")));
            Logging.log("Версия сервера: " + version);
            Logging.log(" Сервер ЕЭИСЦ успешно запущен:"
                    + "\r\nПорт обмена сообщений - " + portFast
                    + "\r\nПорт файлового обмена - " + portFtp + "\r\n", 0);
            fs.start();
            FastServer NT = new FastServer(portFast);
            NT.run();
            //FastServer2 fs2 = new FastServer2(portFast);
            //fs2.start();
        } catch (Exception ex)
        {
            System.out.println("Сервер не удалось запустить " + ex.getMessage());
        }
    }
}
