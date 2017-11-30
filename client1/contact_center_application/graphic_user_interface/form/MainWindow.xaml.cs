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
using contact_center_application.graphic_user_interface.manage_graphical_component;
using System.Windows.Documents;
using System.Text;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Input;
using MoonPdfLib.MuPdf;
using Spire.DocViewer.Wpf;
using MoonPdfLib;
using System.Windows.Media;

namespace contact_center_application.graphic_user_interface.form
{
	/// <summary>
	/// Класс, формирующий графический интерфейс отображения содержимое файловой системы.
	/// </summary>
	public partial class MainWindow : Window
	{
		private DocumentViewer viewer = new DocumentViewer(); //viewerTab
		private MoonPdfPanel moonPdfPanel = new MoonPdfPanel(); //background light-gray
		private DocViewer docViewer = new DocViewer();
		private Image image = new Image();
		XpsDocument doc;
		private bool updateCatalog = false;
		string currentView = "view/temp1";

		/// <summary>
		/// Конструктор, осуществляет чистку папки временных файлов
		/// </summary>
		public MainWindow()
		{
			Logger.initialize();

			InitializeComponent();
			CurrentDataFileSystem.ComboboxFileSystem = this.ComboboxChooseFileSystem;
			CurrentDataFileSystem.treeViewCatalog = treeViewCatalogFileSystem;
			progressConvertation.Visibility = Visibility.Hidden;
			managerPanel.Visibility = Visibility.Collapsed;
			//  DispatcherTimer setup
			System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
			dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
			dispatcherTimer.Start();

			SynchronizationContext uiContext = SynchronizationContext.Current;
			Thread thread = new Thread(Run);
			thread.Start(uiContext);
		}

		/// <summary>
		/// Задается обновление текущей каталоговой системы
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			ButtonUpdateCatalogs_Click(null, null);
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
			try
			{
				int index = Int32.Parse(
					CurrentDataFileSystem.alianceIdPolicy[CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1);
				string answer =
					RequestDataFromServer.getCatalog(index.ToString());
				fullingTreeView(answer);
				buttonRefresh.Visibility = Visibility.Hidden;
			}
			catch (System.NullReferenceException e)
			{
				Logger.log(e.Message);
			}
		}

		/// <summary>
		/// Метод запускается при включении приложения
		/// </summary>
		/// <param name="state"></param>
		private void firstExchangeWithServer(object state)
		{
			try
			{
				string address = SettingsData.getAddress();
				string[] aliance = RequestDataFromServer.primaryExchangeWithSocket(address);
				CurrentDataFileSystem.alianceIdPolicy.Clear();
				CurrentDataFileSystem.ComboboxFileSystem.Items.Clear();
				for (int i = 0; i < aliance.Length; i++)
				{
					string[] item = JsonConvert.DeserializeObject<string[]>(aliance[i]);
					if (item.Length == 3)
					{
						CurrentDataFileSystem.alianceIdPolicy.Add(
							item[1], new Tuple<string, string>(item[0], item[2]));
						CurrentDataFileSystem.ComboboxFileSystem.Items.Add(item[1]);
					}
				}
				CurrentDataFileSystem.ComboboxFileSystem.SelectedItem = JsonConvert.DeserializeObject<string[]>(aliance[0])[1];
			}
			catch (System.Net.Sockets.SocketException socketException)
			{
				Logger.log(socketException.Message);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="json"></param>
		private void fullingTreeView(string json)
		{
			if (!updateCatalog)
			{
				CurrentDataFileSystem.listTreeView.Clear();
			}
			ContextMenuForTreeView.setContextMenuForTreeView();

			CurrentDataFileSystem.basisListItems = getItemsCatalogsFromJson(json, "");

			if (CurrentDataFileSystem.basisListItems.Count != 0)
			{
				CurrentDataFileSystem.treeViewCatalog.Items.Clear();
				foreach (var category in CurrentDataFileSystem.basisListItems)
				{
					CurrentDataFileSystem.treeViewCatalog.Items.Add(category);
				}
			}
		}

		private TreeViewItem getTreeViewItemForFile(
			CatalogForSerialization element, string currentWay, int deep = 1)
		{
			TreeViewItem item = getSearchItemOnCurrentWay(currentWay + "\\" + element.name);
			if (item == null)
			{
				item = ProcessTreeViewItem.getTreeViewItem(element.name, true);
				System.Windows.Controls.ContextMenu docMenu = new System.Windows.Controls.ContextMenu();
				item.MouseDoubleClick += this.selectFile;
				item.KeyDown += Item_KeyDown;
				CurrentDataFileSystem.listTreeView.Add(item, new Tuple<bool, string, bool>(element.file,
					currentWay + "\\" + element.name, true));

				System.Windows.Controls.MenuItem open = new System.Windows.Controls.MenuItem();
				open.Click += Open_Click; ;
				open.Header = "Открыть файл";
				docMenu.Items.Add(open);

				System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
				delete.Header = "Удалить файл";
				delete.Click += EventsForContextMenuTreeView.Delete_Click;
				docMenu.Items.Add(delete);

				System.Windows.Controls.MenuItem rename = new System.Windows.Controls.MenuItem();
				rename.Header = "Переименовать";
				rename.Click += EventsForContextMenuTreeView.Rename_Click;
				docMenu.Items.Add(rename);
				item.ContextMenuOpening += Item_ContextMenuOpening;
				item.ContextMenu = docMenu;
			}
			else
			{
				CurrentDataFileSystem.listTreeView[item] = new Tuple<bool, string, bool>
				(element.file, currentWay + "\\" + element.name, true);
			}

			return item;
		}

		private void Item_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				selectFile(null, null);
			}
		}

