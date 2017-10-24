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
using System.Windows;using System.Windows.Threading;

namespace contact_center_application.form
{
	/// <summary>
	/// Логика взаимодействия для UploadWindow.xaml
	/// </summary>
	public partial class UploadWindow : Window
	{
		public UploadWindow()
		{
			InitializeComponent();
		}

		private string aliance;
		private string relativeWay;
		private string pathToFileIncludeNameFile;

		public UploadWindow(string aliance, string relativeWay, string pathToFileIncludeNameFile)
		{
			InitializeComponent();

			this.aliance = aliance;
			this.relativeWay = relativeWay;
			this.pathToFileIncludeNameFile = pathToFileIncludeNameFile;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="relativeWay"></param>
		public void sendFileToServer()
		{
			this.Show();
			try
			{
				if (File.Exists(pathToFileIncludeNameFile))
				{
					long expectedSizeFile = (new FileInfo(this.pathToFileIncludeNameFile)).Length;

					setData(10, "Сбор сведений о файле");
					string fileName = Path.GetFileName(pathToFileIncludeNameFile);
					ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
					{
						command = "try_upload",
						param1 = expectedSizeFile.ToString(),
						param2 = aliance,
						param3 = "",
						param4_array = System.Text.Encoding.UTF8.GetBytes(relativeWay),
						param5_array = System.Text.Encoding.UTF8.GetBytes(fileName)
					};
					string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
					string answer = ConnectWithFastSocket.sendMessage(resultJson, 1024);
					ObjectForSerialization objResponseFromFastSocket =
						JsonConvert.DeserializeObject<ObjectForSerialization>(answer);

					if (!objResponseFromFastSocket.command.Equals("upload") && !objResponseFromFastSocket.param2.Equals("1"))
					{
						throw new Exception();
					}
					ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
					{
						command = "try_upload",
						param1 = "",
						param2 = objResponseFromFastSocket.param1
					};


					createSocket(RequestDataFromServer.getAddressServer(), 
						RequestDataFromServer.getFtpPort());
					resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);

					sendMessageGetContentFile(resultJson, expectedSizeFile, pathToFileIncludeNameFile);
					closeSocket();
					Thread.Sleep(1500);
					this.Close();
				}
				else
				{
					throw new Exception();
				}
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
			long expectedSize, string relativeWay)
		{
			message += "\0";
			byte[] answerFromServer = new byte[expectedSize + 1];
			int fixedSize = 50 * 1024;

			if (realization)
			{
				int bytesSent = sender.Send(Encoding.UTF8.GetBytes(message));
				int bytesRec = sender.Receive(answerFromServer);

				if (expectedSize < fixedSize)
				{
					setData(55, "Получение содержимого файла...");
					byte[] msg = new byte[expectedSize + 1];
					msg = File.ReadAllBytes(this.pathToFileIncludeNameFile);
					bytesSent = sender.Send(msg);
					bytesRec = sender.Receive(answerFromServer);
					if (answerFromServer[0] == 49)
					{
						setData(90, "Запись файла...");
					}
					else
					{
						throw new Exception();
					}
				}
				else
				{
					byte[] msg = new byte[expectedSize + 1];
					int getBytes = 0;

					var stream = File.OpenRead(this.pathToFileIncludeNameFile);
					do
					{
						setData((int)(15 + 100 * 0.85 *
							(float)getBytes / expectedSize),
							"Получение содержимого файла...");
						stream.Read(msg, getBytes, fixedSize);
						bytesSent = sender.Send(msg);
						bytesRec = sender.Receive(answerFromServer);
						if (answerFromServer[0] == 49)
						{
							//ошибка?
						}
						getBytes += fixedSize;
					} while (getBytes + fixedSize < expectedSize);

					stream.Read(msg, getBytes, (int)(expectedSize - getBytes));
					bytesSent = sender.Send(msg);
					bytesRec = sender.Receive(answerFromServer);
					if (answerFromServer[0] == 49)
					{
						//ошибка?
					}
					stream.Close();
					stream.Dispose();
				}
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
	}

}
