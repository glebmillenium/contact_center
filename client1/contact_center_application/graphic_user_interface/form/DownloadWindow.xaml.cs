using contact_center_application.core;
using contact_center_application.serialization;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace contact_center_application.graphic_user_interface.form
{
	/// <summary>
	/// Логика взаимодействия для DownloadWindow.xaml
	/// </summary>
	public partial class DownloadWindow : Window
	{
		private string aliance;
		private string relativeWay;

		public DownloadWindow()
		{
			InitializeComponent();
		}

		public DownloadWindow(string aliance, string relativeWay)
		{
			InitializeComponent();
			this.aliance = aliance;
			this.relativeWay = relativeWay;
		}

		public void getContentFileAndWriteToFile(string relativeWay)
		{
			this.Show();
			try
			{
				string directoryWay = System.IO.Path.GetDirectoryName(relativeWay);
				Directory.CreateDirectory(directoryWay);
				if (File.Exists(relativeWay))
				{
					File.Delete(relativeWay);
				}

				Tuple<int, int> result = collectInformationAboutFile();

				int expectedSize = result.Item2;
				int index = result.Item1;

				string outputSizeFile = getHumanSize(expectedSize);

				interactionWithFtpSocket(index, expectedSize, relativeWay);
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				this.Close();
			}
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

		public void sendMessageGetContentFile(String message,
			int expectedSize, string relativeWay)
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
					FileStream stream = new FileStream(relativeWay, FileMode.Append);
					setData(55, "Получение содержимого файла...");
					byte[] msg = Encoding.UTF8.GetBytes(message);
					int bytesSent = sender.Send(msg);
					int bytesRec = sender.Receive(answerFromServer);
					answer = ConnectWithFtpSocket.getRightArrayByte(answerFromServer);
					setData(90, "Запись файла...");
					//write answer to file
					stream.Write(answer, 0, answer.Length);
					stream.Close();
					stream.Dispose();
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
					FileStream stream = new FileStream(relativeWay, FileMode.Append);

					do
					{						
						setData((int) (15 + 100 * 0.85 * 
							(float)getBytes/expectedSize),
							"Получение содержимого файла...");
						msg = Encoding.UTF8.GetBytes(toSend);
						bytesSent = sender.Send(msg);
						bytesRec = sender.Receive(answerFromServer);
						sourceArray = ConnectWithFtpSocket.getRightArrayByte(answerFromServer);

						//sourceArray to write
						stream.Write(sourceArray, 0, sourceArray.Length);
						getBytes += fixedSize;
						//Array.ConstrainedCopy(sourceArray, 0, answer,
						//	getBytes - fixedSize, sourceArray.Length);
						toSend = "1";
					} while (getBytes + fixedSize < expectedSize);


					answerFromServer = new byte[expectedSize - getBytes + 1];
					msg = Encoding.UTF8.GetBytes(toSend);
					bytesSent = sender.Send(msg);
					bytesRec = sender.Receive(answerFromServer);
					sourceArray = ConnectWithFtpSocket.getRightArrayByte(answerFromServer);

					stream.Write(sourceArray, 0, sourceArray.Length);
					stream.Close();
					stream.Dispose();
					//Array.ConstrainedCopy(sourceArray, 0, answer,
					//		getBytes, sourceArray.Length);
				}
				setData(99, "Загрузка файла из удаленного файлового ресурса прошла успешно!");

			}
		}

		public void Refresh()
		{
			this.UpdateLayout();
			this.Dispatcher.Invoke((DispatcherOperationCallback)delegate
			{
				return this;
			}, DispatcherPriority.ApplicationIdle, Visibility.Hidden);
		}


		private void setData(int value, string describe)
		{
			progressBar.Value = value;
			info.Text = describe;
			Refresh();
		}

		private string getHumanSize(int expectedSize)
		{
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
			return outputSizeFile;
		}

		private Tuple<int, int> collectInformationAboutFile()
		{
			setData(10, "Сбор сведений о файле");
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "get_content_file",
				param1 = aliance,
				param2 = "",
				param3 = "",
				param4_array = System.Text.Encoding.UTF8.GetBytes(relativeWay)
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 1024);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			int expectedSize = Int32.Parse(objResponseFromFastSocket.param1);
			int index = Int32.Parse(objResponseFromFastSocket.param2);
			if (!objResponseFromFastSocket.command.Equals("content_file"))
			{
				throw new Exception();
			}
			return new Tuple<int, int>(index, expectedSize);
		}

		private void interactionWithFtpSocket(int index, int expectedSize, string relativeWay)
		{

			createSocket(RequestDataFromServer.getAddressServer(), RequestDataFromServer.getFtpPort());
			ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
			{
				command = "content_file",
				param1 = "",
				param2 = index.ToString()
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);

			sendMessageGetContentFile(resultJson, expectedSize, relativeWay);
			closeSocket();
			this.Close();
		}
	}
}
