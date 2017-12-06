using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace callback_ftp_server
{
	class Program
	{
		static void Main(string[] args)
		{
			string PathFolderBack = Path.GetDirectoryName(Directory.GetCurrentDirectory());
			SettingsData.setDirectorySettings(PathFolderBack + @"/settings/ip_connect",
				PathFolderBack + @"/settings/port_ftp", PathFolderBack + @"/settings/port_fast");
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

			ConnectWithFtpSocket.createSocket(address, portFtp);
			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			string answerSocket = ConnectWithFtpSocket.sendMessage(resultJson, 1024);
			ConnectWithFtpSocket.closeSocket();
			ObjectForSerialization objResponseFromFtpSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answerSocket);
			if (objResponseFromFtpSocket != null
					|| objResponseFromFtpSocket.command.Equals("answer_on_test_ftp_socket")
					&& objResponseFromFtpSocket.param1.Equals("true"))
			{
				Logger.log("Сокет передачи данных по ftp_server - открыт");
			}
			else
			{
				Logger.log("Сокет передачи данных по ftp_server - закрыт");
			}
		}
	}
}
