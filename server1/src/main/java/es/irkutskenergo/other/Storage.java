package es.irkutskenergo.other;

import java.util.Date;
import java.util.HashMap;
import java.util.Map;

/**
 * Статический класс предназначен для хранения промежуточных заявок на стороне
 * ftp сервера.
 *
 * @author Ан Г.В.
 * @date 19.10.2017
 */
public class Storage {

    /**
     * queryStorageDate Хранит время создания заявки
     *
     * @param Map<Integer, Date>
     */
    private static Map<Integer, Date> queryStorageDate
            = new HashMap<Integer, Date>();

    /**
     * queryStorageInformation Хранит основную и дополнительную информацию
     * заявки
     *
     * @param Map<Integer, Tuple<byte[], byte[]>>
     */
    private static Map<Integer, Tuple<byte[], byte[]>> queryStorageInformation = new HashMap<Integer, Tuple<byte[], byte[]>>();

    /**
     * queryStorageCommand Хранит информацию о типе взаимодействия сервера с
     * клиентом: false - отправка данных клиенту true - прием данных от клиента
     * String - название команды
     *
     * @param Map<Integer, Tuple<Boolean, String>>
     */
    private static Map<Integer, Tuple<Boolean, String>> queryStorageCommand
            = new HashMap<Integer, Tuple<Boolean, String>>();

    /**
     * idQuery Вспомогательная информацию необходимая для формирования
     * идентификатора заявки
     *
     * @param int
     */
    private static int idQuery = 1;

    /**
     * add Метод добавляет заявку (со всем содержимым) в хранилище Если
     * невозможно добавить заявку в систему (все места заняты, максимальное
     * количество заявок в одно время - 65537), метод вернет -1.
     *
     * @param typeQuery
     * @param nameCommand
     * @param information
     * @param additionalInformation
     * @return
     */
    public static int add(boolean typeQuery, String nameCommand,
            byte[] information, byte[] additionalInformation)
    {
        int circle = 1;
        boolean res = queryStorageDate.containsKey(idQuery);
        while (queryStorageDate.containsKey(idQuery))
        {
            circle++;
            if (circle > 65537)
            {
                return -1;
            }
            idQuery++;
            if (idQuery > 65536)
            {
                idQuery = 1;
            }
        }
        queryStorageDate.put(idQuery, new Date());
        queryStorageCommand.put(idQuery, new Tuple<>(typeQuery, nameCommand));
        queryStorageInformation.put(idQuery, new Tuple<>(information,
                additionalInformation));
        return idQuery;
    }

    /**
     * clear Запускает процесс очистки устаревших заявок
     *
     * @param maxUptime - максимальное время простоя заявки в системе
     */
    public static void clear(int maxUptime)
    {
        long currentDate = new Date().getTime();
        Logging.log("Начало чистки \"старых заявок\" с FTP сервера", 2);
        queryStorageDate.forEach((key, value) ->
        {
            if (value.getTime() > maxUptime + currentDate)
            {
                Logging.log("Чистка в FTP сервере удалена заявка: "
                        + key.toString() + " Содержимое: "
                        + value.toString(), 2);

                remove(key);
            }
        });
    }

    /**
     * containsKey Проверяет наличие ключа в хранилище, возвращает результат
     * проверки
     *
     * @param key
     * @return boolean
     */
    public static boolean containsKey(int key)
    {
        return queryStorageDate.containsKey(key);
    }

    /**
     * get Создает объект (4-х местный кортеж), который содержим информацию о
     * типе заявки, названии команды, несущей заявочной информации,
     * дополнительной информации
     *
     * @param key
     * @return
     */
    public static Quadro<Boolean, String, byte[], byte[]> get(int key)
    {
        return new Quadro<>(queryStorageCommand.get(key).param1,
                queryStorageCommand.get(key).param2,
                queryStorageInformation.get(key).param1,
                queryStorageInformation.get(key).param2);
    }

    /**
     * cut Создает объект (4-х местный кортеж), который содержим информацию о
     * типе заявки, названии команды, несущей заявочной информации,
     * дополнительной информации, и удаляет заявку из хранилища
     *
     * @param key
     * @return
     */
    public static Quadro<Boolean, String, byte[], byte[]> cut(int key)
    {
        Quadro<Boolean, String, byte[], byte[]> obj = get(key);
        remove(key);
        return obj;
    }

    /**
     * remove Удаляет заявку из хранилища по заданному ключу
     *
     * @param key
     */
    private static void remove(int key)
    {
        queryStorageDate.remove(key);
        queryStorageCommand.remove(key);
        queryStorageInformation.remove(key);
    }
}
