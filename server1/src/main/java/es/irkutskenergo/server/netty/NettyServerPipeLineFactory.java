/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.server.netty;

import static org.jboss.netty.channel.Channels.*;

import org.jboss.netty.channel.ChannelPipeline;
import org.jboss.netty.channel.ChannelPipelineFactory;
import org.jboss.netty.handler.codec.frame.DelimiterBasedFrameDecoder;
import org.jboss.netty.handler.codec.frame.Delimiters;
import org.jboss.netty.handler.codec.string.StringDecoder;
import org.jboss.netty.handler.codec.string.StringEncoder;

public class NettyServerPipeLineFactory implements ChannelPipelineFactory {

    public ChannelPipeline getPipeline() throws Exception {
// Create a default pipeline implementation.
        ChannelPipeline pipeline = pipeline();

// Здесь указываем размер буфера (8192 байта) и символ-признак конца пакета.
//Свои пакеты мы обычно терминируем символом с кодом 0, что соответствует nulDelimiter() в терминологии нетти
        pipeline.addLast("framer", new DelimiterBasedFrameDecoder(8192, Delimiters.nulDelimiter()));
        pipeline.addLast("decoder", new StringDecoder()); //Стандартный строковый декодер. Когда пакеты идут в текстовом, XML, JSON или подобном виде. Для бинарных пакетов - другие кодеки.
        pipeline.addLast("encoder", new StringEncoder());

        pipeline.addLast("handler", new NettyServerHandler()); //И наконец, указываем, какой класс у нас будет уведомляться о входящих соединениях и пришедших данных

        return pipeline;
    }
}
