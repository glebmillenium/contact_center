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
using System.Windows.Documents;
using System.Text;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Input;
using MoonPdfLib.MuPdf;
using System.Runtime.InteropServices;
using Spire.DocViewer.Wpf;
using MoonPdfLib;
using System.Windows.Media;

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

		List<TreeViewItem> basisListItems = new List<TreeViewItem>();

		/// <summary>
		/// listTreeView - словарь (хэш-таблица), хранит относительный путь в 
		///				файловой системе, включая имя самого объекта, и значения всех TreeView
		/// 
		/// @param Dictionary<TreeView, string>
		/// </summary>
		private Dictionary<TreeViewItem, Tuple<bool, string>> listTreeView =
											new Dictionary<TreeViewItem, Tuple<bool, string>>();

		/// <summary>
		/// alianceAndId - словарь (хэш-таблица), хранит название файловой системы и его 
		/// уникальный идентификатор
		/// </summary>
		private Dictionary<string, string> alianceAndId = new Dictionary<string, string>();
		private DocumentViewer viewer = new DocumentViewer(); //viewerTab
		private MoonPdfPanel moonPdfPanel = new MoonPdfPanel(); //background light-gray
		private DocViewer docViewer = new DocViewer();
		private Image image = new Image();
		XpsDocument doc;

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

			//callGarbage();
			SynchronizationContext uiContext = SynchronizationContext.Current;
			Thread thread = new Thread(Run);
			thread.Start(uiContext);
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
				string address = "";
				try
				{
					address = File.ReadAllText(@"settings\ip_connect");
				}
				catch (Exception e)
				{
					address = "localhost";
				}
				string[] aliance = RequestDataFromServer.primaryExchangeWithSocket(address);
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
			basisListItems = getItemsCatalogsFromJson(json, "");

			if (basisListItems.Count != 0)
			{
				this.treeViewCatalog.Items.Clear();
				foreach (var category in basisListItems)
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

					System.Windows.Controls.MenuItem upload = new System.Windows.Controls.MenuItem();
					upload.Header = "Загрузить файл в папку";
					upload.Click += Upload_Click;
					docMenu.Items.Add(upload);

					System.Windows.Controls.MenuItem createDir = new System.Windows.Controls.MenuItem();
					createDir.Header = "Создать новую папку в " + element.name;
					createDir.Click += CreateDir_Click; ;
					docMenu.Items.Add(createDir);

					System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
					delete.Header = "Удалить папку";
					delete.Click += Delete_Click; ;
					docMenu.Items.Add(delete);

					this.listTreeView.Add(item, new Tuple<bool, string>
						(element.file, currentWay + "\\" + element.name));
				}
				else
				{
					item = UsersTreeViewItem.getTreeViewItem(element.name, true);
					item.MouseDoubleClick += this.selectFile;
					this.listTreeView.Add(item, new Tuple<bool, string>(element.file, 
						currentWay + "\\" + element.name));

					System.Windows.Controls.MenuItem open = new System.Windows.Controls.MenuItem();
					open.Click += Open_Click; ;
					open.Header = "Открыть файл";
					docMenu.Items.Add(open);

					System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
					delete.Header = "Удалить файл";
					delete.Click += Delete_Click;
					docMenu.Items.Add(delete);
				}

				System.Windows.Controls.MenuItem rename = new System.Windows.Controls.MenuItem();
				rename.Header = "Переименовать";
				rename.Click += Rename_Click;
				docMenu.Items.Add(rename);
				item.ContextMenuOpening += Item_ContextMenuOpening;


				item.ContextMenu = docMenu;

				result.Add(item);
			}
			return result;
		}

		private void CreateDir_Click(object sender, RoutedEventArgs e)
		{
			RenameUnitFileSystem dialog = new RenameUnitFileSystem(true);
			dialog.ShowDialog();
			if ((bool)dialog.DialogResult)
			{
				string nameDirectory = dialog.getNameFile();
				string index = this.alianceAndId[ComboboxFileSystem.SelectedItem.ToString()];
				Tuple<bool, TreeViewItem> selectedItem = searchSelectedItem();
				string relativeWay = this.listTreeView[selectedItem.Item2].Item2;
				RequestDataFromServer.sendToCreateCatalogFileSystem(index, relativeWay, nameDirectory);
			}
		}

		/// <summary>
		/// Событие вызываемое при открытии контекстного меню, осуществляет выделение treeviewitem
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Item_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			bool value = e.Handled;
			TreeViewItem item = sender as TreeViewItem;
			if (item != null)
			{
				item.Focus();
				item.ContextMenu.IsOpen = true;
				e.Handled = true;
			}
		}

		/// <summary>
		/// Событие вызываемое для переименование файла
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Rename_Click(object sender, RoutedEventArgs e)
		{
			RenameUnitFileSystem dialog = new RenameUnitFileSystem();
			dialog.ShowDialog();
			if ((bool)dialog.DialogResult)
			{
				string index = this.alianceAndId[ComboboxFileSystem.SelectedItem.ToString()];
				Tuple<bool, TreeViewItem> selectedItem = searchSelectedItem();
				string relativeWay = this.listTreeView[selectedItem.Item2].Item2;
				string nameFile = dialog.getNameFile();
				RequestDataFromServer.sendToRenameObjectFileSystem(index, relativeWay, nameFile);
			}
		}

		/// <summary>
		/// Событие вызываемое при открытии файла
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Open_Click(object sender, RoutedEventArgs e)
		{
			selectFile(null, null);
		}

		/// <summary>
		/// Событие вызываемое при запросе на удаление файла
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			string index = this.alianceAndId[ComboboxFileSystem.SelectedItem.ToString()];
			Tuple<bool, TreeViewItem> selectedItem = searchSelectedItem();
			string relativeWay = this.listTreeView[selectedItem.Item2].Item2;

			RequestDataFromServer.sendToDeleteObjectFileSystem(index, relativeWay);
		}

		/// <summary>
		/// Событие вызываемое при загрузке файла в систему
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Upload_Click(object sender, EventArgs e)
		{
			System.Windows.Forms.OpenFileDialog OPF = new System.Windows.Forms.OpenFileDialog();
			OPF.Filter = "Все документы|*.*|Файлы txt|*.txt|Файлы csv|*.csv|Файлы doc|*.doc|Файлы docx|*.docx|Файлы xls|*.xls|Файлы xlsx|*.xlsx|Файлы tiff|*.tiff";
			if (OPF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				int index = Int32.Parse(this.alianceAndId[ComboboxFileSystem.SelectedItem.ToString()]);
				Tuple<bool, TreeViewItem> selectedItem = searchSelectedItem();

				string relativeWay = this.listTreeView[selectedItem.Item2].Item2;
				UploadWindow download = new UploadWindow(index.ToString(), relativeWay,
					OPF.FileName, "0");
				try
				{
					download.sendFileToServer();
				}
				catch (Exception exp)
				{

				}
			}
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

		private bool lastLoadPicture = false;

		private void callGarbage()
		{
			image = null;
			docViewer = null;
			viewer = null;
			moonPdfPanel = null;
			doc = null;

			UpdateLayout();
			GC.Collect();
			GC.WaitForPendingFinalizers();

			viewer = new DocumentViewer(); //viewerTab
			moonPdfPanel = new MoonPdfPanel(); //background light-gray
			docViewer = new DocViewer();
			image = new Image();

			this.docViewerStackPanel.Children.Clear();
			this.docViewerStackPanel.Children.Add(this.docViewer);

			this.viewerStackPanel.Children.Clear();
			this.viewerStackPanel.Children.Add(this.viewer);

			this.pdfViewerStackPanel.Children.Clear();
			this.pdfViewerStackPanel.Children.Add(this.moonPdfPanel);

			this.iamgeStackPanel.Children.Clear();
			this.iamgeStackPanel.Children.Add(this.image);

			if (File.Exists("view/temp"))
			{
				File.Delete("view/temp");
			}

			Window_SizeChanged(null, null);
		}

		/// <summary>
		/// Обработчик события, запускается по двойному щелчку мыши по treeView. 
		/// Запускает процесс загрузки содержимого файла с удаленного сервера
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void selectFile(object sender, RoutedEventArgs e)
		{
			Tuple<bool, TreeViewItem> selectedItem = searchSelectedItem();
			if (selectedItem.Item1)
			{
				this.Cursor = Cursors.Wait;

				callGarbage();

				string aliance = ComboboxFileSystem.SelectedItem.ToString();
				string relativeWay = this.listTreeView[selectedItem.Item2].Item2;
				string selected = ComboboxFileSystem.SelectedItem.ToString();
				this.openFile = "tmp" + relativeWay;
				if (Path.GetExtension(relativeWay).Equals(".link"))
				{
					openWeb(Path.GetFileNameWithoutExtension(relativeWay));
				} else
				{
					int index = Int32.Parse(this.alianceAndId[ComboboxFileSystem.SelectedItem.ToString()]);
					DownloadWindow download = new DownloadWindow(index.ToString(),
						relativeWay);
					download.getContentFileAndWriteToFile(this.openFile);
					LoadToViewer(this.openFile);

				}
				this.Cursor = Cursors.Arrow;
			}
		}
		
		/// <summary>
		/// Метод для журналирования действий пользователя
		/// </summary>
		/// <param name="message">Сообщение которое будет отправлено пользователю</param>
		public void logger(string message)
		{
			StreamWriter writer = File.AppendText("log/log.txt");
			writer.Write(message + "\r\n");
			writer.Close();
		}

		/// <summary>
		/// Загружает данные в правую часть окна
		/// </summary>
		/// <param name="way"></param>
		/// <param name="viewWay"></param>
		private void LoadToViewer(string way, string viewWay = "view/temp")
		{
			logger(new DateTime() + " Обработка файла: " + way);

			if (!Directory.Exists(Path.GetDirectoryName(viewWay)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(viewWay));
			}
			File.Copy(way, viewWay, true);
			FileInfo fi = new FileInfo(viewWay);
			if (fi.Length > 500 * 1024 * 1024)
			{
				System.Windows.MessageBox.Show("Размер файла слишком большой для показа в текущем приложении",
					"Слишком большой размер файла");
			}
			else
			{
				string extension = Path.GetExtension(way);

				if (extension.Equals(".txt") || extension.Equals(".csv"))
				{
					logger(new DateTime() + "Отображение файла будет в TextBox");
					this.tabControl.SelectedItem = this.textboxTab;
					displayTextbox(way);
				}
				else if (extension.Equals(".jpeg") || extension.Equals(".tiff") || extension.Equals(".jpg"))
				{
					logger(new DateTime() + "Отображение файла будет в Image");
					string fullWay = Path.Combine(Path.GetDirectoryName(
									Assembly.GetExecutingAssembly().Locati‌​on), way);
					byte[] buffer = System.IO.File.ReadAllBytes(fullWay);//сюда подставляются image
					MemoryStream ms = new MemoryStream(buffer);
					BitmapImage bitmap33 = new BitmapImage();
					bitmap33.BeginInit();
					bitmap33.StreamSource = ms;
					bitmap33.EndInit();
					bitmap33.Freeze();
					image.Source = bitmap33;
					tabControl.SelectedItem = imageTab;
					lastLoadPicture = true;
				}
				else if (extension.Equals(".doc") || extension.Equals(".docx"))
				{
					if ((bool) this.swtichModeView.IsChecked)
					{
						logger(new DateTime() + "Отображение файла будет в DocumentViewer");
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
						doc.Close();
					}
					else
					{
						logger(new DateTime() + "Отображение файла будет в docViewer");
						this.docViewer.CloseDocument();
						this.docViewer.LoadFromFile(way);
						tabControl.SelectedItem = this.docViewerTab;
					}
				}
				else if (extension.Equals(".xlsx") || extension.Equals(".xls"))
				{
					logger(new DateTime() + "Отображение файла будет в viewer");
					this.tabControl.SelectedItem = this.viewerTab;
					this.viewer.Document = null;
					this.viewer.DataContext = null;
					if (doc != null)
					{
						this.doc.Close();
					}
					this.doc = null;

					try
					{
						convertXlsToXps(way, viewWay);
						this.doc = new XpsDocument(viewWay, FileAccess.Read);
						viewer.Document = doc.GetFixedDocumentSequence();
						this.doc.Close();
					}
					catch (Exception exp)
					{
						this.doc = null;
						this.viewer.Document = null;
						this.viewer.DataContext = null;
					}
				}
				else if (extension.Equals(".pdf"))
				{
					logger(new DateTime() + "Отображение файла будет в pdfviewer");
					this.tabControl.SelectedItem = this.pdfViewerTab;
					byte[] bytes = File.ReadAllBytes(way);
					var source = new MemorySource(bytes);

					moonPdfPanel.Open(source);
					moonPdfPanel.PageRowDisplay = MoonPdfLib.PageRowDisplayType.ContinuousPageRows;
				}

			}
		}

		/// <summary>
		/// Отображает содержимое файла (текстового) в RichTextBox textbox
		/// </summary>
		/// <param name="viewWay"></param>
		private void displayTextbox(string viewWay)
		{
			textbox.Height = viewer.Height;
			textbox.Width = viewer.Width;
			textbox.Document.Blocks.Clear();
			var sr = new StreamReader(viewWay, Encoding.UTF8);
			string text = sr.ReadToEnd();
			sr.Close();
			sr.Dispose();
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
					throw new Exception();
			}
			catch (System.ArgumentOutOfRangeException e)
			{
				System.Windows.MessageBox.Show("Не удалось выполнить преобразование документа," +
					" скорее всего файл поврежден и нуждается в восстановлении");
					throw new Exception();
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
			try
			{
				DateTime oldTime = File.GetLastWriteTime(this.openFile);
				Process currProc = Process.Start(this.openFile);
				currProc.WaitForExit();
				currProc.Close();
				if (!oldTime.Equals(File.GetLastWriteTime(this.openFile)))
				{
					if (MessageBox.Show("Отправить измененный файл на сервер?", "Файл был изменен", 
						MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
					{
						int index = Int32.Parse(this.alianceAndId[
							ComboboxFileSystem.SelectedItem.ToString()]);
						Tuple<bool, TreeViewItem> selectedItem = searchSelectedItem();

						string relativeWay = this.listTreeView[selectedItem.Item2].Item2;
						UploadWindow download = new UploadWindow(index.ToString(), 
							Path.GetDirectoryName(relativeWay),
							Path.Combine(Path.GetDirectoryName(
									Assembly.GetExecutingAssembly().Locati‌​on), 
									this.openFile) , 
							"1");
						download.sendFileToServer();
					}
				}
			}
			catch (System.NullReferenceException exceptionWithOpenFile)
			{

			}
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
			textbox.Height = viewer.Height;
			textbox.Width = viewer.Width;
			image.Height = viewer.Height;
			image.Width = viewer.Width;
			docViewer.Height = viewer.Height;
			docViewer.Width = viewer.Width;
			moonPdfPanel.Height = viewer.Height;
			moonPdfPanel.Width = viewer.Width;
		}

		/// <summary>
		/// CheckBox_Click
		/// Обновление графического интерфейса осуществляющего показ содержимого документа
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if ((bool)swtichModeView.IsChecked)
			{
				swtichModeView.Content = "Преобразование в XPS формат";
			}
			else
			{
				swtichModeView.Content = "";
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			setVisibleOnText((sender as TextBox).Text);
		}

		void setVisibleOnText(string text)
		{
			List<TreeViewItem> newListItems = new List<TreeViewItem>();
			foreach (TreeViewItem elem in basisListItems)
			{
				bool newElem = setVisibleOnTextForTreeView(elem, text);
			}
		}

		bool setVisibleOnTextForTreeView(TreeViewItem item, string text, bool mandatory = false)
		{
			string temp = this.listTreeView[item].Item2;
			string nameElement = Path.GetFileName(temp);
			if (mandatory)
			{
				if (item.Items.Count > 0)
				{
					foreach (TreeViewItem elem in item.Items)
					{
						elem.Visibility = Visibility.Visible;
						string temp1 = this.listTreeView[elem].Item2;
						string nameElement1 = Path.GetFileName(temp1);
						elem.Header = highlightText(nameElement1, text, this.listTreeView[elem].Item1);
					}
				}
				item.Header = highlightText(nameElement, text, this.listTreeView[item].Item1);
				item.Visibility = Visibility.Visible;
				return true;
			}
			else
			{
				if (item.Items.Count > 0)
				{
					if (temp.IndexOf(text) > -1)
					{
						item.Visibility = Visibility.Visible;
						bool typeTreeView = listTreeView.ContainsKey(item);
						item.Header = highlightText(nameElement, text, this.listTreeView[item].Item1);
						foreach (TreeViewItem elem in item.Items)
						{
							elem.Visibility = Visibility.Visible;
							string temp1 = this.listTreeView[elem].Item2;
							string nameElement1 = Path.GetFileName(temp1);
							elem.Header = highlightText(nameElement1, text, this.listTreeView[elem].Item1);
							setVisibleOnTextForTreeView(elem, text, true);
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

								string temp1 = this.listTreeView[elem].Item2;
								string nameElement1 = Path.GetFileName(temp1);
								elem.Header = highlightText(nameElement1, text, this.listTreeView[elem].Item1);

								changeVisible++;
							}
							else
							{
								elem.Visibility = Visibility.Collapsed;
							}
						}

						if (changeVisible > 0)
						{
							item.Header = highlightText(nameElement, text, this.listTreeView[item].Item1);
							item.Visibility = Visibility.Visible;
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
					if (temp.IndexOf(text) > -1)
					{
						item.Header = highlightText(nameElement, text, this.listTreeView[item].Item1);
						item.Visibility = Visibility.Visible;
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

		void openWeb(string url)
		{
			string messageBoxText = "Вы уверены, что хотите перейти по этой ссылке: " + url + "?";
			string caption = "Переход по ссылке";
			MessageBoxButton button = MessageBoxButton.YesNoCancel;
			MessageBoxImage icon = MessageBoxImage.Question;
			MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
			if (result == MessageBoxResult.Yes)
			{
				try
				{
					System.Diagnostics.Process.Start(url);
				}
				catch (System.ComponentModel.Win32Exception e)
				{
					System.Windows.MessageBox.Show("Ошибка",
					"Не удалось открыть ссылку");
				}
			}
		}

		TextBlock highlightText(string source, string substring, bool isFile)
		{
			TextBlock result = new TextBlock();
			result.Inlines.Add(UsersTreeViewItem.getImageOnNameFile(Path.GetFileName(source), isFile));
			result.Inlines.Add("  ");
			//result.Height = 10;
			if (substring.Length != 0)
			{
				var indices = new List<int>();
				int index = source.IndexOf(substring, 0);
				while (index > -1)
				{
					indices.Add(index);
					index = source.IndexOf(substring, index + substring.Length);
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
						correctiveName.Foreground = Brushes.Blue;
						correctiveName.TextDecorations = TextDecorations.Underline;

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
	}
}
