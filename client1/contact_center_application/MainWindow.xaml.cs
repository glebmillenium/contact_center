using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using contact_center_application.core;
using Newtonsoft.Json;
using contact_center_application.serialization;
using System;
using System.IO;
using System.Text;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;

namespace contact_center_application
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Dictionary<TreeViewItem, string> listTreeView = new Dictionary<TreeViewItem, string>();
		public MainWindow()
		{
			InitializeComponent();
			string[] aliance = RequestDataFromServer.primaryExchangeWithSocket();
			for (int i = 0; i < aliance.Length; i++)
			{
				ComboboxFileSystem.Items.Add(aliance[i]);
			}
			ComboboxFileSystem.SelectedItem = aliance[0];

			string answer = 
				RequestDataFromServer.getCatalogFileSystem(ComboboxFileSystem.SelectedItem.ToString());
			fullingTreeView(answer);
		}

		private void fullingTreeView(string json)
		{
			this.listTreeView.Clear();
			List<TreeViewItem> listItems = getItemsCatalogsFromJson(json, "");

			if (listItems.Count != 0)
			{
				foreach (var category in listItems)
				{
					this.treeViewCatalog.Items.Add(category);
				}
			}
		}

		private List<TreeViewItem> getItemsCatalogsFromJson(string json, string currentWay)
		{
			List<TreeViewItem> result = new List<TreeViewItem>();
			List<string> listCatalog =
				   JsonConvert.DeserializeObject<List<string>>(json);
			foreach (var catalog in listCatalog)
			{
				CatalogForSerialization element =
					JsonConvert.DeserializeObject<CatalogForSerialization>(catalog);
				TreeViewItem item = new TreeViewItem();
				item.Header = element.name;
				if (!element.file)
				{
					List<TreeViewItem> listChildren =
						getItemsCatalogsFromJson(element.content, 
							currentWay + "\\" + element.name);
					if (listChildren.Count != 0)
					{
						foreach (var children in listChildren)
						{
							item.Items.Add(children);
						}
					}

				}
				else
				{
					item.Selected += this.selectFile;
					this.listTreeView.Add(item, currentWay + "\\" + element.name);
				}
				result.Add(item);
			}
			return result;
		}

		private void ComboboxFileSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void selectFile(object sender, RoutedEventArgs e)
		{
			Tuple<bool, TreeViewItem> selectedItem = searchSelectedItem();
			if (selectedItem.Item1)
			{
				string aliance = ComboboxFileSystem.SelectedItem.ToString();
				string relativeWay = this.listTreeView[selectedItem.Item2];
				string contentFile = RequestDataFromServer.getContentFile(
					aliance,
					relativeWay);
				contentFile = contentFile.Remove(contentFile.Length - 1, 1);

				
				string temporaryRelativeWay = "tmp" + relativeWay;
				writeToFile(temporaryRelativeWay, contentFile);

				LoadToViewer(temporaryRelativeWay, contentFile);
			}
		}

		private void LoadToViewer(string way, string contentFile)
		{
			string extension = Path.GetExtension(way);
			//			XpsDocument doc = new XpsDocument(way, FileAccess.Read);
			//			viewer.Document = doc.GetFixedDocumentSequence();
			if (extension.Equals(".txt"))
			{
				richTextBox.Document.Blocks.Clear();
				richTextBox.Document.Blocks.Add(new Paragraph(new Run(contentFile)));
			}
		}

		private static void writeToFile(string relativeWay, string contentFile)
		{
			string directoryWay = Path.GetDirectoryName(relativeWay);
			Directory.CreateDirectory(directoryWay);
			File.AppendAllText(@relativeWay, contentFile, Encoding.UTF8);
		}

		private Tuple<bool, TreeViewItem> searchSelectedItem()
		{
			foreach (var item in this.listTreeView)
			{
				if (item.Key.IsSelected) {
					return new Tuple<bool, TreeViewItem>(true, item.Key);
				}
			}
			return new Tuple<bool, TreeViewItem>(false, new TreeViewItem());
		}
	}
}
