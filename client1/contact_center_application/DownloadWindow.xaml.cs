using contact_center_application.core;
using contact_center_application.serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace contact_center_application
{
	/// <summary>
	/// Логика взаимодействия для DownloadWindow.xaml
	/// </summary>
	public partial class DownloadWindow : Window
	{
		private string aliance;
		private string relativeWay;
		private byte[] contentFile;

		public DownloadWindow()
		{
			InitializeComponent();
		}

		public DownloadWindow(string aliance, string relativeWay)
		{
			InitializeComponent();
			this.aliance = aliance;
			this.relativeWay = relativeWay;
			this.contentFile = null;
		}

		private void setData(int value, string describe)
		{
			progressBar.Value = value;
			info.Text = describe;
			Refresh();
		}

		public byte[] getContentFile()
		{
			this.Show();

			setData(10, "Сбор сведений о файле");
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "get_content_file",
				param1 = aliance,
				param2 = "",
				param3 = "",
				byteArray = System.Text.Encoding.UTF8.GetBytes(relativeWay)
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 1024);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			
			int expectedSize = Int32.Parse(objResponseFromFastSocket.param1);
			int index = Int32.Parse(objResponseFromFastSocket.param2);
			if (!objResponseFromFastSocket.command.Equals("content_file"))
			{
				return new byte[] { };
			}
			ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
			{
				command = "content_file",
				param1 = "",
				param2 = index.ToString()
			};

			string outputSizeFile = "";
			if (expectedSize < 1024)
				outputSizeFile = String.Format(
					"{0} байт", expectedSize);
			else if (expectedSize < 1048576)
				outputSizeFile = String.Format(
					"{0} кб", (expectedSize / 1024.0).ToString(
					"0.00", CultureInfo.InvariantCulture));
			else if (expectedSize < 1073741824)
				outputSizeFile = String.Format(
					"{0} мб", (expectedSize / 1048576.0).ToString(
					"0.00", CultureInfo.InvariantCulture));
			else
				outputSizeFile = String.Format(
					"{0} гб", (expectedSize / 1073741824.0).ToString("0.00", CultureInfo.InvariantCulture));


			string output = "Получение содержимого файла... Общий размер: " + outputSizeFile;
			setData(12, "Подключение к файловому серверу...");
			Thread.Sleep(500);
			createSocket(RequestDataFromServer.getAddressServer(), 6502);
			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			setData(14, output);
			byte[] answerToFunction = sendMessageGetContentFile(resultJson, expectedSize);
			closeSocket();

			
			Thread.Sleep(1500);
			this.Close();
			return answerToFunction;
		}

		private Socket sender;
		private bool realization = false;

		public void createSocket(String ip, int port)
		{
			IPHostEntry ipHost = Dns.GetHostEntry(ip);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

			sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			sender.Connect(ipEndPoint);
			realization = true;
		}

		public void closeSocket()
		{
			sender.Close();
			realization = false;
		}

		public byte[] sendMessageGetContentFile(String message,
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
					setData(55, "Получение содержимого файла...");
					byte[] msg = Encoding.UTF8.GetBytes(message);
					int bytesSent = sender.Send(msg);
					int bytesRec = sender.Receive(answerFromServer);
					answer = ConnectWithFtpSocket.getRightArrayByte(answerFromServer);
					setData(90, "Получение содержимого файла...");
				}
				else
				{
					answer = new byte[expectedSize];
					byte[] sourceArray = new byte[1024 * 50];
					answerFromServer = new byte[1024 * 50 + 1];
					byte[] msg;
					int bytesSent;
					int bytesRec;
					int getBytes = 0;
					string toSend = message;

					do
					{
						setData((int) (15 + 100 * 0.85 * 
							(float)getBytes/expectedSize),
							"Получение содержимого файла...");
						msg = Encoding.UTF8.GetBytes(toSend);
						bytesSent = sender.Send(msg);
						bytesRec = sender.Receive(answerFromServer);
						sourceArray = ConnectWithFtpSocket.getRightArrayByte(answerFromServer);
						getBytes += fixedSize;
						Array.ConstrainedCopy(sourceArray, 0, answer,
							getBytes - fixedSize, sourceArray.Length);
						toSend = "1";
					} while (getBytes + fixedSize < expectedSize);


					answerFromServer = new byte[expectedSize - getBytes + 1];
					msg = Encoding.UTF8.GetBytes(toSend);
					bytesSent = sender.Send(msg);
					bytesRec = sender.Receive(answerFromServer);
					sourceArray = ConnectWithFtpSocket.getRightArrayByte(answerFromServer);
					Array.ConstrainedCopy(sourceArray, 0, answer,
							getBytes, sourceArray.Length);
				}
			}
			return answer;
		}

		public void Refresh()
		{
			this.UpdateLayout();
			this.Dispatcher.Invoke((DispatcherOperationCallback)delegate
			{
				return this;
			}, DispatcherPriority.ApplicationIdle, Visibility.Hidden);
		}
	}
}
