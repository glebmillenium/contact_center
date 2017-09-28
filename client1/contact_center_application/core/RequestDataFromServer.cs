using System;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using contact_center_application.json.command;

namespace contact_center_application.core
{
    public class RequestDataFromServer
    {
		public static void primaryExchangeWithSocket()
		{
			ConnectWithRemoteSocket.createSocket("localhost", 6500);

			getListFileSystem();
			
		}

		private static void getListFileSystem()
		{
			GetAliance commandForGetAliance = new GetAliance();
			commandForGetAliance.command = "get_aliance";
			commandForGetAliance.user = "gleb";
			commandForGetAliance.password = "1111";
			string resultJson = JsonConvert.SerializeObject(commandForGetAliance);
			Console.Write(resultJson);
		}
	}
}
