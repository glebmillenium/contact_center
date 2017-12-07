using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace callback_ftp_server
{
	class Program
	{
		static void Main(string[] args)
		{
			string PathFolderBack = Path.GetDirectoryName(Directory.GetCurrentDirectory());
			SettingsData.setDirectorySettings(PathFolderBack + @"/settings/ip_connect",
				PathFolderBack + @"/settings/port_ftp", PathFolderBack + @"/settings/port_fast", 
				PathFolderBack + @"/settings/version",
				PathFolderBack + @"/settings/update_file_system");
			Logger logger = new Logger(PathFolderBack + "/log/callback_ftp_server_log.txt",
				PathFolderBack + "/log/callback_ftp_server_exception.txt", PathFolderBack + "/tmp");
			string address = SettingsData.getAddress();
			int portFtp = SettingsData.getFtpPort();
			ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
			{
				command = "test_ftp_socket",
				param1 = "",
				param2 = "-1"
			};

			string resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			float[] array = new float[20];
			bool error = false;
			string output;
			Console.WriteLine("Начало тестирование сокетов удаленного сервера...");
			for (int i = 0; i < array.Length; i++)
			{
				ConnectWithFtpSocket.createSocket(address, portFtp);
				resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
				DateTime start = DateTime.Now;
				string answerSocket = ConnectWithFtpSocket.sendMessage(resultJson, 1024);
				ConnectWithFtpSocket.closeSocket();
				TimeSpan timeExecute = DateTime.Now - start;
				ObjectForSerialization objResponseFromFtpSocket =
					JsonConvert.DeserializeObject<ObjectForSerialization>(answerSocket);
				if (objResponseFromFtpSocket != null
						|| objResponseFromFtpSocket.command.Equals("answer_on_test_ftp_socket")
						&& objResponseFromFtpSocket.param1.Equals("true"))
				{
					output = "Сокет передачи данных по ftp_server - открыт. Время выполнения: "
						+ timeExecute.Milliseconds.ToString() + " миллисекунд";
					Logger.log(output);
					array[i] = timeExecute.Milliseconds;
				}
				else
				{
					output = "Сокет передачи данных по ftp_server - закрыт";
					Logger.log(output);
					error = true;
				}
				Console.WriteLine(output);
				Thread.Sleep(200);
			}
			if (error)
			{
				output = "Во время тестирования были ошибки";
				Logger.log(output);
			}
			else
			{
				output = "Во время тестирования не было обнаружено ошибок. Среднее время отклика: " + array.Average();
				Logger.log(output);
			}
			Console.WriteLine(output);
			Console.WriteLine("...тестирование сокетов удаленного сервера окончено.");
			for (int i = 5; i >= 0; i--)
			{
				Thread.Sleep(1000);
				Console.WriteLine("Консоль закроется через " + i + "сек");
			}
		}
	}
}
