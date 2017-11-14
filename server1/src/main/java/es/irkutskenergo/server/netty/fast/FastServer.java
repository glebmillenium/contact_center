package es.irkutskenergo.server.netty.fast;

import es.irkutskenergo.other.Logging;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.util.concurrent.Executors;
import org.jboss.netty.bootstrap.ServerBootstrap;
import org.jboss.netty.channel.ChannelFactory;
import org.jboss.netty.channel.socket.nio.NioServerSocketChannelFactory;

/**
 * FastServer - предназначен для получения первичных данных от клиента на
 * выполнение запроса, и установки заявки в ftp сервере для полу
 *
 * @author admin
 */
public class FastServer {

    /**
     * FastServer(port) - конструктор, создает сокет на сервере по указанному
     * логическому порту
     *
     * @param port - порт для открытия сокета
     * @throws Exception
     */
    public FastServer(int port) throws Exception
    {        
        ChannelFactory factory = new NioServerSocketChannelFactory(
                Executors.newCachedThreadPool(),
                Executors.newCachedThreadPool());

        ServerBootstrap bootstrap = new ServerBootstrap(factory);
        bootstrap.setPipelineFactory(new FastServerPipeLineFactory());

        bootstrap.setOption("sendBufferSize", 8 * 1024L);
        bootstrap.setOption("receiveBufferSize", 8 * 1024L);
        bootstrap.bind(new InetSocketAddress(port));
        Logging.log("Сервер быстрого обмена запущен по адресу -  "
                + InetAddress.getLocalHost().toString() + ":" + port + "\n", 1);
    }
}
