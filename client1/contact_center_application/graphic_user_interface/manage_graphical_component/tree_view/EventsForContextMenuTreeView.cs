﻿using contact_center_application.core;
using contact_center_application.core.background_task;
using contact_center_application.core.storage_dynamic_data;
using contact_center_application.graphic_user_interface.form;
using contact_center_application.graphic_user_interface.manage_graphical_component.viewer;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace contact_center_application.graphic_user_interface.manage_graphical_component.tree_view
{
	class EventsForContextMenuTreeView
	{
		/// <summary>
		/// Событие вызываемое при создании категории
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void CreateCategory_Click(object sender, RoutedEventArgs e)
		{
			RenameUnitFileSystem dialog = new RenameUnitFileSystem(true);
			dialog.ShowDialog();
			if ((bool)dialog.DialogResult)
			{
				string nameDirectory = dialog.getNameFile();
				string index = CurrentDataFileSystem.alianceIdPolicy[
					CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1;
				RequestDataFromServer.sendToCreateCatalogFileSystem(index, "", nameDirectory);
			}
		}

		/// <summary>
		/// Событие вызываемое при загрузке файла в систему
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void Upload_Click(object sender, EventArgs e)
		{
			System.Windows.Forms.OpenFileDialog OPF = new System.Windows.Forms.OpenFileDialog();
			OPF.Filter = "Все документы|*.*|Файлы txt|*.txt|Файлы csv|*.csv|Файлы doc|*.doc|Файлы docx|*.docx|Файлы xls|*.xls|Файлы xlsx|*.xlsx|Файлы tiff|*.tiff";
			if (OPF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				int index = Int32.Parse(CurrentDataFileSystem.alianceIdPolicy[CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1);
				Tuple<bool, TreeViewItem> selectedItem = CurrentDataFileSystem.searchSelectedItem();

				string relativeWay = CurrentDataFileSystem.listTreeView[selectedItem.Item2].Item2;
				UploadWindow upload = new UploadWindow(index.ToString(), relativeWay,
					OPF.FileName, "0");
				try
				{
					upload.sendFileToServer();
					ButtonUpdateCatalogs_Click(null, null);
				}
				catch (Exception exp)
				{
					Logger.log(exp.Message);

				}
			}
		}

		public static void UploadCategory_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.OpenFileDialog OPF = new System.Windows.Forms.OpenFileDialog();
			OPF.Filter = "Все документы|*.*|Файлы txt|*.txt|Файлы csv|*.csv|Файлы doc|*.doc|Файлы docx|*.docx|Файлы xls|*.xls|Файлы xlsx|*.xlsx|Файлы tiff|*.tiff|Файлы png|*.png";
			if (OPF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				int index = Int32.Parse(CurrentDataFileSystem.alianceIdPolicy[CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1);

				string relativeWay = "";
				UploadWindow download = new UploadWindow(index.ToString(), relativeWay,
					OPF.FileName, "0");
				try
				{
					download.sendFileToServer();

				}
				catch (Exception exp)
				{
					Logger.log(exp.Message);
				}
				ButtonUpdateCatalogs_Click(null, null);
			}
		}

		/// <summary>
		/// Получение содержимого файловой системы, вызывается при нажатии
		/// на кнопку обновление каталогов
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void ButtonUpdateCatalogs_Click(object sender, RoutedEventArgs e)
		{
			/**this.updateCatalog = true;
			TreeViewItem selectedItem = searchSelectedItem().Item2;
			resetFlagsInTreeViewItem();
			getContentFileSystem();
			deleteNotNeedItemsInTreeViewItem(selectedItem);
			selectedItem.IsSelected = true;
			this.updateCatalog = false;*/
		}

		/// <summary>
		/// Событие вызываемое при запросе на удаление файла
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void Delete_Click(object sender, RoutedEventArgs e)
		{
			string index = CurrentDataFileSystem.alianceIdPolicy[CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1;
			Tuple<bool, TreeViewItem> selectedItem = CurrentDataFileSystem.searchSelectedItem();
			selectedItem.Item2.IsSelected = false;
			string relativeWay = CurrentDataFileSystem.listTreeView[selectedItem.Item2].Item2;

			RequestDataFromServer.sendToDeleteObjectFileSystem(index, relativeWay);
			ButtonUpdateCatalogs_Click(null, null);
		}

		/// <summary>
		/// Событие вызываемое при создании директории
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void CreateDir_Click(object sender, RoutedEventArgs e)
		{
			RenameUnitFileSystem dialog = new RenameUnitFileSystem(true);
			dialog.ShowDialog();
			if ((bool)dialog.DialogResult)
			{
				string nameDirectory = dialog.getNameFile();
				string index = CurrentDataFileSystem.alianceIdPolicy[CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1;
				Tuple<bool, TreeViewItem> selectedItem = CurrentDataFileSystem.searchSelectedItem();
				string relativeWay = "";
				if (selectedItem.Item1)
				{
					relativeWay = CurrentDataFileSystem.listTreeView[selectedItem.Item2].Item2;
				}
				RequestDataFromServer.sendToCreateCatalogFileSystem(index, relativeWay, nameDirectory);
				ButtonUpdateCatalogs_Click(null, null);
			}
		}

		/// <summary>
		/// Событие вызываемое для переименование файла
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void Rename_Click(object sender, RoutedEventArgs e)
		{
			Tuple<bool, TreeViewItem> selectedItem = CurrentDataFileSystem.searchSelectedItem();
			string relativeWay = CurrentDataFileSystem.listTreeView[selectedItem.Item2].Item2;
			RenameUnitFileSystem dialog = new RenameUnitFileSystem(Path.GetFileName(relativeWay));
			dialog.ShowDialog();
			if ((bool)dialog.DialogResult)
			{
				string index = CurrentDataFileSystem.alianceIdPolicy[CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1;

				string nameFile = dialog.getNameFile();
				RequestDataFromServer.sendToRenameObjectFileSystem(index, relativeWay, nameFile);
			}
			ButtonUpdateCatalogs_Click(null, null);
		}

		/// <summary>
		/// Событие вызываемое при открытии контекстного меню, осуществляет выделение treeviewitem
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void Item_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			bool value = e.Handled;
			TreeViewItem item = sender as TreeViewItem;
			if (item != null)
			{
				item.Focus();
				item.ContextMenu.IsOpen = true;
				e.Handled = true;
			}
		}

		/// <summary>
		/// Событие вызываемое при открытии файла
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void Open_Click(object sender, RoutedEventArgs e)
		{
			selectFile(null, null);
		}

		/// <summary>
		/// Обработчик события, запускается по двойному щелчку мыши по treeView. 
		/// Запускает процесс загрузки содержимого файла с удаленного сервера
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void selectFile(object sender, RoutedEventArgs e)
		{
			try
			{
				MainWindowElement.stackPanelMessenger.Visibility = Visibility.Visible;
				Tuple<bool, TreeViewItem> selectedItem = CurrentDataFileSystem.searchSelectedItem();
				if (selectedItem.Item1)
				{
					MainWindowElement.cursor = Cursors.Wait;

					ManagerViewer.callGarbage();

					string aliance = CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString();
					string relativeWay = CurrentDataFileSystem.listTreeView[selectedItem.Item2].Item2;

					CurrentDataOpenFile.openFile = "tmp" + relativeWay;
					int index = Int32.Parse(CurrentDataFileSystem.alianceIdPolicy[
						CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1);
					MainWindowElement.progressConvertation.Visibility = Visibility.Visible;

					MainWindowElement.backgroundWorkerDownload.RunWorkerAsync(new ArgumentBackgroundDownload(relativeWay, index.ToString()));
				}
			}
			catch (Exception exp)
			{
				Logger.log(exp.ToString());
				System.Windows.MessageBox.Show("Отказали системные компоненты приложения. " +
					"Попробуйте повторить действие. В случае повторного возникновения ошибки перезапустите приложение.", "Критическая ошибка");
				MainWindowElement.managerPanel.Visibility = Visibility.Visible;
				MainWindowElement.cursor = Cursors.Arrow;
				MainWindowElement.stackPanelMessenger.Visibility = Visibility.Hidden;
				MainWindowElement.window.IsEnabled = true;

				RequestDataFromServer.rebootFastServer_2();
			}
		}

		private static void openWeb(string url)
		{
			string messageBoxText = "Вы уверены, что хотите перейти по этой ссылке: " + url + "?";
			string caption = "Переход по ссылке";
			MessageBoxButton button = MessageBoxButton.YesNoCancel;
			MessageBoxImage icon = MessageBoxImage.Question;
			MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
			if (result == MessageBoxResult.Yes)
			{
				try
				{
					System.Diagnostics.Process.Start(url);
				}
				catch (System.ComponentModel.Win32Exception e)
				{
					Logger.log(e.Message);
					System.Windows.MessageBox.Show("Ошибка",
					"Не удалось открыть ссылку");
				}
			}
		}

		public static void Item_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				EventsForContextMenuTreeView.selectFile(null, null);
			}
		}
	}
}
