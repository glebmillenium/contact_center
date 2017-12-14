using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace callback_fast_server
{
	class Program
	{
		static void Main(string[] args)
		{
			string PathFolderBack = Path.GetDirectoryName(Directory.GetCurrentDirectory());
			SettingsData.setDirectorySettings(PathFolderBack + @"/settings/version",
				PathFolderBack + @"/settings/view", PathFolderBack + @"/settings/network");
			Logger logger = new Logger(PathFolderBack + "/log/callback_fast_server_log.txt", 
				PathFolderBack + "/log/callback_fast_server_exception.txt", PathFolderBack + "/tmp");
			string address = SettingsData.getAddress();
			int port = SettingsData.getFastPort();

			float[] array = new float[20];
			bool error = false;
			string output;
			Console.WriteLine("Начало тестирование сокетов удаленного сервера...");
			for (int i = 0; i < array.Length; i++)
			{
				ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
				{
					command = "test_fast_socket",
				};
				string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
				ConnectWithFastSocket.createSocket(address, port);
				DateTime start = DateTime.Now;
				string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
				TimeSpan timeExecute = DateTime.Now - start;
				ObjectForSerialization objResponseFromFastSocket =
					JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
				if (objResponseFromFastSocket != null
					|| objResponseFromFastSocket.command.Equals("answer_on_test_fast_socket")
					&& objResponseFromFastSocket.param1.Equals("true"))
				{
					output = "Сокет передачи данных по fast_server - открыт. Время отклика от сервера: " + timeExecute.Milliseconds.ToString() + " миллисекунд";
					Logger.log(output);
					array[i] = timeExecute.Milliseconds;
				}
				else
				{
					output = "Сокет передачи данных по fast_server - закрыт";
					Logger.log(output);
					error = true;
				}
				Console.WriteLine(output);
				Thread.Sleep(200);
			}
			if (error)
			{
				output = "Во время тестирования были ошибки";
				Logger.log(output);
			}
			else
			{
				output = "Во время тестирования не было обнаружено ошибок. Среднее время отклика: " + array.Average();
				Logger.log(output);
			}
			Console.WriteLine(output);
			Console.WriteLine("...тестирование сокетов удаленного сервера окончено.");
			for (int i = 5; i >= 0; i--)
			{
				Thread.Sleep(1000);
				Console.WriteLine("Консоль закроется через " + i + "сек");
			}
		}
	}
}
