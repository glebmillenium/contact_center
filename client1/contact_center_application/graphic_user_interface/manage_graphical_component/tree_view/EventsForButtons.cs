using contact_center_application.core.storage_dynamic_data;
using contact_center_application.graphic_user_interface.manage_graphical_component.viewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace contact_center_application.graphic_user_interface.manage_graphical_component.tree_view
{
	class EventsForButtons
	{
		public static void ButtonUpdateCatalogs_Click(object sender, RoutedEventArgs e)
		{
			ManagerViewer.updateCatalog = true;
			TreeViewItem selectedItem = CurrentDataFileSystem.searchSelectedItem().Item2;
			FilterTreeViewItem.resetFlagsInTreeViewItem();
			CurrentDataFileSystem.getContentFileSystem();
			CurrentDataFileSystem.deleteNotNeedItemsInTreeViewItem(selectedItem);
			selectedItem.IsSelected = true;
			ManagerViewer.updateCatalog = false;
		}
	}
}
