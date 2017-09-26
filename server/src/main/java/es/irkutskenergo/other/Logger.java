/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.other;

import java.io.FileWriter;

/**
 * Класс предназначен для вызова журналирования действий в системе
 * 
 * @author Ан Г.В.
 */
public class Logger {

    /**
     * log Метод для журналирования действий в системе
     *
     * @param message Сообщение которое необходимо вывести
     * @param typeMessage режим журналироваиня:
     *      0 - не выводит текст вообще (отсутствие действия)
     *      1 - выводит сообщение на уровне консоли 
     *      2 - запись в отдельный файл event.log
     * @return boolean результат выполненных действий (успех/не успех)
     * @author Ан Г.В.
     */
    public static boolean log(String message, int typeMessage)
    {
        try
        {
            switch (typeMessage)
            {
                case 0:
                    //do nothing
                    return true;
                case 1:
                    System.out.println(message);
                    return true;
                case 2:
                    FileWriter writer = new FileWriter("event.log", true);
                    writer.write(message + "\n");
                    writer.flush();
                    return true;
                default:
                    return false;
            }
        } catch (Exception e)
        {
            System.out.println(e.getMessage());
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
}
