/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.server.netty;

import es.irkutskenergo.core.CommandProcessorSmall;
import org.jboss.netty.channel.ChannelEvent;
import org.jboss.netty.channel.ChannelHandlerContext;
import org.jboss.netty.channel.ChannelStateEvent;
import org.jboss.netty.channel.ExceptionEvent;
import org.jboss.netty.channel.MessageEvent;
import org.jboss.netty.channel.SimpleChannelUpstreamHandler;

public class NettyServerHandler extends SimpleChannelUpstreamHandler {

    public NettyServerHandler()
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
        log("Client connected from " + ctx.getChannel().getRemoteAddress() + " (" + ctx.getChannel().getId() + ")");
    }

    @Override
    public void channelClosed(ChannelHandlerContext ctx, ChannelStateEvent e) throws Exception
    {
        log("Connection closed from " + ctx.getChannel().getRemoteAddress() + " (" + ctx.getChannel().getId() + ")");
    }

    @Override
    public void messageReceived(ChannelHandlerContext ctx, MessageEvent e)
    {
        System.out.println("Пришло сообщение!");
        try
        {
            new CommandProcessorSmall(e.getChannel(), e.getMessage().toString())
                    .start();
            System.out.println(e.getRemoteAddress());
        } catch (Exception ex)
        {
            ex.printStackTrace();
        }
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, ExceptionEvent e)
    {
        log("Error (" + ctx.getChannel().getRemoteAddress() + "): "
                + e.getCause() + " (" + ctx.getChannel().getId() + ")");
        e.getChannel().close();
    }

    private void log(String txt)
    {
        System.out.print("NettyServerHandler: " + txt + "\n");
    }
}
