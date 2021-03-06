package es.irkutskenergo.server.netty.fast;

import es.irkutskenergo.core.command.InteractiveWithDataBase;
import es.irkutskenergo.other.ExceptionServer;
import es.irkutskenergo.other.Logging;
import es.irkutskenergo.other.Storage;
import es.irkutskenergo.other.Triple;
import io.netty.channel.Channel;
import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import org.codehaus.jackson.map.ObjectMapper;
import es.irkutskenergo.serialization.CatalogForSerialization;
import io.netty.buffer.Unpooled;
import io.netty.channel.ChannelHandlerContext;
import java.io.BufferedReader;
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
import java.io.FileReader;
import java.io.UnsupportedEncodingException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

/**
 * SenderSmallData - класс, предназначенный для ответа на первичный запрос от
 * клиента, также генерирует заявку на отправку большого массива данных со
 * стороны ftp сервера
 *
 * @author admin
 */
public class SenderSmallData {

    /**
     * i - вспомогательная переменная, учавствует в формировании заявки на ftp
     *
     * @param int
     */
    private static int i = 0;
    /**
     * @param Channel
     */
    private ChannelHandlerContext ctx;
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
    private static Map<String, Triple<String, String, String>> aliance;
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
     * Переменная по которой отслеживается время последнего действия треда в
     * системе
     */
    /**
     * SenderSmallData(Channel, String) - конструктор, инициализирует
     * сериализатор в JSON, получает канал по которому клиент соединен с
     * сервером fast
     *
     * @param channel
     * @param commandFromClient
     */
    public SenderSmallData(ChannelHandlerContext ctx)
    {
        super();
        //setName("Fast Swap Server");
        this.ctx = ctx;
        this.mapper = new ObjectMapper();
        SenderSmallData.aliance = InteractiveWithDataBase.getRootAliance();

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
    //@Override
    public void process(String commandFromClient)
    {
        this.commandFromClient = commandFromClient;
        Logging.log("Thread: submain FAST start", 4);
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
                        + " Клиент " + ctx.channel().toString()
                        + " Номер запроса " + this.numberConnect
                        + ". Сообщение от клиента: "
                        + commandFromClient, 1);
            }
            sendToClient(response);
        } catch (Exception ex)
        {
            Logging.log("Произошла критическая ошибка при выполнении команды и её отправке."
                    + " Клиент " + ctx.channel().toString()
                    + " Номер запроса " + this.numberConnect
                    + ". Сообщение от клиента: "
                    + commandFromClient + " " + ex.getMessage(), 1);
        }
        Logging.log("Thread: submain FAST end", 4);
    }

    /**
     * sendToClient - осуществляет отправку данных засериализованных в JSON
     * клиенту
     *
     * @param response текст сообщения
     */
    private void sendToClient(String response) throws UnsupportedEncodingException
    {
        if (ctx.channel().isWritable())
        {
            ctx.channel().writeAndFlush(Unpooled.wrappedBuffer((response + "\0").getBytes("UTF-8")));
        }
        //this.channel.write(response + "\0");
    }

    private ObjectForSerialization getObjectFromJson(String jsonInString)
            throws IOException
    {
        return this.mapper.readValue(jsonInString,
                ObjectForSerialization.class);
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
                        + "на стороне клиента " + ctx.channel().toString()
                        + " номер подключения " + numberConnect);
            } else if (obj.command.equals("get_aliance"))
            {
                Logging.log("Обработка запроса на получение всех файловых систем "
                        + ctx.channel().toString() + ") Номер запроса: "
                        + this.numberConnect, 1);
                result = getAliance(obj);
            } else if (obj.command.equals("get_catalog"))
            {
                Logging.log("Обработка запроса на получение "
                        + "содержимого файловой системы "
                        + this.aliance.get(obj.param1).param2 + " Канал"
                        + ctx.channel().toString() + ") Номер запроса: "
                        + this.numberConnect, 3);
                result = getCatalog(obj);
            } else if (obj.command.equals("get_content_file"))
            {
                Logging.log("Обработка запроса на получение содержимого файла "
                        + ctx.channel().toString() + ") Номер запроса: "
                        + this.numberConnect, 1);
                result = getContentFile(obj);
            } else if (obj.command.equals("get_update"))
            {
                Logging.log("Обработка запроса на получение содержимого файла "
                        + "для обновления " + ctx.channel().toString()
                        + ") Номер запроса: " + this.numberConnect, 1);
                result = getFileForRemoteUpdate(obj);
            } else if (obj.command.equals("try_remove"))
            {
                Logging.log("Обработка запроса на удаление файла из файловой "
                        + "системы " + ctx.channel().toString()
                        + ") Номер запроса: " + this.numberConnect, 1);
                result = tryRemoveFile(obj);
            } else if (obj.command.equals("try_create_dir"))
            {
                Logging.log("Обработка запроса на создание каталога в системе "
                        + "файловой системы" + ctx.channel().toString()
                        + ") Номер запроса: " + this.numberConnect, 1);
                result = tryCreateDir(obj);
            } else if (obj.command.equals("try_rename"))
            {
                Logging.log("Обработка запроса на переименование ресурса "
                        + "файловой системы" + ctx.channel().toString()
                        + ") Номер запроса: " + this.numberConnect, 1);
                result = tryRenameFile(obj);
            } else if (obj.command.equals("try_upload"))
            {
                Logging.log("Обработка запроса на загрузку нового файла в "
                        + "файловую систему " + ctx.channel().toString()
                        + ") Номер запроса: " + this.numberConnect, 1);
                result = setupToUpload(obj);
            } else if (obj.command.equals("test_fast_socket"))
            {
                Logging.log("Тестирование соединения клиента с сервером "
                        + ctx.channel().toString()
                        + ") Номер запроса: " + this.numberConnect, 1);
                result = testConnectionWithFastSocket(obj);
            } else if (obj.command.equals("get_version"))
            {
                Logging.log("Получение версии " + obj.param1 + " "
                        + ctx.channel().toString()
                        + ") Номер запроса: " + this.numberConnect, 1);
                result = getVersion(obj);
            } else if (obj.command.equals("get_difference_version"))
            {
                Logging.log("Получение расхождений в версиях "
                        + ctx.channel().toString()
                        + ") Номер запроса: " + this.numberConnect, 1);
                result = getDifferenceInVersion(obj);
            } else if (obj.command.equals("auth"))
            {
                Logging.log("Авторизация пользователя с сервером "
                        + ctx.channel().toString()
                        + ") Номер запроса: " + this.numberConnect
                        + " Имя пользователя: "
                        + new String(obj.param4_array, "UTF-8"), 1);
                result = getRightAccess(obj);
            } else
            {
                Logging.log("Полученный запрос содержит "
                        + "неизвестную для сервера команду "
                        + ctx.channel().toString()
                        + ") Номер запроса: " + this.numberConnect, 1);
                result = getResponseWhenUnknownCommand();
            }
            Logging.log("Запрос успешно выполнен, отправление данных: "
                    + ctx.channel().toString() + ") Номер запроса: "
                    + this.numberConnect, 1);
        } catch (Exception e)
        {
            Logging.log("Критическая ошибка обработки командного запроса"
                    + e.getMessage(), 1);
            result = "";
        }
        return result;
    }

    /**
     * getResponseWhenError Метод генерирующий JSON ответ клиенту при
     * возникновении ошибки (Не поддерживается!)
     *
     * @return
     * @throws IOException
     */
    private String getResponseWhenUnknownCommand() throws IOException
    {
        String result;
        try
        {
            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("error", "UNKNOWN"));
        } catch (IOException ex)
        {
            Logging.log("Ошибка при сериализации объекта "
                    + ctx.channel().toString() + ") Номер запроса: "
                    + this.numberConnect + " Ошибка: " + ex.getMessage(), 1);

            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("error",
                            "0", "Серверу не удалось выполнить требуемую команду"));
        }
        return result;
    }

    /**
     * getAliance Запускает процесс получения сведений о файловых системах
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
        for (Map.Entry<String, Triple<String, String, String>> value
                : this.aliance.entrySet())
        {
            getAlianceForSend[i] = this.mapper.writeValueAsString(
                    new String[]
                    {
                        value.getKey(), value.getValue().param1, value.getValue().param3
                    });
            i++;
        }
        byte[] resultInFtp
                = (this.mapper.writeValueAsString(getAlianceForSend)
                        + "\0").getBytes("UTF-8");

        String expectedSize = Integer.toString(resultInFtp.length);
        return this.mapper.writeValueAsString(
                new ObjectForSerialization("aliance",
                        expectedSize, Storage.sendToStorageInFtpServer(false,
                                obj.command, resultInFtp, new byte[]
                                {
                })));
    }

    /**
     * getCatalog Метод, запускающий процесс получения всех каталогов в
     * интересующей файловой системе
     *
     * @param obj Объект хранящий информацию о файловой системе
     * @return String Информация о содержимом каталога в засериализованном JSON
     * виде
     * @throws IOException
     */
    private String getCatalog(ObjectForSerialization obj)
            throws IOException
    {
        byte[] resultInFtp
                = (getAllFolder(this.aliance.get(obj.param1).param2)
                        + "\0").getBytes("UTF-8");

        String expectedSize = Integer.toString(
                resultInFtp.length);
        String result = this.mapper.writeValueAsString(
                new ObjectForSerialization("catalog",
                        expectedSize, Storage.sendToStorageInFtpServer(false,
                                obj.command, resultInFtp, new byte[]
                                {
                })
                ));
        return result;
    }

    private String getVersion(ObjectForSerialization obj) throws IOException
    {
        String listFolder = getAllFolder("update_storage//" + obj.param1) + "\0";
        byte[] resultInFtp
                = (listFolder).getBytes("UTF-8");

        String expectedSize = Integer.toString(
                resultInFtp.length);
        String result = this.mapper.writeValueAsString(
                new ObjectForSerialization("version",
                        expectedSize, Storage.sendToStorageInFtpServer(false,
                                obj.command, resultInFtp, new byte[]
                                {
                })
                ));
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
        String result = "";
        try
        {
            String path = this.aliance.get(obj.param1).param2
                    + (new String(obj.param4_array, "UTF-8"));
            Logging.log("Обработка запроса на получение файла по пути: " + path
                    + " Канал " + ctx.channel().toString() + ") Номер запроса: "
                    + this.numberConnect, 1);
            byte[] resultInFtp = getFileInArrayByte(path);
            String expectedSize = Integer.toString(resultInFtp.length);
            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("content_file",
                            expectedSize, Storage.sendToStorageInFtpServer(false,
                                    obj.command, resultInFtp, new byte[]
                                    {
                    })));
        } catch (Exception e)
        {
            throw new IOException();
        }
        return result;
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
            Logger.getLogger(SenderSmallData.class.getName()).log(Level.SEVERE,
                    null, ex);
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
     * Создание заявки в FTP-сервере на прием данных
     *
     * @param ObjectForSerialization
     * @return String
     * @throws IOException
     */
    private String setupToUpload(ObjectForSerialization obj) throws IOException
    {
        String strPathToFile = this.aliance.get(obj.param2).param2
                + (new String(obj.param4_array, "UTF-8")) + "\\"
                + (new String(obj.param5_array, "UTF-8"));
        byte[] bytePathToFile = strPathToFile.getBytes("UTF-8");
        String query = Storage.sendToStorageInFtpServer(true, obj.command,
                bytePathToFile, obj.param1.getBytes("UTF-8"));
        return this.mapper.writeValueAsString(
                new ObjectForSerialization("upload",
                        query, "1"));
    }

    /**
     * Переименовывает файл в указанной подсистеме
     *
     * @param obj
     * @return
     * @throws IOException
     */
    private String tryRenameFile(ObjectForSerialization obj) throws IOException
    {
        String result = "";
        try
        {
            String newName = new String(obj.param5_array, "UTF-8");
            String ftpPath = this.aliance.get(obj.param1).param2;
            String relativeOldPath = (new String(obj.param4_array, "UTF-8"));
            String relativeWay = ftpPath + relativeOldPath;
            Path source = Paths.get(relativeWay);
            Logging.log("Переименовывание ресурса по пути: " + relativeWay
                    + " Канал " + ctx.channel().toString() + ") Номер запроса: "
                    + this.numberConnect, 1);
            Files.move(source, source.resolveSibling(newName));
            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("rename", "1"));
        } catch (UnsupportedEncodingException ex)
        {
            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("error", "0"));
        }
        return result;
    }

    private String tryCreateDir(ObjectForSerialization obj) throws IOException
    {
        String result = "";
        try
        {
            String ftpPath = this.aliance.get(obj.param1).param2;
            String relativePath = (new String(obj.param4_array, "UTF-8")) + "\\"
                    + (new String(obj.param5_array, "UTF-8"));
            String relativeWay = ftpPath + relativePath;

            File folder = new File(relativeWay);
            if (!folder.exists())
            {
                folder.mkdir();
                Logging.log("Создание каталога по пути: " + relativeWay
                        + " Канал " + ctx.channel().toString() + ") Номер запроса: "
                        + this.numberConnect, 1);
                result = this.mapper.writeValueAsString(
                        new ObjectForSerialization("create_dir", "1"));
            } else
            {
                result = this.mapper.writeValueAsString(
                        new ObjectForSerialization("create_dir", "0"));
            }
        } catch (UnsupportedEncodingException ex)
        {
            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("error", "0"));
        }
        return result;
    }

    /**
     * Удаляет файл из указанной подсистемы
     *
     * @param obj
     * @return
     * @throws IOException
     */
    private String tryRemoveFile(ObjectForSerialization obj) throws IOException
    {
        String result = "";
        try
        {
            String ftpPath = this.aliance.get(obj.param1).param2;
            String relativePath = (new String(obj.param4_array, "UTF-8"));
            String relativeWay = ftpPath + relativePath;
            deleteObjectFileSystem(new File(relativeWay));
            Logging.log("Удаление ресурса по пути: " + relativeWay
                    + " Канал " + ctx.channel().toString() + ") Номер запроса: "
                    + this.numberConnect, 1);
            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("remove", "1"));
        } catch (UnsupportedEncodingException ex)
        {
            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("error", "0"));
        }
        return result;
    }

    public void deleteObjectFileSystem(File file)
    {
        if (!file.exists())
        {
            return;
        }
        if (file.isDirectory())
        {
            for (File f : file.listFiles())
            {
                deleteObjectFileSystem(f);
            }
            file.delete();
        } else
        {
            file.delete();
        }
    }

    private String testConnectionWithFastSocket(ObjectForSerialization obj)
    {
        String result = "";
        try
        {
            result = this.mapper.writeValueAsString(
                    new ObjectForSerialization("answer_on_test_fast_socket", "true"));
        } catch (IOException ex)
        {
            Logging.log("Не удалось произвести сериализацию файла "
                    + " Канал " + ctx.channel().toString() + " Номер запроса: "
                    + this.numberConnect, 1);
        }
        return result;
    }

    private String getRightAccess(ObjectForSerialization obj) throws IOException
    {
        String login;
        String password;

        try
        {
            login = new String(obj.param4_array, "UTF-8");
            password = new String(obj.param5_array, "UTF-8");
        } catch (UnsupportedEncodingException ex)
        {
            Logging.log("Не удалось получить права доступа для выбранного логина"
                    + "(ошибка кодировки) "
                    + " Канал " + ctx.channel().toString() + " Номер запроса: "
                    + this.numberConnect, 1);
            login = "";
            password = "";
        }
        String result;
        int right = InteractiveWithDataBase.getRightAccess(login, password);

        String version = new String(Files.readAllBytes(Paths.get("version")));
        switch (right)
        {
            case 1:
                result = this.mapper.writeValueAsString(
                        new ObjectForSerialization("auth",
                                String.valueOf(right), version, "",
                                new String("Уровень доступа: смена "
                                        + "файловой системы, просмотр каталогов,"
                                        + " содержимого файлов")
                                        .getBytes("UTF-8")));
                break;
            case 3:
                result = this.mapper.writeValueAsString(
                        new ObjectForSerialization("auth",
                                String.valueOf(right), version, "",
                                new String("Уровень доступа: смена "
                                        + "файловой системы, просмотр каталогов;"
                                        + " загрузка, изменение, удаление, "
                                        + "просмотр файлов; удаление, создание "
                                        + "новых каталогов").getBytes("UTF-8")));
                break;
            default:
                result = this.mapper.writeValueAsString(
                        new ObjectForSerialization("auth",
                                "-1", version, "",
                                new String("Уровень доступа: отсутствует")
                                        .getBytes("UTF-8")));
                break;
        }

        return result;
    }

    private String getDifferenceInVersion(ObjectForSerialization obj) throws IOException
    {
        String currentVersion = obj.param1;
        String[] differenceInVersions = InteractiveWithDataBase.getDifferenceVersion(currentVersion);

        byte[] resultInFtp
                = (this.mapper.writeValueAsString(differenceInVersions)
                        + "\0").getBytes("UTF-8");

        String expectedSize = Integer.toString(resultInFtp.length);
        return this.mapper.writeValueAsString(
                new ObjectForSerialization("version",
                        expectedSize, Storage.sendToStorageInFtpServer(false,
                                obj.command, resultInFtp, new byte[]
                                {
                })));
    }

    private String getFileForRemoteUpdate(ObjectForSerialization obj) throws UnsupportedEncodingException, IOException
    {
        String path = ".//update_storage//" + obj.param1 + "//" + (new String(obj.param4_array, "UTF-8"));
        Logging.log("Обработка запроса на получение файла по пути: " + path
                + " Канал " + ctx.channel().toString() + ") Номер запроса: "
                + this.numberConnect, 1);
        byte[] resultInFtp = getFileInArrayByte(path);
        String expectedSize = Integer.toString(resultInFtp.length);
        return this.mapper.writeValueAsString(
                new ObjectForSerialization("update",
                        expectedSize, Storage.sendToStorageInFtpServer(false,
                                obj.command, resultInFtp, new byte[]
                                {
                })));
    }
}
