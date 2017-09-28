package es.irkutskenergo.core;

import org.jboss.netty.channel.Channel;
import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import org.codehaus.jackson.map.ObjectMapper;
import es.irkutskenergo.serialization.CatalogForSerialization;
import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;
import java.util.logging.Level;
import java.util.logging.Logger;

public class CommandProcessorSmall extends Thread {

    private Channel channel;
    private String commandFromClient;
    ObjectMapper mapper;

    public CommandProcessorSmall(Channel channel, String commandFromClient)
    {
        super();
        setName("cmdProcessor");
        this.channel = channel;
        this.commandFromClient = commandFromClient;
        this.mapper = new ObjectMapper();
    }

    @Override
    public void run()
    {
        String response;
        try
        {
            response = getResponse();
        } catch (IOException ex)
        {
            response = getResponseWhenError();
            System.out.println("Не получилось обработать входящую команду");
        }
        sendToClient(response);
    }

    private void sendToClient(String response)
    {
        this.channel.write(response + "\0");
    }

    private ObjectForSerialization getObjectFromJson(String jsonInString)
            throws IOException
    {

        return this.mapper.readValue(jsonInString,
                ObjectForSerialization.class);
    }

    private String getResponseWhenError()
    {
        String result;
        try
        {
            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("error"));
        } catch (IOException ex)
        {
            System.out.println("Ошибка при сериализации объекта");
            result = "";
        }
        return result;
    }

    /**
     * getResponse()
     *
     * Метод осуществляющий анализ входящей команды для формирования ответа в
     * JSON виде
     *
     * @author Ан Г.В.
     * @return String Сериализованная строка отправки клиенту
     * @throws IOException
     */
    private String getResponse() throws IOException
    {
        ObjectForSerialization obj = getObjectFromJson(
                this.commandFromClient);
        String result = null;
        if (obj.command.equals("error"))
        {
            //debug
            throw new IOException();
        } else if (obj.command.equals("get_aliance"))
        {
            result = getAliance(obj);
        } else if (obj.command.equals("get_catalog"))
        {
            result = getCatalog(obj);
        } else if (obj.command.equals("get_content_file"))
        {
            result = getContentFile(obj);
        }

        return result;
    }

    String[] aliance = new String[]
    {
        "C:\\Users\\admin\\Desktop\\catalog"
    };

    private String getAliance(ObjectForSerialization obj) throws IOException
    {
        return this.mapper.writeValueAsString(
                new ObjectForSerialization("aliance",
                        this.mapper.writeValueAsString(aliance))
        );
    }

    private String getCatalog(ObjectForSerialization obj) throws IOException
    {
        String aliance = obj.param2;
        String result = getAllFolder(aliance);
        return this.mapper.writeValueAsString(
                new ObjectForSerialization("catalog",
                        result)
                );
    }

    public String getAllFolder(String s) throws IOException
    {
        File f = new File(s);
        String[] sDirList = f.list();
        int i;
        List<String> result = new ArrayList<String>();
        for (i = 0; i < sDirList.length; i++)
        {
            File f1 = new File(
                    s + File.separator + sDirList[i]);

            if (f1.isFile())
            {
                result.add(this.mapper.writeValueAsString(
                        new CatalogForSerialization(sDirList[i], true)));
            } else
            {
                result.add(this.mapper.writeValueAsString(
                        new CatalogForSerialization(
                                sDirList[i], false,
                                getAllFolder(s + "/" + sDirList[i])
                        )
                )
                );
            }
        }
        return this.mapper.writeValueAsString(result)
                .replaceAll("[\\\\]+", "\\\\");
    }

    private String getContentFile(ObjectForSerialization obj) throws IOException
    {
        String path = obj.param1;
        return this.mapper.writeValueAsString(
                new ObjectForSerialization("content_file", getFileInString(path)));
    }

    private String getFileInString(String way)
    {
        String s = "";
        try
        {
            Scanner in = new Scanner(new File(way));
            while (in.hasNext())
            {
                s += in.nextLine() + "\r\n";
            }
            in.close();

        } catch (FileNotFoundException ex)
        {
            Logger.getLogger(CommandProcessorSmall.class.getName()).log(Level.SEVERE, null, ex);
        }
        return s;
    }
}
