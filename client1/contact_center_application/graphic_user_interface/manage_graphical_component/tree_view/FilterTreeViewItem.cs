using contact_center_application.core.storage_dynamic_data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace contact_center_application.graphic_user_interface.manage_graphical_component.tree_view
{
	class FilterTreeViewItem
	{
		public static void setVisibleOnText(string text)
		{
			if (text.Length == 0)
			{
				hiddenTreeView();
			}
			else
			{
				List<TreeViewItem> newListItems = new List<TreeViewItem>();
				foreach (TreeViewItem elem in CurrentDataFileSystem.basisListItems)
				{
					setVisibleOnTextForTreeView(elem, text);
				}
			}
		}

		private static void hiddenTreeView()
		{
			foreach (var item in CurrentDataFileSystem.listTreeView)
			{
				item.Key.IsExpanded = false;
				item.Key.Visibility = Visibility.Visible;
				string relativeWay = CurrentDataFileSystem.listTreeView[item.Key].Item2;
				string nameElement = Path.GetFileName(relativeWay);
				item.Key.Header = highlightText(nameElement, "", 
					CurrentDataFileSystem.listTreeView[item.Key].Item1);
			}
		}

		private static bool setVisibleOnTextForTreeView(TreeViewItem item, string text, bool mandatory = false)
		{
			string relativeWay = CurrentDataFileSystem.listTreeView[item].Item2;
			string nameElement = Path.GetFileName(relativeWay);
			if (mandatory)
			{
				if (item.Items.Count > 0)
				{
					foreach (TreeViewItem elem in item.Items)
					{
						elem.Visibility = Visibility.Visible;
						string temp1 = CurrentDataFileSystem.listTreeView[elem].Item2;
						string nameElement1 = Path.GetFileName(temp1);
						elem.Header = highlightText(nameElement1, text, CurrentDataFileSystem.listTreeView[elem].Item1);
					}
				}
				item.Header = highlightText(nameElement, text, CurrentDataFileSystem.listTreeView[item].Item1);
				item.Visibility = Visibility.Visible;

				if ((bool) MainWindowElement.openFolders.IsChecked)
				{
					item.IsExpanded = true;
				}

				return true;
			}
			else
			{
				if (item.Items.Count > 0)
				{
					int resultSearch;
					resultSearch = relativeWay.IndexOf(text);
					if (resultSearch > -1)
					{
						item.Visibility = Visibility.Visible;
						bool typeTreeView = CurrentDataFileSystem.listTreeView.ContainsKey(item);
						item.Header = highlightText(nameElement, text, CurrentDataFileSystem.listTreeView[item].Item1);
						foreach (TreeViewItem elem in item.Items)
						{
							elem.Visibility = Visibility.Visible;
							string temp1 = CurrentDataFileSystem.listTreeView[elem].Item2;
							string nameElement1 = Path.GetFileName(temp1);
							elem.Header = highlightText(nameElement1, text, CurrentDataFileSystem.listTreeView[elem].Item1);
							setVisibleOnTextForTreeView(elem, text, true);
						}

						if ((bool)MainWindowElement.openFolders.IsChecked)
						{
							item.IsExpanded = true;
						}

						return true;
					}
					else
					{
						int changeVisible = 0;
						foreach (TreeViewItem elem in item.Items)
						{
							bool res = setVisibleOnTextForTreeView(elem, text);
							if (res)
							{
								elem.Visibility = Visibility.Visible;

								string temp1 = CurrentDataFileSystem.listTreeView[elem].Item2;
								string nameElement1 = Path.GetFileName(temp1);
								elem.Header = highlightText(nameElement1, text, CurrentDataFileSystem.listTreeView[elem].Item1);

								changeVisible++;
							}
							else
							{
								elem.Visibility = Visibility.Collapsed;
							}
						}

						if (changeVisible > 0)
						{
							item.Header = highlightText(nameElement, text, CurrentDataFileSystem.listTreeView[item].Item1);
							item.Visibility = Visibility.Visible;

							if ((bool)MainWindowElement.openFolders.IsChecked)
							{
								item.IsExpanded = true;
							}

							return true;
						}
						else
						{
							item.Visibility = Visibility.Collapsed;
							return false;
						}
					}
				}
				else
				{
					int resultSearch;
					if ((bool)MainWindowElement.registrButton.IsChecked)
					{
						resultSearch = relativeWay.IndexOf(text);
					}
					else
					{
						resultSearch = relativeWay.ToLower().IndexOf(text.ToLower());
					}
					if (resultSearch > -1)
					{
						item.Header = highlightText(nameElement, text, CurrentDataFileSystem.listTreeView[item].Item1);
						item.Visibility = Visibility.Visible;

						if ((bool)MainWindowElement.openFolders.IsChecked)
						{
							item.IsExpanded = true;
						}

						return true;
					}
					else
					{
						item.Visibility = Visibility.Collapsed;
						return false;
					}
				}
			}
		}

		private static TextBlock highlightText(string source, string substring, bool isFile)
		{
			TextBlock result = new TextBlock();
			result.Inlines.Add(ProcessTreeViewItem.getImageOnNameFile(Path.GetFileName(source), isFile));
			result.Inlines.Add("  ");
			if (substring.Length != 0)
			{
				var indices = new List<int>();
				int index;
				if ((bool)MainWindowElement.registrButton.IsChecked)
					index = source.IndexOf(substring, 0);
				else
					index = source.ToLower().IndexOf(substring.ToLower(), 0);
				while (index > -1)
				{
					indices.Add(index);
					if ((bool)MainWindowElement.registrButton.IsChecked)
						index = source.IndexOf(substring, index + substring.Length);
					else
						index = source.ToLower().IndexOf(substring.ToLower(),
							index + substring.Length);
				}

				if (indices.Count > 0)
				{
					int lastSymbol = 0;

					string notCorrectiveName = "";
					int currentPosition = 0;
					int i = 0;

					do
					{
						int position = indices[i];
						if (position > currentPosition)
						{
							notCorrectiveName = source.Substring(currentPosition, position - currentPosition);
						}
						else
						{
							notCorrectiveName = "";
						}

						TextBlock correctiveName = new TextBlock();
						correctiveName.Inlines.Add(source.Substring(position, substring.Length));
						correctiveName.Foreground = Brushes.BlueViolet;
						correctiveName.Background = Brushes.BurlyWood;
						result.Inlines.Add(notCorrectiveName);
						result.Inlines.Add(correctiveName);

						currentPosition = position + substring.Length;
						i++;
					} while (i < indices.Count);
					if (currentPosition < source.Length)
					{
						result.Inlines.Add(source.Substring(currentPosition, source.Length - currentPosition));
					}
				}
				else
				{
					result.Inlines.Add(source);
				}
			}
			else
			{
				result.Inlines.Add(source);
			}
			return result;
		}

		/// <summary>
		/// Обновить содержимое listTreeView. Вызывается, при обновлении данных с каталоговой системы
		/// </summary>
		public static void resetFlagsInTreeViewItem()
		{
			var temporaryListTreeView = new Dictionary<TreeViewItem, Tuple<bool, string, bool>>();
			foreach (var item in CurrentDataFileSystem.listTreeView)
			{
				temporaryListTreeView.Add(item.Key, new Tuple<bool, string, bool>(
					item.Value.Item1, item.Value.Item2, false));
			}
			CurrentDataFileSystem.listTreeView = temporaryListTreeView;
		}
	}
}