		/// <summary>
		/// Получение 
		/// </summary>
		/// <param name="element"></param>
		/// <param name="currentWay"></param>
		/// <returns></returns>
		private TreeViewItem getTreeViewItemForCatalog(
			CatalogForSerialization element, string currentWay, 
			int deep = 1)
		{
			TreeViewItem item = getSearchItemOnCurrentWay(currentWay + "\\" + element.name);
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

			setContextMenuToTreeViewItem(item, element, deep);

			return item;
		}

		private void policyContactCenter(TreeViewItem item, 
			ContextMenu docMenu, CatalogForSerialization element, int deep)
		{
			System.Windows.Controls.MenuItem upload = new System.Windows.Controls.MenuItem();
			switch (deep)
			{
				case 1:
					upload.Header = "Загрузить файл в категорию";
					upload.Click += EventsForContextMenuTreeView.Upload_Click;
					docMenu.Items.Add(upload);
					break;
				case 2:
					upload.Header = "Загрузить файл в тему";
					upload.Click += EventsForContextMenuTreeView.Upload_Click;
					docMenu.Items.Add(upload);
					break;
				case 3:
					upload.Header = "Загрузить файл в подтему";
					upload.Click += EventsForContextMenuTreeView.Upload_Click;
					docMenu.Items.Add(upload);
					break;
			}

			System.Windows.Controls.MenuItem createDir = new System.Windows.Controls.MenuItem();
			switch (deep)
			{
				case 1:
					createDir.Header = "Создать новую тему в " + element.name;
					createDir.Click += EventsForContextMenuTreeView.CreateDir_Click;
					docMenu.Items.Add(createDir);
					break;
				case 2:
					createDir.Header = "Создать новую подтему в " + element.name;
					createDir.Click += EventsForContextMenuTreeView.CreateDir_Click;
					docMenu.Items.Add(createDir);
					break;
			}

			System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
			switch (deep)
			{
				case 1:
					delete.Header = "Удалить категорию";
					delete.Click += EventsForContextMenuTreeView.Delete_Click;
					docMenu.Items.Add(delete);
					break;
				case 2:
					delete.Header = "Удалить тему";
					delete.Click += EventsForContextMenuTreeView.Delete_Click;
					docMenu.Items.Add(delete);
					break;
				case 3:
					delete.Header = "Удалить подтему";
					delete.Click += EventsForContextMenuTreeView.Delete_Click;
					docMenu.Items.Add(delete);
					break;
				default:
					delete.Header = "Удалить папку";
					delete.Click += EventsForContextMenuTreeView.Delete_Click;
					docMenu.Items.Add(delete);
					break;
			}

			System.Windows.Controls.MenuItem rename = new System.Windows.Controls.MenuItem();
			switch (deep)
			{
				case 1:
					rename.Header = "Переименовать категорию";
					rename.Click += EventsForContextMenuTreeView.Rename_Click;
					docMenu.Items.Add(rename);
					break;
				case 2:
					rename.Header = "Переименовать тему";
					rename.Click += EventsForContextMenuTreeView.Rename_Click;
					docMenu.Items.Add(rename);
					break;
				case 3:
					rename.Header = "Переименовать подтему";
					rename.Click += EventsForContextMenuTreeView.Rename_Click;
					docMenu.Items.Add(rename);
					break;
			}

			item.ContextMenuOpening += Item_ContextMenuOpening;
			item.ContextMenu = docMenu;
		}

