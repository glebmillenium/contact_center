/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.server.netty.data;

import es.irkutskenergo.serialization.ObjectForSerialization;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import org.jboss.netty.channel.ChannelEvent;
import org.jboss.netty.channel.ChannelHandlerContext;
import org.jboss.netty.channel.ChannelStateEvent;
import org.jboss.netty.channel.ExceptionEvent;
import org.jboss.netty.channel.MessageEvent;
import org.jboss.netty.channel.SimpleChannelUpstreamHandler;
import org.codehaus.jackson.map.ObjectMapper;

public class DataServerHandler extends SimpleChannelUpstreamHandler {

    private static Map<String, byte[]> storage = new HashMap<String, byte[]>();
    private static ObjectMapper mapper = new ObjectMapper();
    
    public static boolean addHashKeyIdentificator(String key, byte[] message)
    {
        if (storage.containsKey(key))
        {
            return false;
        } else
        {
            storage.put(key, message);
            return true;
        }
    }

    public DataServerHandler()
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
        try
        {
            String message = e.getMessage().toString();
            String key = getObjectFromJson(message).param2;
            if (storage.containsKey(key))
            {
                new SenderBigData(e.getChannel(),
                        storage.get(key)).start();
                //storage.remove(key);
            } else
            {
                new SenderBigData(e.getChannel(),
                        storage.get(key), false).start();
            }

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

    private ObjectForSerialization getObjectFromJson(String jsonInString)
            throws IOException
    {

        return mapper.readValue(jsonInString,
                ObjectForSerialization.class);
    }
}
