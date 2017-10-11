/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package equalsstr;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author admin
 */
public class EqualsStr {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args)
    {
        try
        {
            FileInputStream fin;
            fin = new FileInputStream(
                    "C:\\Users\\admin\\Desktop\\ИЭСК130716 ЖЗ объединенный.xlsx");
            byte[] buffer_server = new byte[fin.available()];
            fin.read(buffer_server, 0, fin.available());
            fin.close();
            
            FileInputStream fin1;
            fin1 = new FileInputStream(
                    "C:\\contact_center\\client1\\contact_center_application\\"
                            + "bin\\Debug\\tmp\\ИЭСК130716 ЖЗ объединенный.xlsx");
            byte[] buffer_client = new byte[fin1.available()];
            fin1.read(buffer_client, 0, fin1.available());
            fin1.close();
            
            boolean result = true;
            if(buffer_server.length == buffer_client.length)
            {
                int error = 0;
                for(int i = 0; i < buffer_client.length; i++)
                {
                    if(buffer_server[i] != buffer_client[i])
                    {
                        System.out.println(
                                "Символы не равны! байт под номером: " + i);
                        System.out.println(
                                "Байт сервера равен: " + buffer_server[i]);
                        System.out.println(
                                "Байт клиента равен: " + buffer_client[i]);
                        error++;
                    }
                }
                System.out.println("Размер файлов: " + buffer_client.length);
                System.out.println("Количество ошибок: " + error);
                System.out.println("Процент не соответствий: " + 
                        (100.0 * (float) error)/((float) buffer_client.length));
            }
            else
            {
                System.out.println("Размеры исходных файлов не равны!");
                System.out.println("Server.log: " + buffer_server.length);
                System.out.println("Client.log: " + buffer_client.length);
            }
        } catch (FileNotFoundException ex)
        {
            Logger.getLogger(EqualsStr.class.getName()).log(Level.SEVERE, null, ex);
        } catch (IOException ex)
        {
            Logger.getLogger(EqualsStr.class.getName()).log(Level.SEVERE, null, ex);
        }

    }

}