		private void setContextMenuToTreeViewItem(TreeViewItem item, CatalogForSerialization element, int deep)
		{
			System.Windows.Controls.ContextMenu docMenu = new System.Windows.Controls.ContextMenu();
			int policy = Int32.Parse(CurrentDataFileSystem.alianceIdPolicy[CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item2);
			switch (policy)
			{
				case 1:
					policyContactCenter(item, docMenu, element, deep);
					break;
				default:
					System.Windows.Controls.MenuItem upload = new System.Windows.Controls.MenuItem();
					upload.Header = "Загрузить файл в папку";
					upload.Click += EventsForContextMenuTreeView.Upload_Click;
					docMenu.Items.Add(upload);

					System.Windows.Controls.MenuItem createDir = new System.Windows.Controls.MenuItem();
					createDir.Header = "Создать новую папку в " + element.name;
					createDir.Click += EventsForContextMenuTreeView.CreateDir_Click;
					docMenu.Items.Add(createDir);

					System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
					delete.Header = "Удалить папку";
					delete.Click += EventsForContextMenuTreeView.Delete_Click; ;
					docMenu.Items.Add(delete);

					System.Windows.Controls.MenuItem rename = new System.Windows.Controls.MenuItem();
					rename.Header = "Переименовать";
					rename.Click += EventsForContextMenuTreeView.Rename_Click;
					docMenu.Items.Add(rename);
					item.ContextMenuOpening += Item_ContextMenuOpening;
					item.ContextMenu = docMenu;
					break;
			}
			
		}

		/// <summary>
		/// Десериализует json - строку в дерево treeView, 
		/// с указанием их относительного пути
		/// </summary>
		/// <param name="json"></param>
		/// <param name="currentWay"></param>
		/// <returns></returns>
		private List<TreeViewItem> getItemsCatalogsFromJson(string json, string currentWay, int deep = 0)
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
					item = getTreeViewItemForCatalog(element, currentWay, deep);
				}
				else
				{
					item = getTreeViewItemForFile(element, currentWay);
				}

				result.Add(item);
			}
			return result;
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
		/// Событие вызываемое при открытии файла
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Open_Click(object sender, RoutedEventArgs e)
		{
			selectFile(null, null);
		}

		/// <summary>
		/// Обработчик события, вызывается по изменению ComboBox.
		/// По указанному combobox получает содержимое файловой системы
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ComboboxFileSystem_SelectionChanged(object sender, 
			SelectionChangedEventArgs e)
		{
			callGarbage();
			getContentFileSystem();
		}

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

			this.imageStackPanel.Children.Clear();
			this.imageStackPanel.Children.Add(this.image);

