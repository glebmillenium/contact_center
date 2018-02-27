/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.control_structure;

import es.irkutskenergo.core.command.DataConnection;
import es.irkutskenergo.core.command.InteractiveWithDataBase;
import es.irkutskenergo.core.command.ProcessFileCommands;
import es.irkutskenergo.other.Tuple;
import java.io.IOException;
import java.util.List;

/**
 *
 * @author admin
 */
public class UpdaterStructure extends Thread {

    InteractiveWithDataBase IWDB;

    public UpdaterStructure()
    {
        this.IWDB = new InteractiveWithDataBase(DataConnection.DB_NAME,
                DataConnection.USER_DB, DataConnection.PASSWORD_DB,
                DataConnection.DB_PORT);
        System.out.println("Realized connection with SMDB");
    }

    @Override
    public void run()
    {
        try
        {
            String dataStructure;
            List<Tuple<Integer, String>> allPathsFileSystems;
            System.out.println("Starting to process update catalogs file systems");
            while (true)
            {

                allPathsFileSystems = this.IWDB.getAllPathsFileSystemsForUpdate();
                for (Tuple<Integer, String> path : allPathsFileSystems)
                {
                    try
                    {
                        //max - 10485760 byte = 10 Mb
                        dataStructure
                                = ProcessFileCommands.getAllFolder(path.param2)
                                + "\0";
                    } catch (IOException ex)
                    {
                        System.out.println(ex.fillInStackTrace() + " "
                                + ex.getStackTrace() + " " + ex.toString());
                    }
                }

                System.out.println("Пауза в thread");
                Thread.sleep(100000);//100 sec pause
            }
        } catch (InterruptedException ex)
        {
            System.out.println("Ошибка при обновлении каталоговой структуры");
        }
    }
}
