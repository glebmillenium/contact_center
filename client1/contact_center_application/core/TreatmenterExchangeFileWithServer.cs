using contact_center_application.core.background_task;
using contact_center_application.core.serialization;
using contact_center_application.core.storage_dynamic_data;
using contact_center_application.graphic_user_interface.form;
using contact_center_application.graphic_user_interface.manage_graphical_component.viewer;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace contact_center_application.core
{
	class TreatmenterExchangeFileWithServer
	{

		public static void sendFileToServer()
		{
			int index = Int32.Parse(CurrentDataFileSystem.alianceIdPolicy[
							CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1);
			Tuple<bool, TreeViewItem> selectedItem = CurrentDataFileSystem.searchSelectedItem();

			string relativeWay = CurrentDataOpenFile.relationWayOpenFile;
			UploadWindow download = new UploadWindow(index.ToString(),
				Path.GetDirectoryName(relativeWay),
				Path.Combine(Path.GetDirectoryName(
						Assembly.GetExecutingAssembly().Locati‌​on),
						CurrentDataOpenFile.openFile),
				"1");
			download.sendFileToServer();
			try
			{
				ManagerViewer.LoadToViewer(CurrentDataOpenFile.openFile, ManagerViewer.currentView);
			}
			catch (OutOfMemoryException exceptionMemory)
			{
				MessageBox.Show("Системных ресурсов вашей операционной системы оказалось "
					+ "недостаточно для отображения содержимого файла("
					+ Path.GetFileName(CurrentDataOpenFile.openFile)
					+ ") в данном приложении. "
					+ "Попытайтесь открыть файл во внешнем приложении", "Нехватка системных ресурсов");
				Logger.log(exceptionMemory.Message);
			}
			catch (Exception exp)
			{
				MessageBox.Show("Неизвестная ошибка", "UNKNOWN");
				Logger.log(exp.Message);
			}
			finally
			{
				if (ManagerViewer.currentView.Equals("view/temp1"))
				{
					ManagerViewer.currentView = "view/temp2";
				}
				else
				{
					ManagerViewer.currentView = "view/temp1";
				}
				MainWindowElement.progressConvertation.Visibility = Visibility.Hidden;
			}
		}

		public static void loadFileToServer()
		{
			if (!CurrentDataOpenFile.dateOpenFile.Equals(File.GetLastWriteTime(CurrentDataOpenFile.openFile)))
			{
				if (MessageBox.Show("Отправить измененный файл на сервер?", "Файл был изменен",
					MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					TreatmenterExchangeFileWithServer.sendFileToServer();
				}
			}
			else
			{
				if (MessageBox.Show("Файл не был изменен. Все равно отправить на сервер?", "Файл не был изменен",
					MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					TreatmenterExchangeFileWithServer.sendFileToServer();
				}
			}
		}

		public static void getContentFileAndWriteToFile(string relativeWay, string aliance)
		{
			setData(2, "Сбор сведений о файле");
			try
			{
				clearFilesFromTempDirectory(relativeWay);
				setData(5, "Сбор сведений о файле");
				byte[] result;
				setData(7, "Загрузка файла");
				result = RequestDataFromServer.getContentFile(aliance, relativeWay);

				setData(95, "Файл успешно загружен");
				writeToFile("tmp\\" + relativeWay, result);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private static void clearFilesFromTempDirectory(string relativeWay)
		{
			string directoryWay = System.IO.Path.GetDirectoryName(relativeWay);
			Directory.CreateDirectory(directoryWay);
			if (File.Exists(relativeWay))
			{
				File.Delete(relativeWay);
			}
		}

		/// <summary>
		/// Функция осуществляет побайтовую запись в файл
		/// </summary>
		/// <param name="relativeWay"></param>
		/// <param name="contentFile"></param>
		public static void writeToFile(string relativeWay, byte[] contentFile)
		{
			string directoryWay = Path.GetDirectoryName(relativeWay);
			Directory.CreateDirectory(directoryWay);
			if (File.Exists(relativeWay))
			{
				File.Delete(relativeWay);
			}
			File.WriteAllBytes(relativeWay, contentFile);
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

		public static void setData(int value, string describe)
		{
			Messenger.value = value;
			Messenger.message = describe;
			try
			{
				MainWindowElement.backgroundWorkerDownload.ReportProgress(value);
			}
			catch (Exception edasd)
			{

			}
		}
	}
}
