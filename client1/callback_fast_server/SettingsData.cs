﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace callback_fast_server
{
	class SettingsData
	{
		private static string directoryIpAddress = @"settings/ip_connect";
		private static string directoryPortFTP	 = @"settings/port_ftp";
		private static string directoryPortFast	 = @"settings/port_fast";
		private static string directoryVersion	 = @"settings/version";


		public static void setDirectorySettings(string directoryOnIpAddress, string directoryOnPortFTP,
			string directoryOnPortFast, string directoryOnVersion)
		{
			directoryIpAddress = directoryOnIpAddress;
			directoryPortFTP = directoryOnPortFTP;
			directoryPortFast = directoryOnPortFast;
			directoryVersion = directoryOnVersion;
		}

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

		public static int getFtpPort()
		{
			int portFtp;
			try
			{
				portFtp = Int32.Parse(getContentFile(directoryPortFTP));
			}
			catch (Exception e)
			{
				portFtp = 6502;
				Logger.log(e.Message);
			}
			return portFtp;
		}

		public static int getFastPort()
		{
			int portFtp;
			try
			{
				portFtp = Int32.Parse(getContentFile(directoryPortFast));
			}
			catch (Exception e)
			{
				portFtp = 6500;
				Logger.log(e.Message);
			}
			return portFtp;
		}

		private static string getContentFile(string way)
		{
			return File.ReadAllText(way);
		}
	}
}