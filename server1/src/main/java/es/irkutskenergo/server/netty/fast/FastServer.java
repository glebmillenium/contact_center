package es.irkutskenergo.server.netty.fast;

import es.irkutskenergo.other.Logging;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.TimeUnit;
import org.jboss.netty.bootstrap.ServerBootstrap;
import org.jboss.netty.channel.Channel;
import org.jboss.netty.channel.socket.nio.NioServerSocketChannelFactory;
import org.jboss.netty.handler.execution.OrderedMemoryAwareThreadPoolExecutor;

/**
 * FastServer - предназначен для получения первичных данных от клиента на
 * выполнение запроса, и установки заявки в ftp сервере для полу
 *
 * @author admin
 */
public class FastServer {

    private static Channel channel;

    /**
     * FastServer(port) - конструктор, создает сокет на сервере по указанному
     * логическому порту
     *
     * @param port - порт для открытия сокета
     * @throws Exception
     */
    public FastServer(int port) throws Exception
    {
        ExecutorService bossExec = new OrderedMemoryAwareThreadPoolExecutor(1,
                (long) 400000000, 2000000000, 60, TimeUnit.SECONDS);
        ExecutorService ioExec = new OrderedMemoryAwareThreadPoolExecutor(4,
                (long) 400000000, 2000000000, 60, TimeUnit.SECONDS);
        ServerBootstrap bootstrap = new ServerBootstrap(
                new NioServerSocketChannelFactory(bossExec, ioExec, 4));
        bootstrap.setOption("backlog", 500);
        bootstrap.setOption("connectTimeoutMillis", 10000);
        bootstrap.setPipelineFactory(new FastServerPipeLineFactory());
        FastServer.channel = bootstrap.bind(new InetSocketAddress(port));

        /**
         * ExecutorService bossExec = new
         * OrderedMemoryAwareThreadPoolExecutor(1, (long) 400000000, 2000000000,
         * 60, TimeUnit.SECONDS); ExecutorService ioExec = new
         * OrderedMemoryAwareThreadPoolExecutor(4, (long) 400000000, 2000000000,
         * 60, TimeUnit.SECONDS); ServerBootstrap networkServer = new
         * ServerBootstrap(new NioServerSocketChannelFactory(bossExec, ioExec,
         * 4)); networkServer.setOption("backlog", 500);
         * networkServer.setOption("connectTimeoutMillis", 10000);
         * networkServer.setPipelineFactory(new ServerPipelineFactory());
         * Channel channel = networkServer.bind(new InetSocketAddress(port));
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
         */
        Logging.log("Сервер быстрого обмена запущен по адресу -  "
                + InetAddress.getLocalHost().toString() + ":" + port + "\n", 1);
    }
}
