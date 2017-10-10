using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;

namespace contact_center_application.core
{
    class ConnectWithFtpSocket
    {
		private static Socket sender;
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

		public static void closeSocket()
		{
			sender.Close();
			realization = false;
		}

		public static string sendMessage(String message, int expectedSize)
		{
			string result = System.Text.Encoding.UTF8.GetString(sendMessageGetContentFile(message,
			expectedSize));
			return result;
		}

		public static byte[] sendMessageGetContentFile(String message, 
			int expectedSize)
		{
			message += "\0";
			byte[] answerFromServer = null; 
			byte[] answer = null;
			int fixedSize = 64 * 1024;

			if (realization)
			{
				answerFromServer = new byte[expectedSize + 1];
				if (expectedSize < fixedSize)
				{

					byte[] msg = Encoding.UTF8.GetBytes(message);
					int bytesSent = sender.Send(msg);
					int bytesRec = sender.Receive(answerFromServer);
					answer = getRightArrayByte(answerFromServer);

				}
				else
				{
					answer = new byte[expectedSize];
					byte[] sourceArray = new byte[fixedSize];
					answerFromServer = new byte[fixedSize + 1];
					byte[] msg;
					int bytesSent;
					int bytesRec; 
					int getBytes = 0;
					string toSend = message;

					do
					{
						msg = Encoding.UTF8.GetBytes(toSend);
						bytesSent = sender.Send(msg);
						bytesRec = sender.Receive(answerFromServer);
						sourceArray = getRightArrayByte(answerFromServer);
						getBytes += fixedSize;
						Array.ConstrainedCopy(sourceArray, 0, answer, 
							getBytes - fixedSize, sourceArray.Length);
						toSend = "1";
					} while (getBytes + fixedSize < expectedSize);


					answerFromServer = new byte[expectedSize - getBytes + 1];
					msg = Encoding.UTF8.GetBytes(toSend);
					bytesSent = sender.Send(msg);
					bytesRec = sender.Receive(answerFromServer);
					sourceArray = getRightArrayByte(answerFromServer);
					Array.ConstrainedCopy(sourceArray, 0, answer,
							getBytes, sourceArray.Length);
				}
			}
			return answer;
		}

		public static byte[] getRightArrayByte(byte[] answerFromServer)
		{
			byte[] answer = new byte[answerFromServer.Length - 1];
			for (int i = 0; i < answer.Length; i++)
			{
				answer[i] = answerFromServer[i];
			}
			return answer;
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
