package es.irkutskenergo.server.netty;

import es.irkutskenergo.other.Logging;
import org.jboss.netty.channel.ChannelHandlerContext;
import org.jboss.netty.channel.ChannelStateEvent;
import org.jboss.netty.channel.ExceptionEvent;
import org.jboss.netty.channel.MessageEvent;
import org.jboss.netty.channel.SimpleChannelUpstreamHandler;

/**
 *
 * @author admin
 */
public class PlayerHandler extends SimpleChannelUpstreamHandler {

    private PlayerWorkerThread worker;

    PlayerHandler(PacketFrameDecoder decoder, PacketFrameEncoder encoder)
    {
        throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }

    @Override
    public void channelConnected(ChannelHandlerContext ctx, ChannelStateEvent e) throws Exception
    {
        // Событие вызывается при подключении клиента. Я создаю здесь Worker игрока — объект, который занимается обработкой данных игрока непостредственно.
        // Я передаю ему канал игрока (функция e.getChannel()), чтобы он мог в него посылать пакеты
        worker = new PlayerWorkerThread(this, e.getChannel());
    }

    @Override
    public void channelDisconnected(ChannelHandlerContext ctx, ChannelStateEvent e) throws Exception
    {
        // Событие закрытия канала. Используется в основном, чтобы освободить ресурсы, или выполнить другие действия, которые происходят при отключении пользователя. Если его не обработать, Вы можете и не заметить, что пользователь отключился, если он напрямую не сказал этого серверу, а просто оборвался канал.
        worker.disconnectedFromChannel();
    }

    @Override
    public void messageReceived(ChannelHandlerContext ctx, MessageEvent e)
    {
        // Функция принимает уже готовые Packet'ы от игрока, поэтому их можно сразу посылать в worker. За их формирование отвечает другой обработчик.
        if (e.getChannel().isOpen())
        {
            worker.acceptPacket((Packet) e.getMessage());
        }
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, ExceptionEvent e)
    {
        // На канале произошло исключение. Выводим ошибку, закрываем канал.
        Logging.log("Exception from downstream", 0);
        ctx.getChannel().close();
    }
}
