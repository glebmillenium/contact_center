﻿using contact_center_application.core;
using contact_center_application.core.storage_dynamic_data;
using System;
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
	}
}
