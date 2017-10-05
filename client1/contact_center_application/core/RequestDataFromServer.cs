using Newtonsoft.Json;
using contact_center_application.serialization;
using System;
using System.IO;

namespace contact_center_application.core
{
    public class RequestDataFromServer
    {
		public static string[] primaryExchangeWithSocket()
		{
			try
			{
				ConnectWithFastSocket.createSocket("localhost", 6500);
				ConnectWithDataSocket.createSocket("localhost", 6501);
			}
			catch (System.Net.Sockets.SocketException socketException)
			{
				throw new System.Net.Sockets.SocketException();
			}

			return getListFileSystem();
		}

		private static string[] getListFileSystem()
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "get_aliance"
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 1024);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			string[] result = null;

			int expectedSize = Int32.Parse(objResponseFromFastSocket.param1);
			int index = Int32.Parse(objResponseFromFastSocket.param2);
			if (!objResponseFromFastSocket.command.Equals("aliance"))
			{
				return result;
			}
			ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
			{
				command = "get_aliance",
				param1 = "",
				param2 = index.ToString()
			};

			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);

			answer = ConnectWithDataSocket.sendMessage(resultJson, expectedSize);
			result = JsonConvert.DeserializeObject<string[]>(answer);
			return result;
		}

		public static string getCatalogFileSystem(string aliance)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "get_catalog",
				param1 = aliance
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 1024);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			string result = null;

			int expectedSize = Int32.Parse(objResponseFromFastSocket.param1);
			int index = Int32.Parse(objResponseFromFastSocket.param2);
			if (!objResponseFromFastSocket.command.Equals("catalog"))
			{
				return result;
			}
			ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
			{
				command = "get_catalog",
				param1 = "",
				param2 = index.ToString()
			};

			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			answer = ConnectWithDataSocket.sendMessage(resultJson, expectedSize);
			return answer;
		}

		public static byte[] getContentFile(string aliance, string relativeWay)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization {
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

			ConnectWithFtpSocket.createSocket("localhost", 6502);
			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			byte[] answerToFunction = ConnectWithFtpSocket.sendMessageGetContentFile(resultJson, expectedSize);
			ConnectWithFtpSocket.closeSocket();
			///////////////////////
			File.WriteAllBytes("C:\\Users\\admin\\Desktop\\log\\client.log", answerToFunction);

			return answerToFunction;
		}
	}
}
