/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.other;

import java.util.Date;

/**
 * Класс для JSON сериализации между клиентом и сервером
 *
 * @author Ан Г.В.
 * @date 15.09.2017
 */
public class Client {

    private int accessRights;
    private Date lastConnect;

    public Client(int accessRights, Date lastConnect)
    {
        this.accessRights = accessRights;
        this.lastConnect = lastConnect;
    }

    public void setAccessRights(int accessRights)
    {
        this.accessRights = accessRights;
    }

    public int getAccessRights()
    {
        return this.accessRights;
    }

    public void setLastConnect(Date lastConnect)
    {
        this.lastConnect = lastConnect;
    }

    public void setLastConnect(long milliseconds)
    {
        this.lastConnect = new Date(milliseconds);
    }

    public void setLastConnect()
    {
        this.lastConnect = new Date();
    }

    public Date getLastConnect()
    {
        return this.lastConnect;
    }
}
