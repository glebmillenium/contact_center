package es.irkutskenergo.server.netty.fast.swap;

import org.jboss.netty.channel.Channel;
import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import org.codehaus.jackson.map.ObjectMapper;
import es.irkutskenergo.serialization.CatalogForSerialization;
import es.irkutskenergo.server.netty.ftp.FtpServerHandler;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Scanner;
import java.util.logging.Level;
import java.util.logging.Logger;

public class SenderSmallData extends Thread {

    private static int i = 0;
    private Channel channel;
    private String commandFromClient;
    ObjectMapper mapper;
    Map<String, String> aliance;

    public SenderSmallData(Channel channel, String commandFromClient)
    {
        super();
        setName("Fast Swap Server");
        this.channel = channel;
        this.commandFromClient = commandFromClient;
        this.mapper = new ObjectMapper();
        this.aliance = new HashMap<String, String>();
        aliance.put("Simple Catalog for Testing",
                "C:\\Users\\admin\\Desktop\\cat");
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

    private String getAliance(ObjectForSerialization obj) throws IOException
    {
        String[] getAlianceForSend = new String[this.aliance.size()];
        int i = 0;
        for (String key : this.aliance.keySet())
        {
            getAlianceForSend[i] = key;
            i++;
        }
        String resultInFtp = 
                this.mapper.writeValueAsString(getAlianceForSend);
        
        String expectedSize = Integer.toString(resultInFtp.toCharArray().length);
        return this.mapper.writeValueAsString(
                    new ObjectForSerialization("aliance",
                            expectedSize, sendToStorage(resultInFtp)));
    }


    
    private String getCatalog(ObjectForSerialization obj) 
            throws IOException
    {
        String resultInFtp = 
                        getAllFolder(this.aliance.get(obj.param1));;
        String expectedSize = Integer.toString(
                resultInFtp.toCharArray().length);
        System.out.println("Ожидаемый объем данных для отправки: "
                + expectedSize);
        String result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("catalog",
                        expectedSize, sendToStorage(resultInFtp)));
        return result;
    }

    
    /**
     * getAllFolder
     * 
     * Возвращает строку, содержащую рекурсивную информацию о всей внутренней
     * структуре по заданному пути к интересующему каталогу (папке)
     * 
     * @param s
     * @param mapper
     * @return
     * @throws IOException 
     */
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
        return this.mapper.writeValueAsString(result);
    }

    private String getContentFile(ObjectForSerialization obj) throws IOException
    {
        String path = this.aliance.get(obj.param1) + obj.param2;
        
        String resultInFtp = getFileInString(path);
        String expectedSize = Integer.toString(resultInFtp.toCharArray().length);
        return this.mapper.writeValueAsString(
                new ObjectForSerialization("content_file",
                        expectedSize, sendToStorage(resultInFtp)));
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
            Logger.getLogger(SenderSmallData.class.getName()).log(Level.SEVERE, null, ex);
        }
        return s;
    }
    
    private static String sendToStorage(String resultInFtp)
    {
        while(!FtpServerHandler.addHashKeyIdentificator(Integer.toString(i), resultInFtp))
        {
            i++;
            if(i > 1024) i = 0;
        }
        return Integer.toString(i);
    }
}
