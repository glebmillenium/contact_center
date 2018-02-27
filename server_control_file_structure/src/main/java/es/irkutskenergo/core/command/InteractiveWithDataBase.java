/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.core.command;

import es.irkutskenergo.other.Tuple;
import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Timestamp;
import java.util.Calendar;

/**
 *
 * @author admin
 */
public class InteractiveWithDataBase {

    // JDBC variables for opening and managing connection
    private static Connection con;
    private static Statement stmt;

    /**
     * Подключение к СУБД
     *
     * @param dbName
     * @param user
     * @param password
     * @param port
     */
    public InteractiveWithDataBase(String dbName, String user, String password, int port)
    {
        try
        {
            // opening database connection to MySQL server
            this.con = DriverManager.getConnection("jdbc:mysql://localhost:" + port + "/"
                    + dbName, user, password);
            stmt = con.createStatement();
        } catch (SQLException sqlEx)
        {
            sqlEx.printStackTrace();
        } finally
        {
            try
            {
                con.close();
            } catch (SQLException se)
            {

            }
            try
            {
                stmt.close();
            } catch (SQLException se)
            {

            }
        }
    }

    /**
     * Метод получения все абсолютных путей к файловым системам с БД
     *
     * @return
     */
    public List<Tuple<Integer, String>> getAllPathsFileSystemsForUpdate()
    {
        ResultSet rs;
        List<Tuple<Integer, String>> result = new ArrayList<Tuple<Integer, String>>();
        try
        {
            String query = "SELECT fs.id, fs.relative_way FROM file_systems AS fs";
            rs = this.stmt.executeQuery(query);

            while (rs.next())
            {
                result.add(new Tuple<>(rs.getInt(1), rs.getString(2)));
            }
            try
            {
                rs.close();
            } catch (SQLException se)
            {

            }
        } catch (SQLException sqlEx)
        {
            sqlEx.printStackTrace();
            result.clear();
        }
        return result;
    }

    public boolean updateFileSystemStructureCatalog(Integer idFileSystem,
            String structureJSON)
    {
        boolean resultExecuteOperation = false;
        try
        {
            String query = "INSERT INTO file_system_change (id_file_system, "
                    + "last_update, storage_data) \n"
                    + " VALUES (" + idFileSystem + ", "
                    + new Timestamp(Calendar.getInstance().getTimeInMillis()) 
                    + "'" + structureJSON + "');";

            // executing SELECT query
            stmt.executeUpdate(query);
            resultExecuteOperation = true;
        } catch (SQLException ex)
        {
            System.out.println("Не удалось произвести запись в "
                    + "file_system_change");
            resultExecuteOperation = false;
        }
        return resultExecuteOperation;
    }

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

    public static String[] getDifferenceVersion(String version)
    {
        BufferedReader reader = null;
        try
        {
            List<String> result = new ArrayList<String>();
            reader = new BufferedReader(new FileReader("history_update"));
            String line;
            boolean find = false;
            while ((line = reader.readLine()) != null)
            {
                if (line.equals(version))
                {
                    find = true;
                    continue;
                }
                if (find)
                {
                    result.add(line);
                }
            }
            String[] result2 = new String[result.size()];
            for (int i = 0; i < result.size(); i++)
            {
                result2[i] = result.get(i);
            }
            return result2;
        } catch (IOException ex)
        {
            return new String[]
            {
            };
        }
    }

//    public static Map<String, Triple<String, String, String>> getRootAliance()
//    {
//        Map<String, Triple<String, String, String>> result
//                = new HashMap<String, Triple<String, String, String>>();
//        BufferedReader reader = null;
//        try
//        {
//            reader = new BufferedReader(new FileReader("list_connect"));
//            String line;
//            while ((line = reader.readLine()) != null)
//            {
//                System.out.println(line);
//                String[] temp = line.split(";");
//                result.put(temp[0], new Triple<String, String, String>(temp[1], temp[2], temp[3]));
//            }
//        } catch (FileNotFoundException ex)
//        {
//            result.clear();
//            result.put("0", new Triple<String, String, String>("Системный диск",
//                    "C:\\", "1"));
//        } finally
//        {
//            return result;
//        }
//    }
}
