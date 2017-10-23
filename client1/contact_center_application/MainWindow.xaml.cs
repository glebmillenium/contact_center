using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using contact_center_application.core;
using Newtonsoft.Json;
using contact_center_application.serialization;
using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Xps.Packaging;
using System.Threading;
using Spire.Doc;
using Spire.Xls;
using contact_center_application.form;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Documents;
using System.Text;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Drawing;

namespace contact_center_application
{
	/// <summary>
	/// Класс, формирующий графический интерфейс отображения содержимое файловой системы.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// openFile - хранит путь открытого файла в приложении
		/// </summary>
		string openFile = "";

		/// <summary>
		/// listTreeView - словарь (хэш-таблица), хранит относительный путь в 
		///				файловой системе, включая имя самого объекта, и значения всех TreeView
		/// 
		/// @param Dictionary<TreeView, string>
		/// </summary>
		private Dictionary<TreeViewItem, string> listTreeView =
											new Dictionary<TreeViewItem, string>();

		/// <summary>
		/// alianceAndId - словарь (хэш-таблица), хранит название файловой системы и его уникальный идентификатор
		/// </summary>
		private Dictionary<string, string> alianceAndId = new Dictionary<string, string>();
		XpsDocument doc;
		BitmapImage bitmap;

		/// <summary>
		/// Конструктор, осуществляет чистку папки временных файлов
		/// </summary>
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
			thread.Start(uiContext);
		}

		public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
					{
						yield return (T)child;
					}

					foreach (T childOfChild in FindVisualChildren<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}

		/// <summary>
		/// Функция для запуска первичной инициализации графического интерфейса в дочернем треде
		/// </summary>
		/// <param name="state"></param>
		private void Run(object state)
		{
			SynchronizationContext uiContext = state as SynchronizationContext;
			uiContext.Post(firstExchangeWithServer, "");
		}

		/// <summary>
		/// Получает содержимое, выбранной файловой системы
		/// </summary>
		private void getContentFileSystem()
		{
			int index = Int32.Parse(this.alianceAndId[ComboboxFileSystem.SelectedItem.ToString()]);
			string answer =
				RequestDataFromServer.getCatalog(index.ToString());
			fullingTreeView(answer);
			buttonRefresh.Visibility = Visibility.Hidden;
		}

		/// <summary>
		/// Метод запускается при включении приложения
		/// </summary>
		/// <param name="state"></param>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="json"></param>
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

		/// <summary>
		/// Десериализует json - строку в дерево treeView, 
		/// с указанием их относительного пути
		/// </summary>
		/// <param name="json"></param>
		/// <param name="currentWay"></param>
		/// <returns></returns>
		private List<TreeViewItem> getItemsCatalogsFromJson(string json, string currentWay)
		{
			List<TreeViewItem> result = new List<TreeViewItem>();
			List<string> listCatalog =
				   JsonConvert.DeserializeObject<List<string>>(json);
			foreach (var catalog in listCatalog)
			{
				System.Windows.Controls.ContextMenu docMenu = new System.Windows.Controls.ContextMenu();
				CatalogForSerialization element =
					JsonConvert.DeserializeObject<CatalogForSerialization>(catalog);

				TreeViewItem item;

				if (!element.file)
				{
					item = UsersTreeViewItem.getTreeViewItem(element.name, false);
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

					System.Windows.Controls.Button upload = new System.Windows.Controls.Button();
					upload.Content = "Загрузить";
					upload.Click += Upload_Click;
					docMenu.Items.Add(upload);

					System.Windows.Controls.Label delete = new System.Windows.Controls.Label();
					delete.Content = "Удалить папку";
					delete.KeyUp += Delete_KeyUp;
					docMenu.Items.Add(delete);
				}
				else
				{
					item = UsersTreeViewItem.getTreeViewItem(element.name, true);
					item.MouseDoubleClick += this.selectFile;
					this.listTreeView.Add(item, currentWay + "\\" + element.name);

					System.Windows.Controls.Label open = new System.Windows.Controls.Label();
					open.KeyUp += Open_KeyUp;
					open.Content = "Открыть файл";
					docMenu.Items.Add(open);

					System.Windows.Controls.Label delete = new System.Windows.Controls.Label();
					delete.Content = "Удалить файл";
					delete.KeyUp += Delete_KeyUp;
					docMenu.Items.Add(delete);
				}

				System.Windows.Controls.Label rename = new System.Windows.Controls.Label();
				rename.Content = "Переименовать";
				rename.KeyUp += Rename_KeyUp;
				docMenu.Items.Add(rename);

				item.ContextMenu = docMenu;

				result.Add(item);
			}
			return result;
		}

		private void Upload_Click(object sender, EventArgs e)
		{
			OpenFileDialog OPF = new OpenFileDialog();
			OPF.Filter = "Файлы txt|*.txt|Файлы csv|*.csv|Файлы doc|*.doc|Файлы docx|*.docx|Файлы xls|*.xls|Файлы xlsx|*.xlsx|Файлы tiff|*.tiff";
			if (OPF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				System.Windows.MessageBox.Show(OPF.FileName);
			}
		}

		private void Open_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			Tuple<bool, TreeViewItem> selectedItem = searchSelectedItem();
			if (selectedItem.Item1)
			{
				TreeViewItem item = selectedItem.Item2;
			}
		}

		private void Rename_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{

		}

		private void Upload_KeyUp()
		{
			OpenFileDialog OPF = new OpenFileDialog();
			OPF.Filter = "Файлы txt|*.txt|Файлы csv|*.csv|Файлы doc|*.doc|Файлы docx|*.docx|Файлы xls|*.xls|Файлы xlsx|*.xlsx|Файлы tiff|*.tiff";
			if (OPF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				System.Windows.MessageBox.Show(OPF.FileName);
			}
		}

		private void Delete_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{

		}

		/// <summary>
		/// Обработчик события, вызывается по изменению ComboBox.
		/// По указанному combobox получает содержимое файловой системы
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ComboboxFileSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			getContentFileSystem();
		}

		/// <summary>
		/// Обработчик события, запускается по двойному щелчку мыши по treeView. Запускает 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
				DownloadWindow download = new DownloadWindow(index.ToString(),
					relativeWay);
				try
				{
					download.getContentFileAndWriteToFile(this.openFile);

					//writeToFile(this.openFile, contentFile);
					//Array.Clear(contentFile, 0, contentFile.Length);
					LoadToViewer(this.openFile);
				}
				catch(Exception exp)
				{

				}

			}
		}

		/// <summary>
		/// Загружает данные в DocumentViewer viewer
		/// </summary>
		/// <param name="way"></param>
		/// <param name="viewWay"></param>
		private void LoadToViewer(string way, string viewWay = "view/temp")
		{
			string extension = Path.GetExtension(way);

			if (extension.Equals(".txt") || extension.Equals(".csv"))
			{
				this.tabControl.SelectedItem = this.textboxTab;
				displayTextbox(way);
			}
			else if (extension.Equals(".jpeg") || extension.Equals(".tiff") || extension.Equals(".jpg"))
			{
				bitmap = new BitmapImage();
				string fullWay = Path.Combine(Path.GetDirectoryName(
								Assembly.GetExecutingAssembly().Locati‌​on), way);
				bitmap.BeginInit();
				bitmap.UriSource = new Uri(fullWay);
				bitmap.EndInit();
				bitmap.Freeze();
				image.Source = bitmap;

				tabControl.SelectedItem = imageTab;
			}
			else if (extension.Equals(".doc") || extension.Equals(".docx"))
			{
				if ((bool)swtichModeView.IsChecked)
				{
					this.tabControl.SelectedItem = this.viewerTab;
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
				else
				{
					this.tabControl.SelectedItem = this.textboxTab;
					displayTextbox(way);
				}
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

		private void displayTextbox(string viewWay)
		{
			textbox.Document.Blocks.Clear();
			var sr = new StreamReader(viewWay, Encoding.UTF8);
			string text = sr.ReadToEnd();

			var document = new FlowDocument();
			var paragraph = new Paragraph();
			paragraph.Inlines.Add(text);
			document.Blocks.Add(paragraph);
			textbox.Document = document;
		}

		/// <summary>
		/// Функция осуществляет побайтовую запись в файл
		/// </summary>
		/// <param name="relativeWay"></param>
		/// <param name="contentFile"></param>
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

		/// <summary>
		/// Поиск и возврат элемента TreeViewItem, который в текущий момент выделен
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="way"></param>
		/// <param name="viewWay"></param>
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

		/// <summary>
		/// Функция конвертирует из XLS формата в XPS формат для дальнейшего показа
		/// 
		/// </summary>
		/// <param name="way"></param>
		/// <param name="viewWay"></param>
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

		/// <summary>
		/// Запуск загрузки содержимого файла с сервера и открытия в documentViewer либо 
		/// textReach
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(this.openFile);
		}

		/// <summary>
		/// Получение содержимого файловой системы, вызывается при нажатии
		/// на кнопку обновление каталогов
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonUpdateCatalogs_Click(object sender, RoutedEventArgs e)
		{
			getContentFileSystem();
		}

		/// <summary>
		/// buttonRefresh_Click
		/// Запуск повторного подключения к серверу
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonRefresh_Click(object sender, RoutedEventArgs e)
		{
			SynchronizationContext uiContext = SynchronizationContext.Current;
			Thread thread = new Thread(Run);
			thread.Start(uiContext);
		}

		/// <summary>
		/// Window_SizeChanged
		/// Событие вызывается при изменении окна
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			viewer.Height = window.ActualHeight - 200;
		}

		/// <summary>
		/// CheckBox_Click
		/// Обновление графического интерфейса осуществляющего показ содердимого документа
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if ((bool) swtichModeView.IsChecked)
			{
				swtichModeView.Content = "Нагруженный интерфейс";
				this.tabControl.SelectedItem = this.viewerTab;
			}
			else
			{
				swtichModeView.Content = "Простой интерфейс";
				this.tabControl.SelectedItem = this.textboxTab;
			}
		}
	}
}
