using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace Patcher
{
    class ConnectWithFastSocket
    {
		private static Socket sender;
		private static bool realization = false;
		private static String ip;
		private static int port;
		public static void createSocket(String ip, int port)
		{
			ConnectWithFastSocket.ip = ip;
			ConnectWithFastSocket.port = port;
			IPHostEntry ipHost = Dns.GetHostEntry(ip);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

			sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			sender.SendTimeout = 4000;
			sender.Connect(ipEndPoint);
			realization = true;
		}

		internal static void closeSocket()
		{
			if (realization)
			{
				sender.Close();
				sender.Dispose();
				realization = false;
			}
		}

		public static string sendMessage(String message, int expectedSize)
		{
			string result;
			try
			{
				if (!realization)
				{
					createSocket(ConnectWithFastSocket.ip, ConnectWithFastSocket.port);
				}


				message += "\0";
				byte[] answerFromServer = new byte[expectedSize + 1];
				if (realization)
				{
					byte[] msg = Encoding.UTF8.GetBytes(message);
					// Отправляем данные через сокет
					int bytesSent = sender.Send(msg);
					// Получаем ответ от сервера
					int bytesRec = sender.Receive(answerFromServer);
				}
				result = System.Text.Encoding.UTF8.GetString(answerFromServer);
			}
			catch (SocketException socket)
			{
				MessageBox.Show("Сервер не доступен", "Сервер не доступен");
				result = "";
				realization = false;
			}
			return result;
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
