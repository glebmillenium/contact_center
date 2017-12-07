package es.irkutskenergo.server.netty.fast;

import es.irkutskenergo.other.Logging;
import org.jboss.netty.channel.ChannelEvent;
import org.jboss.netty.channel.ChannelHandlerContext;
import org.jboss.netty.channel.ChannelStateEvent;
import org.jboss.netty.channel.ExceptionEvent;
import org.jboss.netty.channel.MessageEvent;
import org.jboss.netty.channel.SimpleChannelUpstreamHandler;

public class FastServerHandler extends SimpleChannelUpstreamHandler {

    SenderSmallData senderSmallData;

    public FastServerHandler()
    {

    }

    @Override
    public void handleUpstream(ChannelHandlerContext ctx, ChannelEvent e) throws Exception
    {
        super.handleUpstream(ctx, e);
    }

    @Override
    public void channelConnected(ChannelHandlerContext ctx, ChannelStateEvent e) throws Exception
    {
        Logging.log("Клиент присоединился к серверу: "
                + ctx.getChannel().getRemoteAddress() + " (" + ctx.getChannel()
                .getId() + ")", 1);
        senderSmallData = new SenderSmallData(e.getChannel());
    }

    @Override
    public void channelClosed(ChannelHandlerContext ctx, ChannelStateEvent e) throws Exception
    {
        Logging.log("Соединение с сервером было закрыто " + ctx.getChannel().getRemoteAddress()
                + " (" + ctx.getChannel().getId() + ")", 1);
        this.senderSmallData = null;
        ctx.getChannel().close();
        ctx.getChannel().disconnect();
        System.gc();
    }

    @Override
    public void messageReceived(ChannelHandlerContext ctx, MessageEvent e)
    {
        try
        {
            Logging.log("Получено сообщение от " + ctx.getChannel().getRemoteAddress()
                    + " (" + ctx.getChannel().getId() + ")", 1);

            System.out.println("Thread: Main FAST");
            Logging.log("Thread: Main FAST", 4);
            senderSmallData.process(e.getMessage().toString());
        } catch (Exception ex)
        {
            Logging.log("Критическая ошибка в главном потоке Netty: "
                    + ex.getMessage(), 1);
        }
    }

    @Override
    public void channelDisconnected(ChannelHandlerContext ctx, ChannelStateEvent e) throws Exception
    {
        channelClosed(ctx, e);
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, ExceptionEvent e)
    {
        Logging.log("Ошибка при передаче данных ("
                + ctx.getChannel().getRemoteAddress() + "): "
                + e.getCause() + " (" + ctx.getChannel().getId() + ")", 1);
        e.getChannel().close();
    }
}
