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
				PathFolderBack + @"settings/version");
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
					Logger.log("Сокет передачи данных по ftp_server - открыт. Время выполнения: "
						+ timeExecute.Milliseconds.ToString() + " миллисекунд");
					array[i] = timeExecute.Milliseconds;
				}
				else
				{
					Logger.log("Сокет передачи данных по ftp_server - закрыт");
					error = true;
				}
				Thread.Sleep(200);
			}
			if (error)
			{
				Logger.log("Во время тестирования были ошибки");
			}
			else
			{
				Logger.log("Во время тестирования не было обнаружено ошибок. Среднее время отклика: " + array.Average());
			}
		}
	}
}
