/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.file_sharing_service;

import es.irkutskenergo.logging.Logging;
import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import org.codehaus.jackson.map.ObjectMapper;

/**
 *
 * @author admin
 */
public class PrimaryQueryTreatmenter extends Thread {

    Socket socket;
    /**
     * Вспомогательный объект, осуществлящий сериализацию данных
     *
     * @param ObjectMapper
     */
    private ObjectMapper mapper = new ObjectMapper();

    PrimaryQueryTreatmenter(Socket socket)
    {
        mapper = new ObjectMapper();
        this.socket = socket;
    }

    @Override
    public void run()
    {
        Logging.log("Thread: submain FTP start", 4);
        try
        {
            try
            {
                byte[] b = new byte[1024];
                Logging.log("Новое соединение: "
                        + socket.getInetAddress(), 2);

                InputStream inputStream = socket.getInputStream();
                inputStream.read(b);
                String query = new String(b, "UTF-8");
                int key = Integer.parseInt(getObjectFromJson(query).param2);
                if (Storage.containsKey(key))
                {
                    Quadro<Boolean, String, byte[], byte[]> obj
                            = Storage.get(key);
                    byte[] message = obj.param3;
                    if (obj.param1)
                    {
                        Logging.log("Запрос на прием данных от "
                                + "клиента: "
                                + socket.getInetAddress()
                                + " По запросу №" + key
                                + " Объем файла: " + message.length
                                + "байт", 2);

                        SenderData tcpSessionLocal = new SenderData(
                                socket,
                                new String(obj.param3, "UTF-8"),
                                Integer.parseInt(new String(obj.param4,
                                        "UTF-8")));
                        tcpSessionLocal.process();
                    } else
                    {
                        Logging.log("Запрос на отправку данных клиенту: "
                                + socket.getInetAddress() + " По запросу №"
                                + key + " Объем файла: "
                                + message.length + "байт", 2);
                        ReceiveData rdSession = new ReceiveData(socket, message);
                        rdSession.process();
                    }
                    Storage.remove(key);
                } else
                {
                    if (key == -1)
                    {
                        Logging.log("Тестирование соединения клиента с сервером"
                                + "Клиент: "
                                + socket.getInetAddress(), 2);
                        OutputStream outputStream;
                        outputStream = socket.getOutputStream();
                        ObjectMapper mapper = new ObjectMapper();;
                        String resultString = mapper.writeValueAsString(
                                new ObjectForSerialization("answer_on_test_ftp_socket",
                                        "true"));
                        byte[] testPackage = resultString.getBytes();
                        outputStream.write(testPackage, 0, testPackage.length);
                    } else
                    {
                        OutputStream outputStream;
                        outputStream = socket.getOutputStream();
                        byte[] failPackage = new byte[]
                        {
                        };
                        outputStream.write(failPackage, 0, failPackage.length);
                        Logging.log("Такого запроса не существует. "
                                + "Возможно, он был удален по причине "
                                + "длительного застоя в системе"
                                + "Клиент: "
                                + socket.getInetAddress(), 2);
                    }
                }
            } catch (IOException | NumberFormatException exception)
            {
                Logging.log("Сервер при выполнении запроса "
                        + "получил неизвестную ошибку: "
                        + exception.getMessage(), 2);
            }
        } catch (Exception exception)
        {
            Logging.log("Критическая ошибка: "
                    + exception.getMessage(), 1);
        }
        Logging.log("Thread: submain FTP end", 4);
    }

    /**
     * getObjectFromJson - сериализует объект из строки в ObjectForSerialization
     *
     * @param jsonInString
     * @return
     * @throws IOException
     */
    private ObjectForSerialization getObjectFromJson(String jsonInString)
            throws IOException
    {
        return mapper.readValue(jsonInString,
                ObjectForSerialization.class);
    }
}
