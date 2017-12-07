using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patcher
{
	class UpdateRootCatalog
	{
		public void startUpdate()
		{
			string PathFolderBack = Path.GetDirectoryName(Directory.GetCurrentDirectory());
			SettingsData.setDirectorySettings(PathFolderBack + @"/settings/ip_connect",
				PathFolderBack + @"/settings/port_ftp", PathFolderBack + @"/settings/port_fast",
				PathFolderBack + @"/settings/version",
				PathFolderBack + @"/settings/update_file_system");

			string address = SettingsData.getAddress();
			int fastPort = SettingsData.getFastPort();
			int ftpPort = SettingsData.getFtpPort();
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "test_fast_socket",
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			ConnectWithFastSocket.createSocket(address, fastPort);
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
