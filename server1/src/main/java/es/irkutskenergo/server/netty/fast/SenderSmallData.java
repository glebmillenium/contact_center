package es.irkutskenergo.server.netty.fast;

import es.irkutskenergo.other.ExceptionServer;
import es.irkutskenergo.other.Logging;
import es.irkutskenergo.other.Tuple;
import org.jboss.netty.channel.Channel;
import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import org.codehaus.jackson.map.ObjectMapper;
import es.irkutskenergo.serialization.CatalogForSerialization;
import es.irkutskenergo.server.ftp.FtpServer;
import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Scanner;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.io.FileInputStream;

/**
 * SenderSmallData - класс, предназначенный для ответа на первичный запрос от
 * клиента, также генерирует заявку на отправку большого массива данных со
 * стороны ftp сервера
 *
 * @author admin
 */
public class SenderSmallData extends Thread {

    /**
     * i - вспомогательная переменная, учавствует в формировании заявки на ftp
     *
     * @param int
     */
    private static int i = 0;
    /**
     * @param Channel
     */
    private Channel channel;
    /**
     * Команда поступившая от клиента в форме JSON запроса
     *
     * @param String
     */
    private String commandFromClient;
    /**
     * Объект типа ObjectMapper, необходим для сериализации в json строку
     *
     * @param ObjectMapper
     */
    ObjectMapper mapper;
    /**
     * Объект, содержащий данные о текущих файловых системах, которые доступны
     * для просмотра пользователям. Состоит из следующих элементов: param1 -
     * уникальный идентификатор файловой системы (обычно хранится на стороне БД)
     * param2 - кортеж содержащий данные о файловой системе param2.1 - короткое
     * имя файловой системы (то что отправляется пользователю) param2.2 - полный
     * путь к файловой системе (пользователю такие данные недоступны)
     *
     * @param Map<String, Tuple<String, String>>
     */
    private static Map<String, Tuple<String, String>> aliance;
    /**
     * Косвенная переменная, необходима для операции нумерации пришедшей заявки,
     * отправки и идентификации на стороне ftp сервера
     *
     * @param int
     */
    private static int query = 0;
    /**
     * Переменная класса, хранит номер заявки
     */
    private final int numberConnect;

    /**
     * SenderSmallData(Channel, String) - конструктор, инициализирует
     * сериализатор в JSON, получает канал по которому клиент соединен с
     * сервером fast
     *
     * @param channel
     * @param commandFromClient
     */
    public SenderSmallData(Channel channel, String commandFromClient)
    {
        super();
        setName("Fast Swap Server");
        this.channel = channel;
        this.commandFromClient = commandFromClient;
        this.mapper = new ObjectMapper();
        this.aliance = new HashMap<String, Tuple<String, String>>();
        aliance.put("0", new Tuple<String, String>("Инструкции по АСРН",
                "C:\\Users\\admin\\Desktop\\Инструкции"));
        aliance.put("1", new Tuple<String, String>("Обычный каталог для тестирования",
                "C:\\Users\\admin\\Desktop\\cat"));

        query++;
        if (query >= 65536)
        {
            query = 1;
        }
        numberConnect = query;
    }

    /**
     * Переопределенный метод start(), запускает обработку пришедшей заявки от
     * клиента на стороне fast сервера
     *
     * @param void
     * @return void
     */
    @Override
    public void run()
    {
        try
        {
            String response;
            try
            {
                response = getResponse();
            } catch (IOException ex)
            {

                response = this.mapper.writeValueAsString(
                        new ObjectForSerialization("error",
                                "0", "Произошла ошибка при выполнении команды"));
                Logging.log("Произошла ошибка при выполнении команды."
                        + " Клиент " + channel.toString()
                        + " Номер запроса " + this.numberConnect
                        + ". Сообщение от клиента: "
                        + commandFromClient, 1);
            }
            sendToClient(response);
        } catch (IOException ex)
        {
            Logging.log("Произошла ошибка при выполнении команды и её отправке."
                    + " Клиент " + channel.toString()
                    + " Номер запроса " + this.numberConnect
                    + ". Сообщение от клиента: "
                    + commandFromClient, 1);
        }
    }

    /**
     * sendToClient - осуществляет отправку данных засериализованных в JSON
     * клиенту
     *
     * @param response текст сообщения
     */
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

     /**
      * getResponseWhenError
      * Метод генерирующий JSON ответ клиенту при возникновении ошибки 
      *                 (Не поддерживается!)
      * @return
      * @throws IOException 
      */
    private String getResponseWhenError() throws IOException
    {
        String result;
        try
        {
            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("error"));
        } catch (IOException ex)
        {
            Logging.log("Ошибка при сериализации объекта "
                    + this.channel.toString() + ") Номер запроса: "
                    + this.numberConnect, 1);

            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("error",
                            "0", "Серверу не удалось выполнить требуемую команду"));
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
        try
        {
            if (obj.command.equals("error"))
            {
                throw new ExceptionServer("Случилась неизвестная ошибка "
                        + "на стороне клиента " + channel.toString()
                        + " номер подключения " + numberConnect);
            } else if (obj.command.equals("get_aliance"))
            {
                Logging.log("Обработка запроса на получение всех файловых систем "
                        + this.channel.toString() + ") Номер запроса: "
                        + this.numberConnect, 1);
                result = getAliance(obj);
            } else if (obj.command.equals("get_catalog"))
            {
                Logging.log("Обработка запроса на получение "
                        + "содержимого файловой системы "
                        + this.aliance.get(obj.param1).y + " Канал"
                        + this.channel.toString() + ") Номер запроса: "
                        + this.numberConnect, 1);
                result = getCatalog(obj);
            } else if (obj.command.equals("get_content_file"))
            {
                result = getContentFile(obj);
            }
            Logging.log("Запрос успешно выполнен, отправление данных: "
                    + this.channel.toString() + ") Номер запроса: "
                    + this.numberConnect, 1);
        } catch (ExceptionServer e)
        {
            Logging.log(e.toString(), 1);
            result = "";
        }

