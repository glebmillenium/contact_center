using contact_center_application.core.storage_dynamic_data;
using contact_center_application.graphic_user_interface.manage_graphical_component.viewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace contact_center_application.core.background_task
{
	class BackgroundDownload
	{
		public static void backgroundWorkerDownload_DoWork(object sender, DoWorkEventArgs e)
		{
			ArgumentBackgroundDonwload obj = (ArgumentBackgroundDonwload) e.Argument;
			TreatmenterExchangeFileWithServer.getContentFileAndWriteToFile(
					obj.getRelativeWay(), obj.getIndex());

		}

		public static void backgroundWorkerDownload_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			MainWindowElement.progressBarMessenger.Value = Messenger.value;
			MainWindowElement.textBlockMessenger.Text = Messenger.message;
		}

		public static void backgroundWorkerDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			try
			{
				ManagerViewer.LoadToViewer(CurrentDataOpenFile.openFile,
					ManagerViewer.currentView);
			}
			catch (OutOfMemoryException exceptionMemory)
			{
				MessageBox.Show("Системных ресурсов вашей операционной системы оказалось " +
					"недостаточно для отображения содержимого файла(" + Path.GetFileName(CurrentDataOpenFile.openFile) +
					") в данном приложении. " +
					"Попытайтесь открыть файл во внешнем приложении", "Нехватка системных ресурсов");
				Logger.log(exceptionMemory.Message);
			}
			catch (Exception exp)
			{
				MessageBox.Show("Неизвестная ошибка", "UNKNOWN");
				Logger.log(exp.Message);
			}

			if (ManagerViewer.currentView.Equals("view/temp1"))
			{
				ManagerViewer.currentView = "view/temp2";
			}
			else
			{
				ManagerViewer.currentView = "view/temp1";
			}
			MainWindowElement.progressConvertation.Visibility = Visibility.Hidden;
			MainWindowElement.managerPanel.Visibility = Visibility.Visible;
			MainWindowElement.cursor = Cursors.Arrow;
			MainWindowElement.stackPanelMessenger.Visibility = Visibility.Hidden;
		}
	}
}
