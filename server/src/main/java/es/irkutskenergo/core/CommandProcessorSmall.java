/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.core;

import org.jboss.netty.channel.Channel;
import org.jboss.netty.channel.MessageEvent;

public class CommandProcessorSmall extends Thread {

    private Channel channel;
    private String commandFromClient;

    public CommandProcessorSmall(Channel channel, String commandFromClient)
    {
        super();
        setName("cmdProcessor");
        this.channel = channel;
        this.commandFromClient = commandFromClient;
    }

    @Override
    public void run()
    {
        try
        {
            String response = "Response text in XML format"; //Вместо игровой логики сгенерируем рыбу :)
            String[] arrayCommands = commandFromClient.split(" ");
            if (arrayCommands.length == 2)
            {
                response = getResponseForOneArguments(arrayCommands);
            }

            sendToClient(response);

            System.out.print("IN > " + this.commandFromClient + "\n");
            System.out.print("OUT > " + response + "\n");
        } catch (Exception error)
        {
            error.printStackTrace();
        }
    }

    private void sendToClient(String response)
    {
        this.channel.write(response + "\0");
    }

    private String getResponseForOneArguments(String[] arrayCommands)
    {
        if (arrayCommands[0].equals("get_aliance"))
        {

        } else if (arrayCommands[0].equals("get_file"))
        {

        }
        return "";
    }
}
