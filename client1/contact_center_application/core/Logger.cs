using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace contact_center_application.core
{
	class Logger
	{
		private static string directoryLog = "log/log.txt";
		private static string direcotryException = "log/exception.txt";
		private static string directoryForTemporary = "tmp";

		public static void initialize()
		{
			File.WriteAllText(directoryLog, "");
			File.WriteAllText(direcotryException, "");
			if (Directory.Exists(directoryForTemporary))
			{
				Directory.Delete(directoryForTemporary, true);
				Directory.CreateDirectory(directoryForTemporary);
			}
		}

		/// <summary>
		/// Метод для журналирования действий пользователя
		/// </summary>
		/// <param name="message">Сообщение которое будет отправлено пользователю</param>
		public static void log(string message)
		{
			StreamWriter writer = File.AppendText(directoryLog);
			writer.Write(DateTime.Now + " " + message + "\r\n");
			writer.Close();
		}
	}
}
