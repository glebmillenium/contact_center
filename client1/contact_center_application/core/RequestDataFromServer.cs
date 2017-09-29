using System;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using contact_center_application.serialization;

namespace contact_center_application.core
{
    public class RequestDataFromServer
    {
		public static string[] primaryExchangeWithSocket()
		{
			ConnectWithRemoteSocket.createSocket("localhost", 6500);
			//auth?
			return getListFileSystem();
		}

		private static string[] getListFileSystem()
		{
			ObjectForSerialization objForSend = new ObjectForSerialization
			{
				command = "get_aliance"
			};
			string resultJson = JsonConvert.SerializeObject(objForSend);
			Console.Write(resultJson);
			string answer = ConnectWithRemoteSocket.sendMessage(resultJson, 1024);
			ObjectForSerialization objResponse = 
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			string[] result = null;
			if (objResponse.command.Equals("aliance"))
			{
				result = JsonConvert.DeserializeObject<string[]>(objResponse.param1);
			}
			return result;
		}
	}
}
