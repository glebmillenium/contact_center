package es.irkutskenergo.logging;

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

    public static boolean writeToConsole = false;

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
            String output = "";
            switch (typeMessage)
            {
                case 0:
                    writer = new FileWriter("logs/server.log", true);
                    output = date.toString() + message + "\r\n";
                    writer.append(output);
                    writer.flush();
                    writer.close();

                    writer = new FileWriter("logs/netty.log", true);
                    writer.append(output);
                    writer.flush();
                    writer.close();

                    writer = new FileWriter("logs/ftp.log", true);
                    writer.append(output);
                    writer.flush();
                    writer.close();

                    break;
                case 1:
                    output = date.toString() + ". FAST SERVER. " + message + "\r\n";
                    writer = new FileWriter("logs/server.log", true);
                    writer.append(output);
                    writer.flush();
                    writer.close();

                    writer = new FileWriter("logs/netty.log", true);
                    writer.append(output);
                    writer.flush();
                    writer.close();

                    break;
                case 2:
                    output = date.toString() + ". FTP SERVER. " + message + "\r\n";
                    writer = new FileWriter("logs/server.log", true);
                    writer.append(output);
                    writer.flush();
                    writer.close();

                    writer = new FileWriter("logs/ftp.log", true);
                    writer.append(output);
                    writer.flush();
                    writer.close();

                    break;
                case 3:
                    output = date.toString() + ". Update Catalog from FastServer. " + message + "\r\n";
                    writer = new FileWriter("logs/update_catalog.log", true);
                    writer.append(output);
                    writer.flush();
                    writer.close();

                    break;
                case 4:
                    output = date.toString() + ". Thread. " + message + "\r\n";
                    writer = new FileWriter("logs/working_thread.log", true);
                    writer.append(output);
                    writer.flush();
                    writer.close();

                    break;
                default:
                    System.out.println("Неверный параметр журналирования: "
                            + typeMessage + "\nСообщение: " + message);
                    return false;
            }
            if (writeToConsole)
            {
                System.out.println(output);
            }
            return true;
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
    public static boolean log(String message)
    {
        try
        {
            System.out.println(message);
        } catch (Exception e)
        {
            System.out.println(e.getMessage());
            return false;
        }
        return true;
    }

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

            file = new File(".//logs//update_catalog.log");
            if (file.exists())
            {
                file.delete();
            }
            file.createNewFile();            
            
            file = new File(".//logs//working_thread.log");
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

        } catch (IOException ex)
        {
            System.out.println("Не удалось произвести очистку журналов");
        }
    }
}
