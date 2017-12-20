using contact_center_application.core;
using contact_center_application.core.storage_dynamic_data;
using contact_center_application.graphic_user_interface.manage_graphical_component.viewer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace contact_center_application.graphic_user_interface.manage_graphical_component.tree_view
{
	class EventsForButtons
	{
		public static void	openFile()
		{
			try
			{
				DateTime oldTime = File.GetLastWriteTime(CurrentDataOpenFile.openFile);
				Process currProc = Process.Start(CurrentDataOpenFile.openFile);
				currProc.WaitForExit();
				currProc.Close();
				if (!oldTime.Equals(File.GetLastWriteTime(CurrentDataOpenFile.openFile)))
				{
					if (SettingsData.getRightWrite() == 1)
					{

						if (MessageBox.Show("Отправить измененный файл на сервер?", "Файл был изменен",
							MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
						{
							TreatmenterExchangeFileWithServer.sendFileToServer();
						}
					}
					else
					{
						//						LightMessenger lm = new LightMessenger("Вы изменили содержимое файла." +
						//	" Но у вас недостаточно прав для отправки файла на сервер.");
						//						backgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
						//						backgroundWorker.RunWorkerAsync(searchTextBox);
					}
				}
			}
			catch (System.NullReferenceException exceptionWithOpenFile)
			{
				Logger.log(exceptionWithOpenFile.Message);
			}
		}

		public static void ButtonUpdateCatalogs_Click(object sender, RoutedEventArgs e)
		{
			ManagerViewer.updateCatalog = true;
			TreeViewItem selectedItem = CurrentDataFileSystem.searchSelectedItem().Item2;
			FilterTreeViewItem.resetFlagsInTreeViewItem();
			CurrentDataFileSystem.getContentFileSystem();
			CurrentDataFileSystem.deleteNotNeedItemsInTreeViewItem();
			selectedItem.IsSelected = true;
			ManagerViewer.updateCatalog = false;
		}
	}
}
