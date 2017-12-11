using contact_center_application.graphic_user_interface.manage_graphical_component.tree_view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace contact_center_application.core.storage_dynamic_data
{
	class CurrentDataFileSystem
	{
		public static List<TreeViewItem> basisListItems = new List<TreeViewItem>();

		/// <summary>
		/// listTreeView - словарь (хэш-таблица), хранит относительный путь в 
		///				файловой системе, включая имя самого объекта, и значения всех TreeView
		/// 
		/// @param Dictionary<TreeView, Tuple<bool, string, bool>> - графический элемент; 
		///						тип объекта, который отображает элемент - файл - true, или каталог - false;
		///						string - относительный путь файла в системе;
		///						bool - флаг обновления 
		/// </summary>
		public static Dictionary<TreeViewItem, Tuple<bool, string, bool>> listTreeView =
											new Dictionary<TreeViewItem, Tuple<bool, string, bool>>();

		/// <summary>
		/// alianceAndId - словарь (хэш-таблица), хранит название файловой системы и его 
		/// уникальный идентификатор
		/// </summary>
		public static Dictionary<string, Tuple<string, string>> alianceIdPolicy =
											new Dictionary<string, Tuple<string, string>>();
		public static ComboBox ComboboxFileSystem;

		public static TreeView treeViewCatalog;

		/// <summary>
		/// Поиск и возврат элемента TreeViewItem, который в текущий момент выделен
		/// </summary>
		/// <returns></returns>
		public static Tuple<bool, TreeViewItem> searchSelectedItem()
		{
			foreach (var item in CurrentDataFileSystem.listTreeView)
			{
				if (item.Key.IsSelected)
				{
					return new Tuple<bool, TreeViewItem>(true, item.Key);
				}
			}
			return new Tuple<bool, TreeViewItem>(false, new TreeViewItem());
		}

		public static void deleteNotNeedItemsInTreeViewItem(TreeViewItem equalItem)
		{
			var temporaryListTreeView = new Dictionary<TreeViewItem, Tuple<bool, string, bool>>();

			foreach (var item in CurrentDataFileSystem.listTreeView)
			{
				if (item.Value.Item3 == true)
				{
					temporaryListTreeView.Add(item.Key, new Tuple<bool, string, bool>(
						item.Value.Item1, item.Value.Item2, item.Value.Item3));
				}
			}
			CurrentDataFileSystem.listTreeView = temporaryListTreeView;
		}

		/// <summary>
		/// Получает содержимое, выбранной файловой системы
		/// </summary>
		public static void getContentFileSystem()
		{
			try
			{
				int index = Int32.Parse(
					CurrentDataFileSystem.alianceIdPolicy[
						CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1);
				string answer =
					RequestDataFromServer.getCatalog(index.ToString());
				ProcessTreeViewItem.fullingTreeView(answer);
				MainWindowElement.buttonRefresh.Visibility = Visibility.Hidden;
			}
			catch (System.NullReferenceException e)
			{
				Logger.log(e.Message);
			}
		}
	}
}
