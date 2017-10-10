/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.serialization;

/**
 *
 * @author admin
 */
public class ObjectForSerialization {

    public String command;
    public String param1 = null;
    public String param2 = null;
    public String param3 = null;
    public byte[] param4_array = null;
    public byte[] param5_array = null;

    public ObjectForSerialization()
    {
        this.command = null;
        this.param1 = null;
        this.param2 = null;
        this.param3 = null;
        this.param4_array = null;
        this.param5_array = null;
    }

    public ObjectForSerialization(String command)
    {
        this.command = command;
        this.param1 = null;
        this.param2 = null;
        this.param3 = null;
        this.param4_array = null;
        this.param5_array = null;
    }

    public ObjectForSerialization(String command, String param1)
    {
        this.command = command;
        this.param1 = param1;
        this.param2 = null;
        this.param3 = null;
        this.param4_array = null;
        this.param5_array = null;
    }

    public ObjectForSerialization(String command, String param1, String param2)
    {
        this.command = command;
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = null;
        this.param4_array = null;
        this.param5_array = null;
    }

    public ObjectForSerialization(String command, String param1, String param2,
            String param3)
    {
        this.command = command;
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param4_array = null;
        this.param5_array = null;
    }

    public ObjectForSerialization(String command, String param1, String param2,
            String param3, byte[] byteArray)
    {
        this.command = command;
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param4_array = byteArray;
        this.param5_array = null;
    }
    
        public ObjectForSerialization(String command, String param1, String param2,
            String param3, byte[] byteArray, byte[] byteArray2)
    {
        this.command = command;
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param4_array = byteArray;
        this.param5_array = byteArray2;
    }

    public String ToString()
    {
        return "command: " + this.command + "\nparam1: " + this.param1
                + "\nparam2: " + this.param2 + "\nparam3: " + this.param3;
    }
}
