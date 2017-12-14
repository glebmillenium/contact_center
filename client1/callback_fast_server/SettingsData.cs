using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace callback_fast_server
{
	class SettingsData
	{
		private static string directoryVersion = @"settings/version";
		private static string directoryView = @"settings/view";
		private static string directoryNetwork = @"settings/network";
		private static int rightRead = 0;
		private static int rightWrite = 0;
		private static int rightExecute = 0;

		public static void setDirectorySettings(string directoryVersion,
			string directoryView, string directoryNetwork)
		{
			SettingsData.directoryView = directoryView;
			SettingsData.directoryNetwork = directoryNetwork;
			SettingsData.directoryVersion = directoryVersion;
		}

		public static void setRightAccess(int right)
		{
			BitArray result = new BitArray(new int[] { right });
			rightRead = result[0] == false ? 0 : 1;
			rightWrite = result[1] == false ? 0 : 1;
			rightExecute = result[2] == false ? 0 : 1;
		}

		public static int getRightRead()
		{
			return rightRead;
		}

		public static int getRightWrite()
		{
			return rightWrite;
		}

		public static int getRightExecute()
		{
			return rightExecute;
		}

		public static void setVersion(string version)
		{
			File.WriteAllText(directoryVersion, version);
		}

		public static string getVersion()
		{
			string version = "";
			try
			{
				version = getContentFile(directoryVersion);
			}
			catch (Exception e)
			{
				version = "0.0.1";
				Logger.log(e.Message);
			}
			return version;
		}

		public static string getAddress()
		{
			string address = "";
			try
			{
				address = getContentFile(directoryNetwork + @"/ip_connect");
			}
			catch (Exception e)
			{
				address = "localhost";
				Logger.log(e.Message);
			}
			return address;
		}

		public static void setAddress(string address)
		{
			File.WriteAllText(directoryNetwork + @"/ip_connect", address);
		}

		public static string getIntervalUpdate()
		{
			string intervalUpdate = "";
			try
			{
				intervalUpdate = getContentFile(directoryNetwork + @"/time_update_file_system");
			}
			catch (Exception e)
			{
				intervalUpdate = "2";
				Logger.log(e.Message);
			}
			return intervalUpdate;
		}

		public static int getFtpPort()
		{
			int portFtp;
			try
			{
				portFtp = Int32.Parse(getContentFile(directoryNetwork + @"/port_ftp"));
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
			int portFast;
			try
			{
				portFast = Int32.Parse(getContentFile(directoryNetwork + @"/port_fast"));
			}
			catch (Exception e)
			{
				portFast = 6500;
				Logger.log(e.Message);
			}
			return portFast;
		}

		public static bool isViewDocDocx()
		{
			bool result;
			try
			{
				result = bool.Parse(getContentFile(directoryView + @"/doc_docx"));
			}
			catch (Exception e)
			{
				result = false;
				Logger.log(e.Message);
			}
			return result;
		}

		public static bool isViewXlsXlsx()
		{
			bool result;
			try
			{
				result = bool.Parse(getContentFile(directoryView + @"/xls_xlsx"));
			}
			catch (Exception e)
			{
				result = false;
				Logger.log(e.Message);
			}
			return result;
		}

		public static bool isViewPdf()
		{
			bool result;
			try
			{
				result = bool.Parse(getContentFile(directoryView + @"/pdf"));
			}
			catch (Exception e)
			{
				result = false;
				Logger.log(e.Message);
			}
			return result;
		}

		public static bool isViewJpegTiffJpgPng()
		{
			bool result;
			try
			{
				result = bool.Parse(getContentFile(directoryView + @"/jpeg_tiff_jpg_png"));
			}
			catch (Exception e)
			{
				result = false;
				Logger.log(e.Message);
			}
			return result;
		}

		public static bool isViewTxtCsv()
		{
			bool result;
			try
			{
				result = bool.Parse(getContentFile(directoryView + @"/txt_csv"));
			}
			catch (Exception e)
			{
				result = false;
				Logger.log(e.Message);
			}
			return result;
		}

		public static bool isOpenNow()
		{
			bool result;
			try
			{
				result = bool.Parse(getContentFile(directoryView + @"/open_now"));
			}
			catch (Exception e)
			{
				result = false;
				Logger.log(e.Message);
			}
			return result;
		}

		public static int getReservePort()
		{
			int portReserve;
			try
			{
				portReserve = Int32.Parse(getContentFile(directoryNetwork + @"/port_reserve"));
			}
			catch (Exception e)
			{
				portReserve = 6500;
				Logger.log(e.Message);
			}
			return portReserve;
		}

		public static void setReservePort(int port)
		{
			File.WriteAllText(directoryNetwork + @"/port_reserve", port.ToString());
		}

		public static void setFastPort(int port)
		{
			File.WriteAllText(directoryNetwork + @"/port_fast", port.ToString());
		}

		public static void setFtpPort(int port)
		{
			File.WriteAllText(directoryNetwork + @"/port_ftp", port.ToString());
		}

		public static void setOpenNow(bool isOpenNow)
		{
			File.WriteAllText(directoryView + @"/open_now", isOpenNow.ToString());
		}

		public static void setViewJpegTiffJpgPng(bool viewJpegTiffJpgPng)
		{
			File.WriteAllText(directoryView + @"/jpeg_tiff_jpg_png", viewJpegTiffJpgPng.ToString());
		}

		public static void setViewTxtCsv(bool viewTxtCsv)
		{
			File.WriteAllText(directoryView + @"/txt_csv", viewTxtCsv.ToString());
		}

		public static void setViewXlsXlsx(bool viewXlsXlsx)
		{
			File.WriteAllText(directoryView + @"/xls_xlsx", viewXlsXlsx.ToString());
		}

		public static void setViewDocDocx(bool viewDocDocx)
		{
			File.WriteAllText(directoryView + @"/doc_docx", viewDocDocx.ToString());
		}

		public static void setViewPdf(bool viewPdf)
		{
			File.WriteAllText(directoryView + @"/pdf", viewPdf.ToString());
		}

		public static void setIntervalUpdate(int interval)
		{
			File.WriteAllText(directoryNetwork + @"/time_update_file_system", interval.ToString());
		}

		private static string getContentFile(string way)
		{
			return File.ReadAllText(way);
		}
	}
}
