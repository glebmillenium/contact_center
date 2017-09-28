using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace contact_center_application.core
{
    class ConnectWithRemoteSocket
    {
		public static Socket sender;
		private static bool realization = false;
		public static void createSocket(String ip, int port)
		{
			IPHostEntry ipHost = Dns.GetHostEntry(ip);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

			sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			sender.Connect(ipEndPoint);
			realization = true;
		}

		public static void sendMessage(String message)
		{
			byte[] answerFromServer = new byte[1024];
			if (realization)
			{
				byte[] msg = Encoding.UTF8.GetBytes(message);
				// Отправляем данные через сокет
				int bytesSent = sender.Send(msg);
				Console.WriteLine("Wait...");
				// Получаем ответ от сервера
				int bytesRec = sender.Receive(answerFromServer);
				Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(answerFromServer, 0, bytesRec));
			}
		}

		public static void releaseSocket()
		{
			if (realization)
			{
				sender.Shutdown(SocketShutdown.Both);
				sender.Close();
			}
		}
	}
}
