using contact_center_application.core;
using contact_center_application.core.serialization;
using contact_center_application.core.storage_dynamic_data;
using contact_center_application.graphic_user_interface.manage_graphical_component.viewer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace contact_center_application.graphic_user_interface.manage_graphical_component.tree_view
{
	class ProcessTreeViewItem
	{
		public static TreeViewItem CreateTreeViewItem(string name, string icon)
		{
			TreeViewItem item = new TreeViewItem();
			StackPanel stack = new StackPanel();
			stack.Orientation = Orientation.Horizontal;
			System.Windows.Controls.Image img = new System.Windows.Controls.Image();

			BitmapImage bi3 = new BitmapImage();
			bi3.BeginInit();
			bi3.UriSource = new Uri(icon, UriKind.Relative);
			bi3.EndInit();
			img.Stretch = Stretch.Fill;
			img.Source = bi3;

			Label lbl = new Label();
			lbl.Content = name;

			stack.Children.Add(img);
			

			item.Header = stack;
			return item;
		}

		public static TreeViewItem getTreeViewItem(string nameFile, bool isFile)
		{
			TreeViewItem newItem = new TreeViewItem();
			newItem.Header = createTextBlockForObjectFromFileSystem(nameFile, isFile);
			return newItem;
		}

		public static Image getImageOnNameFile(string nameFile, bool isFile)
		{
			string path = "";
			string extension = Path.GetExtension(nameFile);
			if (isFile)
			{
				if (extension.Equals(".doc"))
				{
					path = @"resources/doc.png";
				}
				else if (extension.Equals(".docx"))
				{
					path = @"resources/docx.png";
				}
				else if (extension.Equals(".xlsx"))
				{
					path = @"resources/xlsx.png";
				}
				else if (extension.Equals(".xls"))
				{
					path = @"resources/xls.png";
				}
				else if (extension.Equals(".html") || extension.Equals(".htm"))
				{
					path = @"resources/html.png";
				}
				else if (extension.Equals(".htm"))
				{
					path = @"resources/htm.png";
				}
				else if (extension.Equals(".csv"))
				{
					path = @"resources/csv.png";
				}
				else if (extension.Equals(".pdf"))
				{
					path = @"resources/pdf.png";
				}
				else if (extension.Equals(".txt"))
				{
					path = @"resources/txt.png";
				}
				else if (extension.Equals(".link") || extension.Equals(".url"))
				{
					path = @"resources/link.png";
				}
				else if (extension.Equals(".tiff") || extension.Equals(".tiff"))
				{
					path = @"resources/tiff.png";
				}
				else if (extension.Equals(".jpg") || extension.Equals(".jpeg"))
				{
					path = @"resources/jpeg.png";
				}
				else if (extension.Equals(".png"))
				{
					path = @"resources/png.png";
				}
				else
				{
					path = @"resources/unknown.png";
				}
			}
			else
			{
				path = @"resources/catalog.png";
			}

			Image tempImage = new Image();
			BitmapImage bitmapImage = new BitmapImage(new Uri(path,
				UriKind.Relative));
			tempImage.Source = bitmapImage;
			return tempImage;
		}

		public static TextBlock createTextBlockForObjectFromFileSystem(System.Windows.Controls.Label textBlock,
			string nameFile, bool isFile)
		{
			if (!isFile)
			{
				int i = 0;
			}
			string path = "";
			string extension = Path.GetExtension(nameFile);
			if (isFile)
			{
				if (extension.Equals(".doc"))
				{
					path = @"resources/doc.png";
				}
				else if (extension.Equals(".docx"))
				{
					path = @"resources/docx.png";
				}
				else if (extension.Equals(".xlsx"))
				{
					path = @"resources/xlsx.png";
				}
				else if (extension.Equals(".xls"))
				{
					path = @"resources/xls.png";
				}
				else if (extension.Equals(".html") || extension.Equals(".htm"))
				{
					path = @"resources/html.png";
				}
				else if (extension.Equals(".htm"))
				{
					path = @"resources/htm.png";
				}
				else if (extension.Equals(".csv"))
				{
					path = @"resources/csv.png";
				}
				else if (extension.Equals(".pdf"))
				{
					path = @"resources/pdf.png";
				}
				else if (extension.Equals(".txt"))
				{
					path = @"resources/txt.png";
				}
				else if (extension.Equals(".link") || extension.Equals(".url"))
				{
					path = @"resources/link.png";
				}
				else if (extension.Equals(".tiff") || extension.Equals(".tiff"))
				{
					path = @"resources/tiff.png";
				}
				else if (extension.Equals(".jpg") || extension.Equals(".jpeg"))
				{
					path = @"resources/jpeg.png";
				}
				else if (extension.Equals(".png"))
				{
					path = @"resources/png.png";
				}
				else
				{
					path = @"resources/unknown.png";
				}
			}
			else
			{
				path = @"resources/catalog.png";
			}

			Image tempImage = new Image();
			BitmapImage bitmapImage = new BitmapImage(new Uri(path,
				UriKind.Relative));
			tempImage.Source = bitmapImage;


			TextBlock tempTextBlock = new TextBlock();
			tempTextBlock.Inlines.Add(tempImage);
			if (extension.Equals(".link") || extension.Equals(".url"))
			{
				tempTextBlock.Foreground = Brushes.Blue;
				tempTextBlock.TextDecorations = TextDecorations.Underline; ;
				tempTextBlock.Inlines.Add("  " + Path.GetFileNameWithoutExtension(nameFile));
			}
			else
			{
				tempTextBlock.Inlines.Add("  ");
				tempTextBlock.Inlines.Add(textBlock);
			}

			return tempTextBlock;
		}

		public static TextBlock createTextBlockForObjectFromFileSystem(string nameFile, bool isFile)
		{
			string path = "";
			string extension = Path.GetExtension(nameFile);
			if (isFile)
			{

				if (extension.Equals(".doc"))
				{
					path = @"resources/doc.png";
				}
				else if (extension.Equals(".docx"))
				{
					path = @"resources/docx.png";
				}
				else if (extension.Equals(".xlsx"))
				{
					path = @"resources/xlsx.png";
				}
				else if (extension.Equals(".xls"))
				{
					path = @"resources/xls.png";
				}
				else if (extension.Equals(".html") || extension.Equals(".htm"))
				{
					path = @"resources/html.png";
				}
				else if (extension.Equals(".htm"))
				{
					path = @"resources/htm.png";
				}
				else if (extension.Equals(".csv"))
				{
					path = @"resources/csv.png";
				}
				else if (extension.Equals(".pdf"))
				{
					path = @"resources/pdf.png";
				}
				else if (extension.Equals(".txt"))
				{
					path = @"resources/txt.png";
				}
				else if (extension.Equals(".link") || extension.Equals(".url"))
				{
					path = @"resources/link.png";
				}
				else if (extension.Equals(".tiff") || extension.Equals(".tiff"))
				{
					path = @"resources/tiff.png";
				}
				else if (extension.Equals(".jpg") || extension.Equals(".jpeg"))
				{
					path = @"resources/jpeg.png";
				}
				else if (extension.Equals(".png"))
				{
					path = @"resources/png.png";
				}
				else
				{
					path = @"resources/unknown.png";
				}
			}
			else
			{
				path = @"resources/catalog.png";
			}

			Image tempImage = new Image();
			BitmapImage bitmapImage = new BitmapImage(new Uri(path,
				UriKind.Relative));
			tempImage.Source = bitmapImage;


			TextBlock tempTextBlock = new TextBlock();
			tempTextBlock.Inlines.Add(tempImage);
			if (extension.Equals(".link") || extension.Equals(".url"))
			{
				tempTextBlock.Foreground = Brushes.Blue;
				tempTextBlock.TextDecorations = TextDecorations.Underline; ;
				tempTextBlock.Inlines.Add("  " + Path.GetFileNameWithoutExtension(nameFile));
			}
			else
			{
				tempTextBlock.Inlines.Add("  " + nameFile);
			}
			return tempTextBlock;
		}

		public static TreeViewItem getSearchItemOnCurrentWay(string currentWay)
		{
			TreeViewItem result = null;
			foreach (var elem in CurrentDataFileSystem.listTreeView)
			{
				if (elem.Value.Item2.Equals(currentWay))
				{
					return elem.Key;
				}
			}
			return result;
		}

		public static TreeViewItem getTreeViewItemForFile(CatalogForSerialization element, 
			string currentWay, int deep = 1)
		{
			TreeViewItem item = ProcessTreeViewItem.getSearchItemOnCurrentWay(currentWay + "\\" + element.name);
			if (item == null)
			{
				item = ProcessTreeViewItem.getTreeViewItem(element.name, true);
				CurrentDataFileSystem.listTreeView.Add(item, new Tuple<bool, string, bool>(element.file,
	currentWay + "\\" + element.name, true));
				ContextMenuForTreeView.setContextMenuToTreeViewItemFile(item, element);
			}
			else
			{
				CurrentDataFileSystem.listTreeView[item] = new Tuple<bool, string, bool>
				(element.file, currentWay + "\\" + element.name, true);
			}

			return item;
		}

		/// <summary>
		/// Получение 
		/// </summary>
		/// <param name="element"></param>
		/// <param name="currentWay"></param>
		/// <returns></returns>
		public static TreeViewItem getTreeViewItemForCatalog(
			CatalogForSerialization element, string currentWay,
			int deep = 1)
		{
			TreeViewItem item = ProcessTreeViewItem.getSearchItemOnCurrentWay(currentWay + "\\" + element.name);
			if (item != null)
			{
				item.Items.Clear();
				CurrentDataFileSystem.listTreeView[item] = new Tuple<bool, string, bool>
				(element.file, currentWay + "\\" + element.name, true);
			}
			else
			{
				item = ProcessTreeViewItem.getTreeViewItem(element.name, false);
				CurrentDataFileSystem.listTreeView.Add(item, new Tuple<bool, string, bool>
				(element.file, currentWay + "\\" + element.name, true));
			}

			List<TreeViewItem> listChildren =
				getItemsCatalogsFromJson(element.content,
					currentWay + "\\" + element.name, ++deep);
			if (listChildren.Count != 0)
			{
				foreach (var children in listChildren)
				{
					item.Items.Add(children);
				}
			}

			ContextMenuForTreeView.setContextMenuToTreeViewItemCatalog(item, element, deep);

			return item;
		}

		/// <summary>
		/// Десериализует json - строку в дерево treeView, 
		/// с указанием их относительного пути
		/// </summary>
		/// <param name="json"></param>
		/// <param name="currentWay"></param>
		/// <returns></returns>
		public static List<TreeViewItem> getItemsCatalogsFromJson(string json, string currentWay, int deep = 0)
		{
			List<TreeViewItem> result = new List<TreeViewItem>();
			List<string> listCatalog =
				   JsonConvert.DeserializeObject<List<string>>(json);
			foreach (var catalog in listCatalog)
			{
				CatalogForSerialization element =
					JsonConvert.DeserializeObject<CatalogForSerialization>(catalog);
				TreeViewItem item;
				if (!element.file)
				{
					item = ProcessTreeViewItem.getTreeViewItemForCatalog(element, currentWay, deep);
				}
				else
				{
					item = ProcessTreeViewItem.getTreeViewItemForFile(element, currentWay);
				}

				result.Add(item);
			}
			return result;
		}

		private void resetFlagsInTreeViewItem()
		{
			var temporaryListTreeView = new Dictionary<TreeViewItem, Tuple<bool, string, bool>>();
			foreach (var item in CurrentDataFileSystem.listTreeView)
			{
				temporaryListTreeView.Add(item.Key, new Tuple<bool, string, bool>(
					item.Value.Item1, item.Value.Item2, false));
			}
			CurrentDataFileSystem.listTreeView = temporaryListTreeView;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="json"></param>
		public static void fullingTreeView(string json)
		{
			if (!ManagerViewer.updateCatalog)
			{
				CurrentDataFileSystem.listTreeView.Clear();
			}
			ContextMenuForTreeView.setContextMenuForTreeViewCatalog();

			CurrentDataFileSystem.basisListItems = ProcessTreeViewItem.getItemsCatalogsFromJson(json, "");

			if (CurrentDataFileSystem.basisListItems.Count != 0)
			{
				CurrentDataFileSystem.treeViewCatalog.Items.Clear();
				foreach (var category in CurrentDataFileSystem.basisListItems)
				{
					CurrentDataFileSystem.treeViewCatalog.Items.Add(category);
				}
			}
		}
	}
}
