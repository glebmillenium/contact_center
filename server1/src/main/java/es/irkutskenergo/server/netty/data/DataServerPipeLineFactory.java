/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.server.netty.data;

import org.jboss.netty.buffer.ChannelBuffer;
import org.jboss.netty.buffer.ChannelBuffers;
import org.jboss.netty.channel.Channel;
import org.jboss.netty.channel.ChannelHandlerContext;
import static org.jboss.netty.channel.Channels.*;

import org.jboss.netty.channel.ChannelPipeline;
import org.jboss.netty.channel.ChannelPipelineFactory;
import org.jboss.netty.handler.codec.frame.DelimiterBasedFrameDecoder;
import org.jboss.netty.handler.codec.frame.Delimiters;
import org.jboss.netty.handler.codec.oneone.OneToOneEncoder;
import org.jboss.netty.handler.codec.string.StringDecoder;
import org.jboss.netty.handler.codec.string.StringEncoder;

public class DataServerPipeLineFactory implements ChannelPipelineFactory {

    public ChannelPipeline getPipeline() throws Exception {
        ChannelPipeline pipeline = pipeline();

// Здесь указываем размер буфера (8192 байта) и символ-признак конца пакета.
//Свои пакеты мы обычно терминируем символом с кодом 0, что соответствует nulDelimiter() в терминологии нетти
        pipeline.addLast("framer", new DelimiterBasedFrameDecoder(8192, Delimiters.nulDelimiter()));
        pipeline.addLast("decoder", new StringDecoder()); //Стандартный строковый декодер. Когда пакеты идут в текстовом, XML, JSON или подобном виде. Для бинарных пакетов - другие кодеки.
        pipeline.addLast("encoder", new OneToOneEncoder() {
            @Override
            protected Object encode(ChannelHandlerContext channelHandlerContext, Channel channel, Object o) throws Exception {

                if (!(o instanceof byte[])) {
                    return o;
                }

                ChannelBuffer buffer = ChannelBuffers.dynamicBuffer();
                //buffer.writeInt(((byte[]) o).length);
                buffer.writeBytes((byte[]) o);
                return buffer;
            }
        });

        pipeline.addLast("handler", new DataServerHandler()); //И наконец, указываем, какой класс у нас будет уведомляться о входящих соединениях и пришедших данных

        return pipeline;
    }
}
