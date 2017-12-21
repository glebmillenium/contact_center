package es.irkutskenergo.server.netty.fast;

import es.irkutskenergo.other.Logging;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.TimeUnit;
import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.Channel;

import io.netty.bootstrap.ServerBootstrap;

import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelOption;
import io.netty.channel.ChannelPipeline;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import io.netty.handler.codec.DelimiterBasedFrameDecoder;
import io.netty.handler.codec.Delimiters;
import io.netty.handler.timeout.ReadTimeoutHandler;
import io.netty.util.concurrent.DefaultEventExecutorGroup;
import io.netty.util.concurrent.EventExecutorGroup;

/**
 * FastServer - предназначен для получения первичных данных от клиента на
 * выполнение запроса, и установки заявки в ftp сервере для полу
 *
 * @author admin
 */
public class FastServer {

    private int port;

    /**
     * FastServer(port) - конструктор, создает сокет на сервере по указанному
     * логическому порту
     *
     * @param port - порт для открытия сокета
     * @throws Exception
     */
    public FastServer(int port) throws Exception
    {
        this.port = port;
    }

    public void run() throws Exception
    {
        /*NioEventLoopGroup boosGroup = new NioEventLoopGroup();
        NioEventLoopGroup workerGroup = new NioEventLoopGroup();
        ServerBootstrap bootstrap = new ServerBootstrap();
        bootstrap.group(boosGroup, workerGroup);
        bootstrap.channel(NioServerSocketChannel.class);
        EventExecutorGroup group = new DefaultEventExecutorGroup(1500);
        EventLoopGroup group = new NioEventLoopGroup();
        bootstrap.childHandler(new ChannelInitializer<SocketChannel>() {
            @Override
            protected void initChannel(SocketChannel ch) throws Exception
            {
                ChannelPipeline pipeline = ch.pipeline();
                pipeline.addLast("frameDecoder", new DelimiterBasedFrameDecoder(
                        8096, Delimiters.nulDelimiter()));
                pipeline.addLast(group, "serverHandler", new FastServerHandler());
                //pipeline.addLast("readTimeoutHandler", 
                //        new ReadTimeoutHandler(5000));
            }
        });
        bootstrap.option(ChannelOption.TCP_NODELAY, true);
        bootstrap.childOption(ChannelOption.SO_KEEPALIVE, true);
        bootstrap.bind(this.port).sync();*/

        NioEventLoopGroup boosGroup = new NioEventLoopGroup(1);
        NioEventLoopGroup workerGroup = new NioEventLoopGroup(50);
        ServerBootstrap bootstrap = new ServerBootstrap();
        bootstrap.group(boosGroup, workerGroup);
        bootstrap.channel(NioServerSocketChannel.class);

        //bootstrap.localAddress(this.port);

        bootstrap.childHandler(new ChannelInitializer<SocketChannel>() {
            protected void initChannel(SocketChannel socketChannel) throws Exception
            {
                socketChannel.pipeline().addLast("frameDecoder", new DelimiterBasedFrameDecoder(
                        8096, Delimiters.nulDelimiter()));
                socketChannel.pipeline().addLast(new FastServerHandler());
            }
        });
        ChannelFuture channelFuture = bootstrap
                .bind(new InetSocketAddress(port)).sync();
        channelFuture.channel().closeFuture().sync();
    }

//    ExecutorService bossExec = new OrderedMemoryAwareThreadPoolExecutor(1,
//            (long) 400000000, 2000000000, 60, TimeUnit.SECONDS);
//    ExecutorService ioExec = new OrderedMemoryAwareThreadPoolExecutor(4,
//            (long) 400000000, 2000000000, 60, TimeUnit.SECONDS);
//    ServerBootstrap bootstrap = new ServerBootstrap(
//            new NioServerSocketChannelFactory(bossExec, ioExec, 4));
//
//    bootstrap.setOption (
//
//    "backlog", 500);
//    bootstrap.setOption (
//
//    "connectTimeoutMillis", 10000);
//    bootstrap.setPipelineFactory (
//    new FastServerPipeLineFactory());
//        FastServer.channel  = bootstrap.bind(new InetSocketAddress(port));
    /**
     * ExecutorService bossExec = new OrderedMemoryAwareThreadPoolExecutor(1,
     * (long) 400000000, 2000000000, 60, TimeUnit.SECONDS); ExecutorService
     * ioExec = new OrderedMemoryAwareThreadPoolExecutor(4, (long) 400000000,
     * 2000000000, 60, TimeUnit.SECONDS); ServerBootstrap networkServer = new
     * ServerBootstrap(new NioServerSocketChannelFactory(bossExec, ioExec, 4));
     * networkServer.setOption("backlog", 500);
     * networkServer.setOption("connectTimeoutMillis", 10000);
     * networkServer.setPipelineFactory(new ServerPipelineFactory()); Channel
     * channel = networkServer.bind(new InetSocketAddress(port));
     */
    /**
     * ChannelFactory factory = new NioServerSocketChannelFactory(
     * Executors.newCachedThreadPool(), Executors.newCachedThreadPool());
     *
     * ServerBootstrap bootstrap = new ServerBootstrap(factory);
     * bootstrap.setPipelineFactory(new FastServerPipeLineFactory());
     *
     * bootstrap.setOption("sendBufferSize", 8 * 1024L);
     * bootstrap.setOption("receiveBufferSize", 8 * 1024L);*
     *
     * Logging.log (
     *
     *
     * "Сервер быстрого обмена запущен по адресу - " +
     * InetAddress.getLocalHost().toString() + ":" + port + "\n", 1); }
     *
     */
}
