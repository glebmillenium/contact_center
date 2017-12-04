using contact_center_application.core.storage_dynamic_data;
using contact_center_application.graphic_user_interface.form;
using contact_center_application.graphic_user_interface.manage_graphical_component.viewer;
using System;
using System.IO;
using System.Reflection;
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
	}
}
