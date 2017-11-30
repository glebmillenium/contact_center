using contact_center_application.core;
using contact_center_application.graphic_user_interface.form;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace contact_center_application.graphic_user_interface.manage_graphical_component
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
	}
}
