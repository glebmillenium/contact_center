package es.irkutskenergo.server.netty.fast;

import es.irkutskenergo.other.Logging;
//import org.jboss.netty.channel.ChannelEvent;
//import org.jboss.netty.channel.ChannelHandlerContext;
//import org.jboss.netty.channel.ChannelStateEvent;
//import org.jboss.netty.channel.ExceptionEvent;
//import org.jboss.netty.channel.MessageEvent;
//import org.jboss.netty.channel.SimpleChannelUpstreamHandler;

import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import java.nio.charset.Charset;

public class FastServerHandler extends ChannelInboundHandlerAdapter {

    SenderSmallData senderSmallData;

    public FastServerHandler()
    {

    }

    @Override
    public void channelRegistered(ChannelHandlerContext ctx) throws Exception
    {
        Logging.log("Клиент присоединился к серверу: "
                + ctx.channel().remoteAddress().toString() + " (" + ctx.channel()
                .id() + ")", 1);
    }

    @Override
    public void channelRead(ChannelHandlerContext ctx, Object msg)
    {
        // (2)
        // Discard the received data silently.
        SenderSmallData senderSmallData = new SenderSmallData(ctx);
        try
        {
            Logging.log("Получено сообщение от " + ctx.channel().remoteAddress()
                    + " (" + ctx.channel().id() + ")", 1);

            System.out.println("Thread: Main FAST");
            Logging.log("Thread: Main FAST", 4);
            senderSmallData.process(((ByteBuf) msg).toString(Charset.forName("UTF-8")));
        } catch (Exception ex)
        {
            Logging.log("Критическая ошибка в главном потоке Netty: "
                    + ex.getMessage(), 1);
        }
        //((ByteBuf) msg).release(); // (3)
    }


    @Override
    public void channelUnregistered(ChannelHandlerContext ctx) throws Exception
    {
        Logging.log("Соединение с сервером было закрыто " + ctx.channel().remoteAddress()
                + " (" + ctx.channel().id() + ")", 1);
        this.senderSmallData = null;
        ctx.channel().disconnect();
        ctx.channel().close();
        System.gc();
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause)
    {
        Logging.log("Ошибка при передаче данных ("
                + ctx.channel().remoteAddress() + "): "
                + cause.getMessage() + " (" + ctx.channel().id() + ")", 1);
        ctx.close();
    }
}
