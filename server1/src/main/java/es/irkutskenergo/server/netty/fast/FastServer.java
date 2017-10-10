package es.irkutskenergo.server.netty.fast;

import es.irkutskenergo.other.Logging;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.util.concurrent.Executors;
import org.jboss.netty.bootstrap.ServerBootstrap;
import org.jboss.netty.channel.ChannelFactory;
import org.jboss.netty.channel.socket.nio.NioServerSocketChannelFactory;

/**
 *
 * @author admin
 */
public class FastServer {

    public FastServer(int port) throws Exception
    {
        ChannelFactory factory = new NioServerSocketChannelFactory(
                Executors.newCachedThreadPool(),
                Executors.newCachedThreadPool());

        ServerBootstrap bootstrap = new ServerBootstrap(factory);
        bootstrap.setPipelineFactory(new FastServerPipeLineFactory());

        bootstrap.setOption("child.tcpNoDelay", true);
        bootstrap.setOption("child.keepAlive", true);
        bootstrap.bind(new InetSocketAddress(port));

        Logging.log("Сервер быстрого обмена запущен по адресу -  "
                + InetAddress.getLocalHost().toString() + ":" + port + "\n", 1);
    }
}
