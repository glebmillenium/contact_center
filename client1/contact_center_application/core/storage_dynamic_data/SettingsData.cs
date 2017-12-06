using System;
using System.IO;

namespace contact_center_application.core.storage_dynamic_data
{
	class SettingsData
	{
		private static string directoryIpAddress = @"settings\ip_connect";
		private static string directoryPortFTP = @"settings\port_ftp";
		private static string directoryPortFast = @"settings\port_fast";
		private static string address = "localhost";
		private static string portFtp = "6502";
		private static string portFast = "6500";

		public static string getAddress()
		{
			string address = "";
			try
			{
				address = getContentFile(directoryIpAddress);
			}
			catch (Exception e)
			{
				address = "localhost";
				Logger.log(e.Message);
			}
			return address;
		}

		public static string getFtpPort()
		{
			string portFtp = "";
			try
			{
				portFtp = getContentFile(directoryPortFTP);
			}
			catch (Exception e)
			{
				portFtp = "6502";
				Logger.log(e.Message);
			}
			return portFtp;
		}

		public static string getFastPort()
		{
			string portFast = "";
			try
			{
				portFast = getContentFile(directoryPortFast);
			}
			catch (Exception e)
			{
				portFast = "6500";
				Logger.log(e.Message);
			}
			return portFast;
		}

		private static string getContentFile(string way)
		{
			return File.ReadAllText(way);
		}
	}
}
