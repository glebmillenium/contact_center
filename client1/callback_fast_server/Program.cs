using Newtonsoft.Json;
using System.IO;

namespace callback_fast_server
{
	class Program
	{
		static void Main(string[] args)
		{

			string PathFolderBack = Path.GetDirectoryName(Directory.GetCurrentDirectory());
			SettingsData.setDirectorySettings(PathFolderBack + @"/settings/ip_connect",
				PathFolderBack + @"/settings/port_ftp", PathFolderBack + @"/settings/port_fast");
			Logger logger = new Logger(PathFolderBack + "/log/callback_fast_server_log.txt", 
				PathFolderBack + "/log/callback_fast_server_exception.txt", PathFolderBack + "/tmp");
			string address = SettingsData.getAddress();
			int port = SettingsData.getFastPort();
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "test_fast_socket",
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			ConnectWithFastSocket.createSocket(address, port);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			if (objResponseFromFastSocket != null
				|| objResponseFromFastSocket.command.Equals("answer_on_test_fast_socket")
				&& objResponseFromFastSocket.param1.Equals("true"))
			{
				
				Logger.log("Сокет передачи данных по fast_server - открыт");
			}
			else
			{
				Logger.log("Сокет передачи данных по fast_server - закрыт");
			}
		}
	}
}
