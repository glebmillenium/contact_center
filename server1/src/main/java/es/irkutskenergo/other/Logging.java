package es.irkutskenergo.other;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.util.Date;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Класс предназначен для вызова журналирования действий в системе
 *
 * @author Ан Г.В.
 */
public class Logging {

    /**
     * log Метод для журналирования действий в системе
     *
     * @param message Сообщение которое необходимо вывести
     * @param typeMessage режим журналирования: 0 - запись server.log, ftp.log,
     * netty.log 1 - запись server.log, netty.log 2 - запись server.log, ftp.log
     *
     * @return boolean результат выполненных действий (успех/не успех)
     * @author Ан Г.В.
     */
    public static boolean log(String message, int typeMessage)
    {
        FileWriter writer;
        Date date = new Date();
        try
        {
            switch (typeMessage)
            {
                case 0:
                    writer = new FileWriter("logs/server.log", true);
                    writer.append(date.toString() + message + "\n");
                    writer.flush();
                    writer.close();

                    writer = new FileWriter("logs/netty.log", true);
                    writer.append(date.toString() + message + "\n");
                    writer.flush();
                    writer.close();

                    writer = new FileWriter("logs/ftp.log", true);
                    writer.append(date.toString() + message + "\n");
                    writer.flush();
                    writer.close();

                    return true;
                case 1:
                    writer = new FileWriter("logs/server.log", true);
                    writer.append(date.toString() + ". FAST SERVER. " + message + "\n");
                    writer.flush();
                    writer.close();

                    writer = new FileWriter("logs/netty.log", true);
                    writer.append(date.toString() + ". FAST SERVER. " + message + "\n");
                    writer.flush();
                    writer.close();

                    return true;
                case 2:
                    writer = new FileWriter("logs/server.log", true);
                    writer.append(date.toString() + ". FTP SERVER. " + message + "\n");
                    writer.flush();
                    writer.close();

                    writer = new FileWriter("logs/ftp.log", true);
                    writer.append(date.toString() + ". FTP SERVER. " + message + "\n");
                    writer.flush();
                    writer.close();

                    return true;
                default:
                    System.out.println("Неверный параметр журналирования: "
                            + typeMessage + "\nСообщение: " + message);
                    return false;
            }
        } catch (Exception e)
        {
            System.out.println("Модуль журналирования не работает!");
            return false;
        }
    }

    /**
     * log Метод для журналирования действий в системе
     *
     * @param message Сообщение которое необходимо вывести в консоли
     * @return boolean результат выполненных действий (успех/не успех)
     * @author Ан Г.В.
     */
//    public static boolean log(String message)
//    {
//        try
//        {
//            System.out.println(message);
//        } catch (Exception e)
//        {
//            System.out.println(e.getMessage());
//            return false;
//        }
//        return true;
//    }
    public static void clear()
    {
        FileWriter writer = null;
        try
        {
            File dir = new File(".//logs");
            dir.mkdir();

            File file = new File(".//logs//server.log");
            if (file.exists())
            {
                file.delete();
            }
            file.createNewFile();

            file = new File(".//logs//netty.log");
            if (file.exists())
            {
                file.delete();
            }
            file.createNewFile();

            file = new File(".//logs//ftp.log");
            if (file.exists())
            {
                file.delete();
            }
            file.createNewFile();
            int i = 0;

        } catch (IOException ex)
        {
            System.out.println("Не удалось произвести очистку журналов");
        }
    }
}
