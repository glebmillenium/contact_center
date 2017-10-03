using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using contact_center_application.core;
using Newtonsoft.Json;
using contact_center_application.serialization;
using System;
using System.IO;
using System.Text;
using Spire.Doc;
using System.Diagnostics;
using System.Windows.Xps.Packaging;

namespace contact_center_application
{

	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string openFile = "";
		private Dictionary<TreeViewItem, string> listTreeView = new Dictionary<TreeViewItem, string>();
		private Dictionary<string, string> alianceAndId= new Dictionary<string, string>();
		public MainWindow()
		{
			System.Drawing.Bitmap bitmap = contact_center_application.Properties.Resources.img_refresh;
			InitializeComponent();
			string[] aliance = RequestDataFromServer.primaryExchangeWithSocket();
			alianceAndId.Clear();
			for (int i = 0; i < aliance.Length; i++)
			{
				string[] item = JsonConvert.DeserializeObject<string[]>(aliance[i]);
				if (item.Length == 2)
				{
					this.alianceAndId.Add(item[1], item[0]);
					ComboboxFileSystem.Items.Add(item[1]);
				}
			}
			ComboboxFileSystem.SelectedItem = JsonConvert.DeserializeObject<string[]>(aliance[0])[1];
			string selected = ComboboxFileSystem.SelectedItem.ToString();
			int index = Int32.Parse(this.alianceAndId[ComboboxFileSystem.SelectedItem.ToString()]);

			string answer = 
				RequestDataFromServer.getCatalogFileSystem(index.ToString());
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
				string selected = ComboboxFileSystem.SelectedItem.ToString();
				int index = Int32.Parse(this.alianceAndId[ComboboxFileSystem.SelectedItem.ToString()]);
				byte[] contentFile = RequestDataFromServer.getContentFile(
					index.ToString(),
					relativeWay);
				//contentFile = contentFile.Remove(contentFile.Length - 1, 1);

				
				string temporaryRelativeWay = "tmp" + relativeWay;
				writeToFile(temporaryRelativeWay, contentFile);
				this.openFile = temporaryRelativeWay;
				LoadToViewer(temporaryRelativeWay);
			}
		}

		private void LoadToViewer(string way, string viewWay = "view/temp")
		{
			string extension = Path.GetExtension(way);
			//			XpsDocument doc = new XpsDocument(way, FileAccess.Read);
			//			viewer.Document = doc.GetFixedDocumentSequence();
			
			if (extension.Equals(".txt"))
			{
				
				//richTextBox.Document.Blocks.Clear();
				//richTextBox.Document.Blocks.Add(new Paragraph(new Run(contentFile)));
			}
			else if (extension.Equals(".doc") || extension.Equals(".docx"))
			{
				convertDocxDocToXps(way, viewWay);
				XpsDocument doc = new XpsDocument(viewWay, FileAccess.Read);
				viewer.Document = doc.GetFixedDocumentSequence();
			}
			else if (extension.Equals(".xlsx"))
			{

			}
		}

		private static void writeToFile(string relativeWay, byte[] contentFile)
		{
			string directoryWay = Path.GetDirectoryName(relativeWay);
			Directory.CreateDirectory(directoryWay);
			File.WriteAllBytes(@relativeWay, contentFile);
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

		public static void convertDocxDocToXps(string way, string viewWay)
		{
			try
			{
				Document doc = new Document(way);
				string directoryWay = Path.GetDirectoryName(viewWay);
				Directory.CreateDirectory(directoryWay);
				doc.SaveToFile(viewWay, Spire.Doc.FileFormat.XPS);
			}
			catch (System.OutOfMemoryException e)
			{

			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(this.openFile);
		}
	}
}
