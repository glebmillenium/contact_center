/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.core.command;

import es.irkutskenergo.other.Triple;
import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.util.HashMap;
import java.util.Map;

/**
 *
 * @author admin
 */
public class InteractiveWithDataBase {

    public static int getRightAccess(String login, String password)
    {
        int right = -1;

        if (login.equals("contact") && password.equals("center"))
        {
            right = 1;
        } else if (login.equals("supervisor") && password.equals("center"))
        {
            right = 3;
        } else
        {
            right = -1;
        }
        return right;
    }

    public static Map<String, Triple<String, String, String>> getRootAliance()
    {
        Map<String, Triple<String, String, String>> result
                = new HashMap<String, Triple<String, String, String>>();
        BufferedReader reader = null;
        try
        {
            reader = new BufferedReader(new FileReader("list_connect"));
            String line;
            while ((line = reader.readLine()) != null)
            {
                System.out.println(line);
                String[] temp = line.split(";");
                result.put(temp[0], new Triple<String, String, String>(temp[1], temp[2], temp[3]));
            }
        } catch (FileNotFoundException ex)
        {
            result.clear();
            result.put("0", new Triple<String, String, String>("Системный диск",
                    "C:\\", "1"));
        } finally
        {
            return result;
        }
    }
}
