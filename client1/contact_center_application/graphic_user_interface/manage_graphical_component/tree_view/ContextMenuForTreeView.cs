using contact_center_application.core;
using contact_center_application.serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace contact_center_application.graphic_user_interface.manage_graphical_component
{
	class ContextMenuForTreeView
	{
		private static void policyContactCenter(TreeViewItem item,
			ContextMenu docMenu, CatalogForSerialization element, int deep)
		{
			System.Windows.Controls.MenuItem upload = new System.Windows.Controls.MenuItem();
			switch (deep)
			{
				case 1:
					upload.Header = "Загрузить файл в категорию";
					upload.Click += EventsForContextMenuTreeView.Upload_Click;
					docMenu.Items.Add(upload);
					break;
				case 2:
					upload.Header = "Загрузить файл в тему";
					upload.Click += EventsForContextMenuTreeView.Upload_Click;
					docMenu.Items.Add(upload);
					break;
				case 3:
					upload.Header = "Загрузить файл в подтему";
					upload.Click += EventsForContextMenuTreeView.Upload_Click;
					docMenu.Items.Add(upload);
					break;
			}

			System.Windows.Controls.MenuItem createDir = new System.Windows.Controls.MenuItem();
			switch (deep)
			{
				case 1:
					createDir.Header = "Создать новую тему в " + element.name;
					createDir.Click += EventsForContextMenuTreeView.CreateDir_Click;
					docMenu.Items.Add(createDir);
					break;
				case 2:
					createDir.Header = "Создать новую подтему в " + element.name;
					createDir.Click += EventsForContextMenuTreeView.CreateDir_Click;
					docMenu.Items.Add(createDir);
					break;
			}

			System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
			switch (deep)
			{
				case 1:
					delete.Header = "Удалить категорию";
					delete.Click += EventsForContextMenuTreeView.Delete_Click;
					docMenu.Items.Add(delete);
					break;
				case 2:
					delete.Header = "Удалить тему";
					delete.Click += EventsForContextMenuTreeView.Delete_Click;
					docMenu.Items.Add(delete);
					break;
				case 3:
					delete.Header = "Удалить подтему";
					delete.Click += EventsForContextMenuTreeView.Delete_Click;
					docMenu.Items.Add(delete);
					break;
				default:
					delete.Header = "Удалить папку";
					delete.Click += EventsForContextMenuTreeView.Delete_Click;
					docMenu.Items.Add(delete);
					break;
			}

			System.Windows.Controls.MenuItem rename = new System.Windows.Controls.MenuItem();
			switch (deep)
			{
				case 1:
					rename.Header = "Переименовать категорию";
					rename.Click += EventsForContextMenuTreeView.Rename_Click;
					docMenu.Items.Add(rename);
					break;
				case 2:
					rename.Header = "Переименовать тему";
					rename.Click += EventsForContextMenuTreeView.Rename_Click;
					docMenu.Items.Add(rename);
					break;
				case 3:
					rename.Header = "Переименовать подтему";
					rename.Click += EventsForContextMenuTreeView.Rename_Click;
					docMenu.Items.Add(rename);
					break;
			}

			item.ContextMenuOpening += EventsForContextMenuTreeView.Item_ContextMenuOpening;
			item.ContextMenu = docMenu;
		}



		public static void setContextMenuToTreeViewItem(TreeViewItem item, CatalogForSerialization element, int deep)
		{
			System.Windows.Controls.ContextMenu docMenu = new System.Windows.Controls.ContextMenu();
			int policy = Int32.Parse(CurrentDataFileSystem.alianceIdPolicy[CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item2);
			switch (policy)
			{
				case 1:
					policyContactCenter(item, docMenu, element, deep);
					break;
				default:
					System.Windows.Controls.MenuItem upload = new System.Windows.Controls.MenuItem();
					upload.Header = "Загрузить файл в папку";
					upload.Click += EventsForContextMenuTreeView.Upload_Click;
					docMenu.Items.Add(upload);

					System.Windows.Controls.MenuItem createDir = new System.Windows.Controls.MenuItem();
					createDir.Header = "Создать новую папку в " + element.name;
					createDir.Click += EventsForContextMenuTreeView.CreateDir_Click;
					docMenu.Items.Add(createDir);

					System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
					delete.Header = "Удалить папку";
					delete.Click += EventsForContextMenuTreeView.Delete_Click; ;
					docMenu.Items.Add(delete);

					System.Windows.Controls.MenuItem rename = new System.Windows.Controls.MenuItem();
					rename.Header = "Переименовать";
					rename.Click += EventsForContextMenuTreeView.Rename_Click;
					docMenu.Items.Add(rename);
					item.ContextMenuOpening += EventsForContextMenuTreeView.Item_ContextMenuOpening;
					item.ContextMenu = docMenu;
					break;
			}
		}

		public static void setContextMenuForTreeView()
		{
			if (CurrentDataFileSystem.alianceIdPolicy[
					CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item2.Equals("0"))
			{
				System.Windows.Controls.ContextMenu docMenu = new System.Windows.Controls.ContextMenu();
				System.Windows.Controls.MenuItem createDir = new System.Windows.Controls.MenuItem();
				createDir.Header = "Создать новую папку";
				createDir.Click += EventsForContextMenuTreeView.CreateCategory_Click;
				System.Windows.Controls.MenuItem upload = new System.Windows.Controls.MenuItem();
				upload.Header = "Загрузить файл";
				upload.Click += EventsForContextMenuTreeView.UploadCategory_Click;
				docMenu.Items.Add(upload);
				docMenu.Items.Add(createDir);

				CurrentDataFileSystem.treeViewCatalog.ContextMenu = docMenu;
			}
			if (CurrentDataFileSystem.alianceIdPolicy[CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item2.Equals("1"))
			{
				System.Windows.Controls.ContextMenu docMenu = new System.Windows.Controls.ContextMenu();
				System.Windows.Controls.MenuItem createDir = new System.Windows.Controls.MenuItem();
				createDir.Header = "Создать новую категорию";
				createDir.Click += EventsForContextMenuTreeView.CreateCategory_Click;
				docMenu.Items.Add(createDir);

				CurrentDataFileSystem.treeViewCatalog.ContextMenu = docMenu;
			}
		}
	}
}
