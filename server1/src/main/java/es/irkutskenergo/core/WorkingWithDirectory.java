/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.core;

import es.irkutskenergo.serialization.CatalogForSerialization;
import java.io.File;

/**
 *
 * @author admin
 */
public class WorkingWithDirectory {
    
    public static void getAllFolder(String s)
    {
        File f = new File(s);
        String[] sDirList = f.list();
        int i;
        for (i = 0; i < sDirList.length; i++)
        {
            File f1 = new File(
                    s + File.separator + sDirList[i]);

            if (f1.isFile())
            {
                new CatalogForSerialization(sDirList[i], true);
                System.out.println(sDirList[i]);
            } else
            {
                
                getAllFolder(s + "\\" + sDirList[i]);
            }
        }
    }
}
