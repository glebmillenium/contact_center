using Newtonsoft.Json;
using System;
using System.IO;
using System.Globalization;

namespace Patcher
{
    public class RequestDataFromServer
    {
		static string addressServer = "128.0.0.1";
		static int portFtp = 6502;
		static int portFast = 6500;

		public static string[] getDifferenceVersion(string version)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "get_difference_version",
				param1 = version
			};
			
			ConnectWithFastSocket.createSocket(SettingsData.getAddress(),
				SettingsData.getFastPort());
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			string[] result = null;

			if (!objResponseFromFastSocket.param2.Equals("0"))
			{

				int expectedSize = Int32.Parse(objResponseFromFastSocket.param1);
				int index = Int32.Parse(objResponseFromFastSocket.param2);
				if (!objResponseFromFastSocket.command.Equals("version"))
				{
					return result;
				}
				ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
				{
					command = "get_difference_version",
					param1 = "",
					param2 = index.ToString()
				};

				resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);

				ConnectWithFtpSocket.createSocket(addressServer, 6502);
				resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
				string answerSocket = ConnectWithFtpSocket.sendMessage(resultJson, expectedSize);
				ConnectWithFtpSocket.closeSocket();

				result = JsonConvert.DeserializeObject<string[]>(answerSocket);
			}
			return result;
		}

		public static string[] getVersion(string version)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "get_version",
				param1 = version
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			string[] result = null;

			int expectedSize = Int32.Parse(objResponseFromFastSocket.param1);
			int index = Int32.Parse(objResponseFromFastSocket.param2);
			if (!objResponseFromFastSocket.command.Equals("version"))
			{
				return result;
			}
			ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
			{
				command = "get_version",
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

		public static byte[] getFileForUpdateNewVersion(string version, string currentWay)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "get_update",
				param1 = version,
				param2 = "",
				param3 = "",
				param4_array = System.Text.Encoding.UTF8.GetBytes(currentWay)
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);

			int expectedSize = Int32.Parse(objResponseFromFastSocket.param1);
			int index = Int32.Parse(objResponseFromFastSocket.param2);
			if (!objResponseFromFastSocket.command.Equals("update"))
			{
				return new byte[] { };
			}
			ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
			{
				command = "get_update",
				param1 = "",
				param2 = index.ToString()
			};

			string outputSizeFile = getHumanSize(expectedSize);
			string output = "Получение содержимого файла... Общий размер: " + outputSizeFile;

			ConnectWithFtpSocket.createSocket(addressServer, portFtp);
			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			byte[] answerFromFunction = ConnectWithFtpSocket.sendMessageGetContentFile(resultJson, expectedSize);
			ConnectWithFtpSocket.closeSocket();

			return answerFromFunction;
		}

		public static void getFilesForUpdate()
		{

		}

		public static void createConnection()
		{
			closeConnection();
			addressServer = SettingsData.getAddress();
			portFtp = SettingsData.getFtpPort();
			portFast = SettingsData.getFastPort();
			ConnectWithFastSocket.createSocket(addressServer, portFast);
		}

		public static void closeConnection()
		{
			ConnectWithFastSocket.closeSocket();
		}

		public static void rebootFastServer()
		{
			closeConnection();
			primaryExchangeWithSocket();
		}

		public static string[] primaryExchangeWithSocket()
		{
			addressServer = SettingsData.getAddress();
			portFtp = SettingsData.getFtpPort();
			portFast = SettingsData.getFastPort();
			try
			{
				ConnectWithFastSocket.createSocket(addressServer, portFast);
			}
			catch (System.Net.Sockets.SocketException socketException)
			{
				throw new System.Net.Sockets.SocketException();
			}

			return getAliance();
		}

		public static Tuple<String, int, String> getRightAccess(string login, string password)
		{
			addressServer = SettingsData.getAddress();
			portFtp = SettingsData.getFtpPort();
			portFast = SettingsData.getFastPort();
			try
			{
				ConnectWithFastSocket.createSocket(addressServer, portFast);

				ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
				{
					command = "auth",
					param1 = "",
					param2 = SettingsData.getVersion(),
					param3 = "",
					param4_array = System.Text.Encoding.UTF8.GetBytes(login),
					param5_array = System.Text.Encoding.UTF8.GetBytes(password)
				};
				string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
				string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
				ObjectForSerialization objResponseFromFastSocket =
					JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
				Tuple<String, int, String> result;
				try
				{
					result = new Tuple<String, int, String>(objResponseFromFastSocket.param2, Int32.Parse(objResponseFromFastSocket.param1),
						System.Text.Encoding.UTF8.GetString(objResponseFromFastSocket.param4_array));
				}
				catch (Exception e)
				{
					result = new Tuple<String, int, String>("Неизвестно", -1,
						"Сервер не находится в рабочем состоянии");
				}
				return result;
			}
			catch (Exception socketException)
			{
				throw new Exception();
			}
		}

		private static string[] getAliance()
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "get_aliance"
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
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

		private static bool sendMessageToRenameFile()
		{
			return true;
		}

		private static bool sendMessageToUploadFile(string aliance, 
			string relativePathWithExistFileSystem, string pathToUploadFile)
		{
			bool notificationToUserInGUI = false;
			FileInfo fi = new FileInfo(pathToUploadFile);
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "try_upload",
				param1 = fi.Length.ToString(),
				param2 = aliance,
				param3 = "",
				param4_array = System.Text.Encoding.UTF8.GetBytes(relativePathWithExistFileSystem)
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			if (objResponseFromFastSocket.param2.Equals("1"))
			{
				int index = Int32.Parse(objResponseFromFastSocket.param1);
				ObjectForSerialization objForSendToFtpSocket = new ObjectForSerialization
				{
					command = "try_upload",
					param1 = "",
					param2 = index.ToString()
				};

				ConnectWithFtpSocket.createSocket(addressServer, portFtp);
				resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
				notificationToUserInGUI = ConnectWithFtpSocket.sendFile(resultJson, 
					pathToUploadFile, fi.Length);
				ConnectWithFtpSocket.closeSocket();
				return notificationToUserInGUI;
			}
			//answer.param1 - номер запроса
			//answer.param2 - результат 0/1
			return notificationToUserInGUI;
		}

		private static bool sendMessageToDeleteFile()
		{
			return true;
		}

		public static string getCatalog(string aliance)
		{
			try
			{
				ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
				{
					command = "get_catalog",
					param1 = aliance
				};

				string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
				string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
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
			} catch(FormatException excp)
			{
				throw new FormatException();
			}
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
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
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

			string outputSizeFile = getHumanSize(expectedSize);
			string output = "Получение содержимого файла... Общий размер: " + outputSizeFile;

			ConnectWithFtpSocket.createSocket(addressServer, portFtp);
			resultJson = JsonConvert.SerializeObject(objForSendToFtpSocket);
			byte[] answerFromFunction = ConnectWithFtpSocket.sendMessageGetContentFile(resultJson, expectedSize);
			ConnectWithFtpSocket.closeSocket();

			return answerFromFunction;
		}

		private static string getHumanSize(int expectedSize)
		{
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
			return outputSizeFile;
		}

		public static int getFtpPort()
		{
			return portFtp;
		}

		public static int getFastPort()
		{
			return portFast;
		}

		public static bool sendToRenameObjectFileSystem(string aliance, string relativeWay, string newName)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "try_rename",
				param1 = aliance,
				param2 = "", param3 = "",
				param4_array = System.Text.Encoding.UTF8.GetBytes(relativeWay),
				param5_array = System.Text.Encoding.UTF8.GetBytes(newName)
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			if (objResponseFromFastSocket.param1.Equals("1"))
				return true;
			else
				return false;
		}

		public static bool sendToCreateCatalogFileSystem(string aliance, string relativeWay, string nameDirectory)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "try_create_dir",
				param1 = aliance,
				param2 = "",
				param3 = "",
				param4_array = System.Text.Encoding.UTF8.GetBytes(relativeWay),
				param5_array = System.Text.Encoding.UTF8.GetBytes(nameDirectory)
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			if (objResponseFromFastSocket.param1.Equals("1"))
				return true;
			else
				return false;
		}

		public static bool sendToDeleteObjectFileSystem(string aliance, string relativeWay)
		{
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "try_remove",
				param1 = aliance,
				param2 = "",
				param3 = "",
				param4_array = System.Text.Encoding.UTF8.GetBytes(relativeWay),
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			if (objResponseFromFastSocket.param1.Equals("1"))
				return true;
			else
				return false;
		}
	}
}