        return result;
    }

    /**
     * getAliance
     * Запускает процесс получения сведений о файловых системах
     * 
     * @param obj Объект, не несет информативной нагрузки (AUTHORIZATION TODO!)
     * @return String засериализованная строка, которая хранит информацию о
     * файловых системах
     * @throws IOException 
     */
    private String getAliance(ObjectForSerialization obj) throws IOException
    {
        String[] getAlianceForSend = new String[this.aliance.size()];
        int i = 0;
        for (Map.Entry<String, Tuple<String, String>> value
                : this.aliance.entrySet())
        {
            getAlianceForSend[i] = this.mapper.writeValueAsString(
                    new String[]
                    {
                        value.getKey(), value.getValue().x
                    });
            i++;
        }
        byte[] resultInFtp
                = (this.mapper.writeValueAsString(getAlianceForSend)
                        + "\0").getBytes("UTF-8");

        String expectedSize = Integer.toString(resultInFtp.length);

        return this.mapper.writeValueAsString(
                new ObjectForSerialization("aliance",
                        expectedSize, sendToStorageInFtpServer(resultInFtp, 0)));
    }

    /**
     * getCatalog
     * Метод, запускающий процесс получения всех каталогов в интересующей 
     * файловой системе
     * 
     * @param  obj         Объект хранящий информацию о файловой системе
     * @return String      Информация о содержимом каталога в засериализованном 
     * JSON виде
     * @throws IOException 
     */
    private String getCatalog(ObjectForSerialization obj)
            throws IOException
    {
        byte[] resultInFtp
                = (getAllFolder(this.aliance.get(obj.param1).y)
                        + "\0").getBytes("UTF-8");

        String expectedSize = Integer.toString(
                resultInFtp.length);
        String result = this.mapper.writeValueAsString(
                new ObjectForSerialization("catalog",
                        expectedSize, sendToStorageInFtpServer(resultInFtp, 0)));
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

    /**
     * Запускает процесс получения требуемого файла с удаленной файловой системы
     * раскодирует засериализованный объект в котором хранится информация о
     * файловой системе, относительном пути к файлу, включая имя самого файла.
     *
     * @param obj
     * @return
     * @throws IOException
     */
    private String getContentFile(ObjectForSerialization obj) throws IOException
    {
        String path = this.aliance.get(obj.param1).y
                + (new String(obj.param4_array, "UTF-8"));
        Logging.log("Обработка запроса на получение файла по пути: " + path
                + " Канал " + this.channel.toString() + ") Номер запроса: "
                + this.numberConnect, 1);
        byte[] resultInFtp = getFileInArrayByte(path);
        String expectedSize = Integer.toString(resultInFtp.length);
        return this.mapper.writeValueAsString(
                new ObjectForSerialization("content_file",
                        expectedSize, sendToStorageInFtpServer(resultInFtp, 0)));
    }

    /**
     * getFileInString Получить содержимое файла по указанному пути в String
     * кодировки UTF-16 (Не рекомендуется для использования)
     *
     * @param way Путь к файлу
     * @return String содержимое файла в кодировке UTF-16
     */
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

    /**
     * getFileInArrayByte Получить содержимое файла по указанному пути в виде
     * byte массива (Не рекомендуется для чтения большого объема файла)
     *
     * @param way Путь к файлу
     * @return byte[] Содержимое файла
     */
    private byte[] getFileInArrayByte(String way)
    {
        try
        {
            FileInputStream fin = new FileInputStream(way);
            byte[] buffer = new byte[fin.available()];
            fin.read(buffer, 0, fin.available());
            fin.close();

            return buffer;
        } catch (FileNotFoundException e)
        {

        } catch (IOException e)
        {

        }
        
        return new byte[]
        {
        };
    }

    /**
     * sendToStorageInFtpServer Отправка заявки (и содержимое заявки) на
     * хранение в FTP сервере до тех пор, пока клиент не придет за ней на
     * получение, либо пока не истечет время её хранения (в случае запущенного
     * граббера)
     *
     * @param resultInFtp Содержимое заявки
     * @param typeQuery Тип заявки
     * @return String Идентификатор заявки
     */
    private static String sendToStorageInFtpServer(byte[] resultInFtp, int typeQuery)
    {
        String typeQueryToInt = String.valueOf(typeQuery);
        while (!FtpServer.addHashKeyIdentificator(Integer.toString(i), resultInFtp, typeQueryToInt))
        {
            i++;
            if (i > 1024)
            {
                i = 0;
            }
        }
        return Integer.toString(i);
    }
}
