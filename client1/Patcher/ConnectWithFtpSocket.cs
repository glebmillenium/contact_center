using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;
using System.IO;

namespace Patcher
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
			sender.SendTimeout = 4000;
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
			int fixedSize = 50 * 1024;

			if (realization)
			{
				answerFromServer = new byte[expectedSize + 1];
				if (expectedSize < fixedSize)
				{
					//TreatmenterExchangeFileWithServer.setData(50, "Начало загрузки");
					byte[] msg = Encoding.UTF8.GetBytes(message);
					int bytesSent = sender.Send(msg);
					int bytesRec = sender.Receive(answerFromServer);
					answer = getRightArrayByte(answerFromServer);
				}
				else
				{
					//TreatmenterExchangeFileWithServer.setData(8, "Начало загрузки");
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
						double track = 8.0 + (95.0 - 8.0) * ((double) getBytes / expectedSize);
						//TreatmenterExchangeFileWithServer.setData((int) track, "Загрузка");
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

		public static bool sendFile(string resultJson, string pathToUploadFile, long expectedSize)
		{
			resultJson += "\0";
			byte[] answerFromServer = null;
			int fixedSize = 50 * 1024;
			if (realization)
			{
				FileStream f = new FileStream(pathToUploadFile, FileMode.Open, FileAccess.Read, 
					FileShare.Read);
				BinaryReader sr = new BinaryReader(f);
				int currentBytes = 0;
				byte[] input;
				int bytesSent;
				int bytesRec;
				if (expectedSize > fixedSize)
				{
					while (currentBytes + fixedSize < expectedSize)
					{
						input = sr.ReadBytes(fixedSize);
						bytesSent = sender.Send(input);
						bytesRec = sender.Receive(answerFromServer);
						if (answerFromServer[0] != 49)
						{
							return false;
						}
					}
					input = sr.ReadBytes((int)(expectedSize - (long)currentBytes));
					bytesSent = sender.Send(input);
					bytesRec = sender.Receive(answerFromServer);
					sr.Close();
				}
				else
				{
					input = sr.ReadBytes((int)expectedSize);
					bytesSent = sender.Send(input);
					bytesRec = sender.Receive(answerFromServer);
					if (answerFromServer[0] != 49)
					{
						return false;
					}
					sr.Close();
				}
			}
			return true;
		}
	}
}
