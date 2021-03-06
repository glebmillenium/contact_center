﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace callback_fast_server
{
    class ConnectWithFastSocket
    {
		private static Socket sender;
		private static bool realization = false;
		public static void createSocket(String ip, int port)
		{
			IPHostEntry ipHost = Dns.GetHostEntry(ip);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

			sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			//sender.SendTimeout = 10000;
			sender.Connect(ipEndPoint);
			realization = true;
		}

		public static string sendMessage(String message, int expectedSize)
		{
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
			return System.Text.Encoding.UTF8.GetString(answerFromServer);
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
