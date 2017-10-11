using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using contact_center_application.core;
using Newtonsoft.Json;
using contact_center_application.serialization;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Xps.Packaging;
using System.Threading;
using Spire.Doc;
using Spire.Xls;
using System.Windows.Forms;

namespace contact_center_application
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string openFile = "";
		private Dictionary<TreeViewItem, string> listTreeView =
			new Dictionary<TreeViewItem, string>();
		private Dictionary<string, string> alianceAndId = new Dictionary<string, string>();
		XpsDocument doc;
		public MainWindow()
		{
			if (Directory.Exists("tmp"))
			{
				Directory.Delete("tmp", true);
				Directory.CreateDirectory("tmp");
			}

			InitializeComponent();
			SynchronizationContext uiContext = SynchronizationContext.Current;
			Thread thread = new Thread(Run);
			// Запустим поток и установим ему контекст синхронизации,
			// таким образом этот поток сможет обновлять UI
			thread.Start(uiContext);

			//firstExchangeWithServer();
		}

		private void Run(object state)
		{
			// вытащим контекст синхронизации из state'а
			SynchronizationContext uiContext = state as SynchronizationContext;
			// говорим что в UI потоке нужно выполнить метод UpdateUI 
			// и передать ему в качестве аргумента строку
			uiContext.Post(firstExchangeWithServer, "");
		}

		private void getContentFileSystem()
		{
			int index = Int32.Parse(this.alianceAndId[ComboboxFileSystem.SelectedItem.ToString()]);

			string answer =
				RequestDataFromServer.getCatalogFileSystem(index.ToString());

			fullingTreeView(answer);
		}

		private void firstExchangeWithServer(object state)
		{
			try
			{
				string[] aliance = RequestDataFromServer.primaryExchangeWithSocket();
				alianceAndId.Clear();
				ComboboxFileSystem.Items.Clear();
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
			}
			catch (System.Net.Sockets.SocketException socketException)
			{

			}
		}

		private void fullingTreeView(string json)
		{
			this.listTreeView.Clear();
			List<TreeViewItem> listItems = getItemsCatalogsFromJson(json, "");

			if (listItems.Count != 0)
			{
				this.treeViewCatalog.Items.Clear();
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

				// Create the ContextMenuStrip.
				System.Windows.Controls.ContextMenu docMenu = new System.Windows.Controls.ContextMenu();

				//Create some menu items.
				System.Windows.Controls.Label open = new System.Windows.Controls.Label();
				open.Content = "Открыть файл";
				System.Windows.Controls.Label rename = new System.Windows.Controls.Label();
				rename.Content = "Переименовать";
				System.Windows.Controls.Label upload = new System.Windows.Controls.Label();
				upload.Content = "Загрузить";
				System.Windows.Controls.Label delete = new System.Windows.Controls.Label();
				delete.Content = "Удалить файл";

				//Add the menu items to the menu.
				docMenu.Items.Add(open);
				docMenu.Items.Add(rename);
				docMenu.Items.Add(upload);
				docMenu.Items.Add(delete);
				item.ContextMenu = docMenu;

				result.Add(item);
			}
			return result;
		}

		private void ComboboxFileSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			getContentFileSystem();
		}

		private void selectFile(object sender, RoutedEventArgs e)
		{
			Tuple<bool, TreeViewItem> selectedItem = searchSelectedItem();
			if (selectedItem.Item1)
			{
				string aliance = ComboboxFileSystem.SelectedItem.ToString();
				string relativeWay = this.listTreeView[selectedItem.Item2];
				string selected = ComboboxFileSystem.SelectedItem.ToString();
				this.openFile = "tmp" + relativeWay;

				int index = Int32.Parse(this.alianceAndId[ComboboxFileSystem.SelectedItem.ToString()]);
				//byte[] contentFile = null;

				DownloadWindow download = new DownloadWindow(index.ToString(),
					relativeWay);
				download.getContentFileAndWriteToFile(this.openFile);

				//writeToFile(this.openFile, contentFile);
				//Array.Clear(contentFile, 0, contentFile.Length);
				LoadToViewer(this.openFile);
			}
		}

		private void LoadToViewer(string way, string viewWay = "view/temp")
		{
			string extension = Path.GetExtension(way);
			//			XpsDocument doc = new XpsDocument(way, FileAccess.Read);
			//			viewer.Document = doc.GetFixedDocumentSequence();


			if (extension.Equals(".txt") || extension.Equals(".csv"))
			{
				/*
				 * string strFile = way;
				Paragraph flowParagraph = new Paragraph();
				flowParagraph.Inlines.Add(File.ReadAllText(strFile));
				FlowDocument flowDoc = new FlowDocument(flowParagraph);
				IDocumentPaginatorSource idpSource = flowDoc;
				DocumentPaginator docPaginator = idpSource.DocumentPaginator;

				//richTextBox.Document.Blocks.Clear();
				//richTextBox.Document.Blocks.Add(new Paragraph(new Run(contentFile)));
				*/
			}
			else if (extension.Equals(".doc") || extension.Equals(".docx"))
			{
				this.viewer.Document = null;
				this.viewer.DataContext = null;
				if (doc != null)
				{
					this.doc.Close();
				}

				convertDocxDocToXps(way, viewWay);
				this.doc = new XpsDocument(viewWay, FileAccess.Read);
				viewer.Document = doc.GetFixedDocumentSequence();
			}
			else if (extension.Equals(".xlsx") || extension.Equals(".xls"))
			{
				this.viewer.Document = null;
				this.viewer.DataContext = null;
				if (doc != null)
				{
					this.doc.Close();
				}
				this.doc = null;

				convertXlsToXps(way, viewWay);
				this.doc = new XpsDocument(viewWay, FileAccess.Read);
				viewer.Document = doc.GetFixedDocumentSequence();
			}
		}

		private static void te

		private static void writeToFile(string relativeWay, byte[] contentFile)
		{
			string directoryWay = Path.GetDirectoryName(relativeWay);
			Directory.CreateDirectory(directoryWay);
			if (File.Exists(relativeWay))
			{
				File.Delete(relativeWay);
			}
			File.WriteAllBytes(relativeWay, contentFile);
		}

		private Tuple<bool, TreeViewItem> searchSelectedItem()
		{
			foreach (var item in this.listTreeView)
			{
				if (item.Key.IsSelected)
				{
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
				if (File.Exists(viewWay))
				{
					File.Delete(viewWay);
				}
				doc.SaveToFile(viewWay, Spire.Doc.FileFormat.XPS);
				doc.Close();
			}
			catch (System.OutOfMemoryException e)
			{
				System.Windows.MessageBox.Show("По неясным причинам приложению не удалось отобразить требуемый документ." +
					" Тем не менее требуемый документ можно открыть в любом внешнем приложении.\n" +
					"Например: Word Office, OpenOffice, LibreOffice", "Ошибка вывода на экран");
			}
			catch (System.IO.IOException e)
			{
				System.Windows.MessageBox.Show("По неясным причинам приложению не удалось отобразить требуемый документ." +
					" Тем не менее требуемый документ можно открыть в любом внешнем приложении.\n" +
					"Например: Word Office, OpenOffice, LibreOffice", "Ошибка вывода на экран");
			}
			catch (System.ApplicationException e)
			{
				System.Windows.MessageBox.Show("По неясным причинам приложению не удалось отобразить требуемый документ." +
					" Тем не менее требуемый документ можно открыть в любом внешнем приложении.\n" +
					"Например: Word Office, OpenOffice, LibreOffice", "Ошибка вывода на экран");
			}
		}

		public static void convertXlsToXps(string way, string viewWay)
		{
			try
			{
				Workbook workbook = new Workbook();
				workbook.LoadFromFile(way, ExcelVersion.Version97to2003);
				string directoryWay = Path.GetDirectoryName(viewWay);
				Directory.CreateDirectory(directoryWay);
				if (File.Exists(viewWay))
				{
					File.Delete(viewWay);
				}
				workbook.SaveToFile(viewWay, Spire.Xls.FileFormat.XPS);
				workbook.Dispose();
				workbook = null;
			}
			catch (System.NotSupportedException e)
			{
				System.Windows.MessageBox.Show("Не удалось выполнить преобразование документа," +
					" скорее всего файл поврежден и нуждается в восстановлении");
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(this.openFile);
		}


		private void ButtonUpdateCatalogs_Click(object sender, RoutedEventArgs e)
		{
			getContentFileSystem();
		}

		private void buttonRefresh_Click(object sender, RoutedEventArgs e)
		{
			SynchronizationContext uiContext = SynchronizationContext.Current;
			Thread thread = new Thread(Run);
			// Запустим поток и установим ему контекст синхронизации,
			// таким образом этот поток сможет обновлять UI
			thread.Start(uiContext);
		}
	}
}