			if (File.Exists(currentView))
			{
				File.Delete(currentView);
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
			try
			{
				Tuple<bool, TreeViewItem> selectedItem = CurrentDataFileSystem.searchSelectedItem();
				if (selectedItem.Item1)
				{
					this.Cursor = Cursors.Wait;

					callGarbage();

					string aliance = CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString();
					string relativeWay = CurrentDataFileSystem.listTreeView[selectedItem.Item2].Item2;
					string selected = CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString();
					CurrentDataOpenFile.openFile = "tmp" + relativeWay;
					if (Path.GetExtension(relativeWay).Equals(".link"))
					{
						openWeb(Path.GetFileNameWithoutExtension(relativeWay));
					}
					else
					{
						int index = Int32.Parse(CurrentDataFileSystem.alianceIdPolicy[CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1);
						DownloadWindow download = new DownloadWindow(index.ToString(),
							relativeWay);
						this.IsEnabled = false;
						download.getContentFileAndWriteToFile(CurrentDataOpenFile.openFile);
						this.IsEnabled = true;
						progressConvertation.Visibility = Visibility.Visible;
						try
						{
							LoadToViewer(CurrentDataOpenFile.openFile, this.currentView);
						}
						catch (OutOfMemoryException exceptionMemory)
						{
							MessageBox.Show("Системных ресурсов вашей операционной системы оказалось " +
								"недостаточно для отображения содержимого файла(" + Path.GetFileName(CurrentDataOpenFile.openFile) +
								") в данном приложении. " +
								"Попытайтесь открыть файл во внешнем приложении", "Нехватка системных ресурсов");
							Logger.log(exceptionMemory.Message);
						}
						catch (Exception exp)
						{
							MessageBox.Show("Неизвестная ошибка", "UNKNOWN");
							Logger.log(exp.Message);
						}
						finally
						{
							if (this.currentView.Equals("view/temp1"))
							{
								this.currentView = "view/temp2";
							}
							else
							{
								this.currentView = "view/temp1";
							}
							progressConvertation.Visibility = Visibility.Hidden;
						}
					}
					this.managerPanel.Visibility = Visibility.Visible;
					this.Cursor = Cursors.Arrow;
				}
			}
			catch (Exception exp)
			{
				Logger.log(exp.ToString());
				System.Windows.MessageBox.Show("Отказали системные компоненты приложения. " +
					"Попробуйте повторить действие. В случае повторного возникновения ошибки перезапустите приложение.", "Критическая ошибка");
			}
		}

		/// <summary>
		/// Загружает данные в правую часть окна
		/// </summary>
		/// <param name="way"></param>
		/// <param name="viewWay"></param>
		private void LoadToViewer(string way, string viewWay)
		{
			CurrentDataOpenFile.dateOpenFile = File.GetLastWriteTime(way);
			CurrentDataOpenFile.relationWayOpenFile = CurrentDataFileSystem.listTreeView[CurrentDataFileSystem.searchSelectedItem().Item2].Item2;
			Logger.log(new DateTime() + " Обработка файла: " + way);

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

				if (extension.Equals(".txt") || extension.Equals(".csv") ||
					extension.Equals(".xml") || extension.Equals(".html"))
				{
					Logger.log("Отображение файла будет в TextBox");
					this.tabControl.SelectedItem = this.textboxTab;
					displayTextbox(way);
				}
				else if (extension.Equals(".jpeg") || extension.Equals(".tiff") ||
					extension.Equals(".jpg") || extension.Equals(".png"))
				{
					Logger.log("Отображение файла будет в Image");
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
				}
				else if (extension.Equals(".doc") || extension.Equals(".docx"))
				{
					if ((bool)this.switchModeViewButton.IsChecked)
					{
						Logger.log("Отображение файла будет в DocumentViewer");
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
						Logger.log("Отображение файла будет в docViewer");
						this.docViewer.CloseDocument();
						this.docViewer.LoadFromFile(way);
						tabControl.SelectedItem = this.docViewerTab;
					}
				}
				else if (extension.Equals(".xlsx") || extension.Equals(".xls"))
				{
					Logger.log("Отображение файла будет в viewer");
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
						Logger.log(exp.Message);
					}
				}
				else if (extension.Equals(".pdf"))
				{
					Logger.log("Отображение файла будет в pdfviewer");
					this.tabControl.SelectedItem = this.pdfViewerTab;
					byte[] bytes = File.ReadAllBytes(way);
					var source = new MemorySource(bytes);

					moonPdfPanel.Open(source);
					moonPdfPanel.PageRowDisplay = MoonPdfLib.PageRowDisplayType.ContinuousPageRows;
				}
				else
				{

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
				Logger.log(e.Message);
			}
			catch (System.IO.IOException e)
			{
				System.Windows.MessageBox.Show("По неясным причинам приложению не удалось отобразить требуемый документ." +
					" Тем не менее требуемый документ можно открыть в любом внешнем приложении.\n" +
					"Например: Word Office, OpenOffice, LibreOffice", "Ошибка вывода на экран");
				Logger.log(e.Message);
			}
			catch (System.ApplicationException e)
			{
				System.Windows.MessageBox.Show("По неясным причинам приложению не удалось отобразить требуемый документ." +
					" Тем не менее требуемый документ можно открыть в любом внешнем приложении.\n" +
					"Например: Word Office, OpenOffice, LibreOffice", "Ошибка вывода на экран");
				Logger.log(e.Message);
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
				Logger.log(e.Message);
				throw new Exception();
			}
			catch (System.ArgumentOutOfRangeException e)
			{
				System.Windows.MessageBox.Show("Не удалось выполнить преобразование документа," +
					" скорее всего файл поврежден и нуждается в восстановлении");
				Logger.log(e.Message);
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
				DateTime oldTime = File.GetLastWriteTime(CurrentDataOpenFile.openFile);
				Process currProc = Process.Start(CurrentDataOpenFile.openFile);
				currProc.WaitForExit();
				currProc.Close();
				if (!oldTime.Equals(File.GetLastWriteTime(CurrentDataOpenFile.openFile)))
				{
					if (MessageBox.Show("Отправить измененный файл на сервер?", "Файл был изменен",
						MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
					{
						sendFileToServer();
					}
				}
			}
			catch (System.NullReferenceException exceptionWithOpenFile)
			{
				Logger.log(exceptionWithOpenFile.Message);
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
			this.updateCatalog = true;
			TreeViewItem selectedItem = CurrentDataFileSystem.searchSelectedItem().Item2;
			resetFlagsInTreeViewItem();
			getContentFileSystem();
			CurrentDataFileSystem.deleteNotNeedItemsInTreeViewItem(selectedItem);
			selectedItem.IsSelected = true;
			this.updateCatalog = false;
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
			if (window.ActualHeight - 165 <= 0) return;
			viewer.Height = window.ActualHeight - 165;
			textbox.Height = viewer.Height;
			textbox.Width = viewer.Width;
			image.Height = viewer.Height;
			image.Width = viewer.Width;
			docViewer.Height = viewer.Height;
			docViewer.Width = viewer.Width;
			moonPdfPanel.Height = viewer.Height;
			moonPdfPanel.Width = viewer.Width;
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			setVisibleOnText((sender as TextBox).Text);
		}

		void setVisibleOnText(string text)
		{
			List<TreeViewItem> newListItems = new List<TreeViewItem>();
			foreach (TreeViewItem elem in CurrentDataFileSystem.basisListItems)
			{
				bool newElem = setVisibleOnTextForTreeView(elem, text);
			}
		}

		bool setVisibleOnTextForTreeView(TreeViewItem item, string text, bool mandatory = false)
		{
			string temp = CurrentDataFileSystem.listTreeView[item].Item2;
			string nameElement = Path.GetFileName(temp);
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
				return true;
			}
			else
			{
				if (item.Items.Count > 0)
				{
					int resultSearch;
					resultSearch = temp.IndexOf(text);
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
					if ((bool) this.registrButton.IsChecked)
					{
						resultSearch = temp.IndexOf(text);
					}
					else
					{
						resultSearch = temp.ToLower().IndexOf(text.ToLower());
					}
					if (resultSearch > -1)
					{
						item.Header = highlightText(nameElement, text, CurrentDataFileSystem.listTreeView[item].Item1);
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
					Logger.log(e.Message);
					System.Windows.MessageBox.Show("Ошибка",
					"Не удалось открыть ссылку");
				}
			}
		}

		TextBlock highlightText(string source, string substring, bool isFile)
		{
			TextBlock result = new TextBlock();
			result.Inlines.Add(ProcessTreeViewItem.getImageOnNameFile(Path.GetFileName(source), isFile));
			result.Inlines.Add("  ");
			//result.Height = 10;
			if (substring.Length != 0)
			{
				var indices = new List<int>();
				int index;
				if((bool)this.registrButton.IsChecked)
					index = source.IndexOf(substring, 0);
				else
					index = source.ToLower().IndexOf(substring.ToLower(), 0);
				while (index > -1)
				{
					indices.Add(index);
					if ((bool) this.registrButton.IsChecked)
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

		private TreeViewItem getSearchItemOnCurrentWay(string currentWay)
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

		private void loadToFileToServer_Click(object sender, RoutedEventArgs e)
		{
			if (!CurrentDataOpenFile.dateOpenFile.Equals(File.GetLastWriteTime(CurrentDataOpenFile.openFile)))
			{
				if (MessageBox.Show("Отправить измененный файл на сервер?", "Файл был изменен",
					MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					sendFileToServer();
				}
			}
			else
			{
				if (MessageBox.Show("Файл не был изменен. Все равно отправить на сервер?", "Файл не был изменен",
					MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					sendFileToServer();
				}
			}
		}

		private void sendFileToServer()
		{
			int index = Int32.Parse(CurrentDataFileSystem.alianceIdPolicy[
							CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1);
			Tuple<bool, TreeViewItem> selectedItem = CurrentDataFileSystem.searchSelectedItem();

			string relativeWay = CurrentDataOpenFile.relationWayOpenFile;
			UploadWindow download = new UploadWindow(index.ToString(),
				Path.GetDirectoryName(relativeWay),
				Path.Combine(Path.GetDirectoryName(
						Assembly.GetExecutingAssembly().Locati‌​on),
						CurrentDataOpenFile.openFile),
				"1");
			download.sendFileToServer();
			try
			{
				LoadToViewer(CurrentDataOpenFile.openFile, this.currentView);
			}
			catch (OutOfMemoryException exceptionMemory)
			{
				MessageBox.Show("Системных ресурсов вашей операционной системы оказалось "
					+ "недостаточно для отображения содержимого файла("
					+ Path.GetFileName(CurrentDataOpenFile.openFile)
					+ ") в данном приложении. "
					+ "Попытайтесь открыть файл во внешнем приложении", "Нехватка системных ресурсов");
				Logger.log(exceptionMemory.Message);
			}
			catch (Exception exp)
			{
				MessageBox.Show("Неизвестная ошибка", "UNKNOWN");
				Logger.log(exp.Message);
			}
			finally
			{
				if (this.currentView.Equals("view/temp1"))
				{
					this.currentView = "view/temp2";
				}
				else
				{
					this.currentView = "view/temp1";
				}
				progressConvertation.Visibility = Visibility.Hidden;
			}
		}

		private void switchModeViewButton_Click(object sender, RoutedEventArgs e)
		{
			string extension = Path.GetExtension(CurrentDataOpenFile.openFile);
			if ((bool) this.switchModeViewButton.IsChecked)
			{
				this.switchModeViewButton.Background = 
					new SolidColorBrush(Colors.DarkCyan);
			}
			else
			{
				this.switchModeViewButton.Background = 
					new SolidColorBrush(Colors.White);
			}

			if (extension.Equals(".docx") || extension.Equals(".doc"))
			{
				try
				{
					LoadToViewer(CurrentDataOpenFile.openFile, this.currentView);
				}
				catch (OutOfMemoryException exceptionMemory)
				{
					MessageBox.Show("Системных ресурсов вашей операционной системы оказалось " +
						"недостаточно для отображения содержимого файла(" + Path.GetFileName(CurrentDataOpenFile.openFile) +
						") в данном приложении. " +
						"Попытайтесь открыть файл во внешнем приложении", "Нехватка системных ресурсов");
					Logger.log(exceptionMemory.Message);
				}
				catch (Exception exp)
				{
					MessageBox.Show("Неизвестная ошибка", "UNKNOWN");
					Logger.log(exp.Message);
				}
				finally
				{
					if (this.currentView.Equals("view/temp1"))
					{
						this.currentView = "view/temp2";
					}
					else
					{
						this.currentView = "view/temp1";
					}
					progressConvertation.Visibility = Visibility.Hidden;
				}
			}
		}

		private void registrButton_Click(object sender, RoutedEventArgs e)
		{
			if ((bool) this.switchModeViewButton.IsChecked)
			{
				this.registrButton.Background =
					new SolidColorBrush(Colors.Black);
				this.registrButton.ToolTip = "Выключить учет регистра букв";
			}
			else
			{
				this.registrButton.Background =
					new SolidColorBrush(Colors.White);
				this.registrButton.ToolTip = "Включить учет регистра букв";
			}
			setVisibleOnText(this.searchTextBox.Text);
		}

		private void buttonSettings_Click(object sender, RoutedEventArgs e)
		{
			new Settings().ShowDialog();
		}
	}
}