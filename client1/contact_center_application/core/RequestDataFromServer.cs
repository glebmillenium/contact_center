﻿using Newtonsoft.Json;
using contact_center_application.serialization;
using System;

namespace contact_center_application.core
{
    public class RequestDataFromServer
    {
		public static string[] primaryExchangeWithSocket()
		{
			ConnectWithFastSocket.createSocket("localhost", 6500);
			ConnectWithFtpSocket.createSocket("localhost", 7000);

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

			answer = ConnectWithFtpSocket.sendMessage(resultJson, expectedSize);
			result = JsonConvert.DeserializeObject<string[]>(answer);
			return result;
		}

		public static string getCatalogFileSystem(string aliance)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "get_catalog",
				//TODO
				param1 = "0"
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
			answer = ConnectWithFtpSocket.sendMessage(resultJson, expectedSize);
			return answer;
		}

		public static string getContentFile(string aliance, string relativeWay)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization {
				command = "get_content_file",
				param1 = aliance,
				param2 = relativeWay
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 1024);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);

			string result = null;

			int expectedSize = Int32.Parse(objResponseFromFastSocket.param1);
			int index = Int32.Parse(objResponseFromFastSocket.param2);
			if (!objResponseFromFastSocket.command.Equals("content_file"))
			{
				return result;
			}
			ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
			{
				command = "content_file",
				param1 = "",
				param2 = index.ToString()
			};

			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			answer = ConnectWithFtpSocket.sendMessage(resultJson, expectedSize);

			return answer;
		}
	}
}
