package es.irkutskenergo.other;

import java.util.Date;

/**
 *
 * @author admin
 */
public class ExceptionServer extends Exception {

    String message = "";
    Date date = new Date();

    public ExceptionServer(String message)
    {
        this.message = message;
        date = new Date();
    }

    @Override
    public String toString()
    {
        return "Сообщение об ошибке: " + this.message + " Время возникновения: "
                + date.toString();
    }
}
