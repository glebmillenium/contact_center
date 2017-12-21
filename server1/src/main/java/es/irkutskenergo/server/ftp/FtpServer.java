package es.irkutskenergo.server.ftp;

import es.irkutskenergo.other.Logging;
import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;

/**
 * Сервер, предназначенный для осуществления отправки большого объема информации
 *
 * @author admin
 */
public class FtpServer extends Thread {

    private ServerSocket server;
    private int port;
    private boolean canWork = false;

    public FtpServer(int port) throws IOException
    {
        this.port = port;
        this.server = new ServerSocket(this.port);
        this.canWork = true;
    }

    @Override
    public void finalize() throws Throwable
    {
        try
        {
            this.canWork = false;
            Logging.log("Сервер открытый по порту: " + this.port + " был закрыт", 2);
        } finally
        {
            super.finalize();
        }
    }

    
    @Override
    public void run()
    {
        if (this.canWork)
        {
            try
            {
                Logging.log("Сервер запущен по порту: " + this.port, 2);
                if (server.isBound())
                {
                    while (this.canWork)
                    {
                        Socket socket = server.accept();
                        PrimaryQueryTreatmenter answer = new PrimaryQueryTreatmenter(socket);
                        Logging.log("Thread: Main FTP", 4);
                        answer.start();
                    }
                }
            } catch (IOException ex)
            {
                Logging.log("Работа сервера приостановлена по неизвестной "
                        + "причине" + ex.getMessage(), 2);
            }
        }
    }



    /**
     * storage - временное хранилище для, поступающих заявок из вне.
     *
     * @param_1 String - Информация о заявке;
     * @param_1_1 Tuple<Integer,> - Номер заявки по которой сервер выдает
     * данные;
     * @param_1_2 Tuple<,Date> - Время создания заявки, максимальное время жизни
     * такой заявки определяется в граббере, который запускается периодически;
     * @param_2 Tuple<Boolean, byte[], byte[]> - содержимое заявки, заполняется
     * внешним модулем (обычно это осуществляет fast server);
     * @param_2_1 Tuple<Boolean, , > - Тип заявки, которая определяет поведение
     * обмена сервера с клиентом: false - имеющиеся данные необходимо отправить
     * клиенту (пакетная отправка данных по 50 кб); true - данные необходимо
     * принять на стороне сервера;
     * @param_2_2 Tuple< , byte[], > - Основная информация о заявке: путь к
     * файлу если это отправка
     * @param_2_3 Tuple< , , byte[]> - дополнительная информация прикрепляемая к
     * заявке для дальнейшего анализа серверу
     */
    //private static Map<Integer, Quadro<Date, Boolean, byte[], byte[]>> 
    //        storage = new HashMap<Integer, Quadro<Date, Boolean, byte[], 
    //                                                                byte[]>>();
    /**
     * Метод для чистки данных в хранилище (Не поддерживается!)
     *
     * @param maxUptime
     */
    /*private void clear(int maxUptime)
    {
        long currentDate = new Date().getTime();
        Logging.log("Начало чистки \"старых заявок\" с FTP сервера", 2);
        storage.forEach((key, value) -> {
            if(value.param1.getTime() > maxUptime + currentDate)
            {
                storage.remove(key);
                Logging.log("Чистка в FTP сервере удалена заявка: " 
                    + key.toString() + " Содержимое: " 
                    + value.toString(), 2);
            }
        });

        Logging.log("Чистка \"старых заявок\" с FTP сервера завершена", 2);
    }*/
}
