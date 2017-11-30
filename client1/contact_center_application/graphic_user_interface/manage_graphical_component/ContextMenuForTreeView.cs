using contact_center_application.core;
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
