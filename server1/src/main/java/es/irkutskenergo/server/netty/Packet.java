/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.server.netty;

import java.io.IOException;
import org.jboss.netty.buffer.ChannelBuffer;

/**
 *
 * @author admin
 */
public abstract class Packet {

    public static Packet read(ChannelBuffer buffer) throws IOException
    {
        int id = buffer.readUnsignedShort(); // Получаем ID пришедшего пакета, чтобы определить, каким классом его читать
        Packet packet = getPacket(id); // Получаем инстанс пакета с этим ID
        if (packet == null)
        {
            throw new IOException("Bad packet ID: " + id); // Если произошла ошибка и такого пакета не может быть, генерируем исключение
        }
        packet.get(buffer); // Читаем в пакет данные из буфера
        return packet;
    }

    public static Packet write(Packet packet, ChannelBuffer buffer)
    {
        buffer.writeChar(packet.getId()); // Отправляем ID пакета
        packet.send(buffer); // Отправляем данные пакета
        return packet;
    }

    private static Packet getPacket(int id)
    {
        throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }

    // Функции, которые должен реализовать каждый класс пакета
    public abstract void get(ChannelBuffer buffer);

    public abstract void send(ChannelBuffer buffer);

    private int getId()
    {
        throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }
}
