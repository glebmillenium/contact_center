/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.server.netty.fast;

//import org.jboss.netty.channel.ChannelHandlerContext;
//import static org.jboss.netty.channel.Channels.*;
//
//import org.jboss.netty.channel.ChannelPipeline;
//import org.jboss.netty.channel.ChannelPipelineFactory;
//import org.jboss.netty.channel.ChannelStateEvent;
//import org.jboss.netty.handler.codec.frame.DelimiterBasedFrameDecoder;
//import org.jboss.netty.handler.codec.frame.Delimiters;
//import org.jboss.netty.handler.codec.string.StringDecoder;
//import org.jboss.netty.handler.codec.string.StringEncoder;

//public class FastServerPipeLineFactory implements ChannelPipelineFactory {

//    @Override
//    public ChannelPipeline getPipeline() throws Exception
//    {
//        ChannelPipeline pipeline = pipeline();
//        //pipeline.addLast("framer", new DelimiterBasedFrameDecoder(8192, Delimiters.nulDelimiter()));
//        pipeline.addLast("decoder", new StringDecoder());
//        pipeline.addLast("encoder", new StringEncoder());
//        pipeline.addLast("handler", new FastServerHandler());
//        return pipeline;
//    }
//}
