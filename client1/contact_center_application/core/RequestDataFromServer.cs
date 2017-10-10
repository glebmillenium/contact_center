using Newtonsoft.Json;
using contact_center_application.serialization;
using System;
using System.IO;
using System.Windows.Controls;
using System.Globalization;

namespace contact_center_application.core
{
    public class RequestDataFromServer
    {
		static string addressServer = "localhost";
		static int portFtp = 6502;
		static int portFast = 6500;
		public static string getAddressServer()
		{
			return addressServer;
		}

		public static string[] primaryExchangeWithSocket(string address = "localhost")
		{
			addressServer = address;
			try
			{
				ConnectWithFastSocket.createSocket(addressServer, portFast);
				//ConnectWithDataSocket.createSocket(addressServer, 6501);
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

			ConnectWithFtpSocket.createSocket(addressServer, 6502);
			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			string answerSocket = ConnectWithFtpSocket.sendMessage(resultJson, expectedSize);
			ConnectWithFtpSocket.closeSocket();

			result = JsonConvert.DeserializeObject<string[]>(answerSocket);
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

			ConnectWithFtpSocket.createSocket(addressServer, portFtp);
			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			answer = ConnectWithFtpSocket.sendMessage(resultJson, expectedSize);
			ConnectWithFtpSocket.closeSocket();
			
			return answer;
		}

		public static byte[] getContentFile(string aliance, string relativeWay)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization {
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
				return new byte[] { };
			}
			ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
			{
				command = "content_file",
				param1 = "",
				param2 = index.ToString()
			};

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


			string output = "Получение содержимого файла... Общий размер: " + outputSizeFile;

			ConnectWithFtpSocket.createSocket(addressServer, portFtp);
			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			byte[] answerToFunction = ConnectWithFtpSocket.sendMessageGetContentFile(resultJson, expectedSize);
			ConnectWithFtpSocket.closeSocket();

			return answerToFunction;
		}

		public static int getFtpPort()
		{
			return portFtp;
		}

		public static int getFastPort()
		{
			return portFast;
		}
	}
}
