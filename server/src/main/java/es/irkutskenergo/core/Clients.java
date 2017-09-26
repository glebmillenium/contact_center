/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.core;

import es.irkutskenergo.other.Client;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import sun.awt.Mutex;

public class Clients {

    private static Map<String, Client> listContainsAllClientsConnect
            = new HashMap<String, Client>();
    private static Mutex possibilityChange;

    public Clients()
    {

    }

    /**
     *
     * @return
     */
    public int connect(String address)
    {
        int access;

        if (listContainsAllClientsConnect.containsKey(address))
        {
            //need analitycal process acess for user
            possibilityChange.lock();
            access = 7;
            listContainsAllClientsConnect.put(address, new Client(access,
                    new Date()));
            possibilityChange.unlock();
        } else
        {
            access = listContainsAllClientsConnect.get(address).getAccessRights();
        }
        return access;
    }

    public void grabber(long maximumSimple)
    {
        long currentDate = new Date().getTime();
        possibilityChange.lock();
        for (Map.Entry<String, Client> entry : listContainsAllClientsConnect.entrySet())
        {
            if (currentDate - entry.getValue().getLastConnect().getTime()
                    > maximumSimple)
            {
                listContainsAllClientsConnect.remove(entry.getKey());
            }
        }
        possibilityChange.unlock();
    }

    public void grabber()
    {
        long maximumSimple = 1200000;
        long currentDate = new Date().getTime();
        possibilityChange.lock();
        for (Map.Entry<String, Client> entry
                : listContainsAllClientsConnect.entrySet())
        {
            if (currentDate - entry.getValue().getLastConnect().getTime()
                    > maximumSimple)
            {
                listContainsAllClientsConnect.remove(entry.getKey());
            }
        }
        possibilityChange.unlock();
    }
}
