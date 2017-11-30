using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace contact_center_application.core
{
	class SettingsData
	{
		private static string directoryIpAddress = @"settings\ip_connect";

		public static string getAddress()
		{
			string address = "";
			try
			{
				address = File.ReadAllText(directoryIpAddress);
			}
			catch (Exception e)
			{
				address = "localhost";
				Logger.log(e.Message);
			}
			return address;
		}
	}
}
