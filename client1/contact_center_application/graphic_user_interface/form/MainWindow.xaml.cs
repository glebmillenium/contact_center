using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using contact_center_application.core;
using Newtonsoft.Json;
using contact_center_application.core.serialization;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using contact_center_application.graphic_user_interface.manage_graphical_component;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using contact_center_application.core.storage_dynamic_data;
using contact_center_application.graphic_user_interface.manage_graphical_component.tree_view;

namespace contact_center_application.graphic_user_interface.form
{
	/// <summary>
	/// Класс, формирующий графический интерфейс отображения содержимое файловой системы.
	/// </summary>
	public partial class MainWindow : Window
	{

		private bool updateCatalog = false;

		/// <summary>
		/// Конструктор, осуществляет чистку папки временных файлов
		/// </summary>
		public MainWindow()
		{
			Logger.initialize();
			InitializeComponent();

			MainWindowElement.cursor = this.Cursor;
			MainWindowElement.tabControl = this.tabControlForViewer;
			MainWindowElement.textboxTab = this.textboxTabForViewer;
			MainWindowElement.docViewerTab = this.docViewerTabForViewer;
			MainWindowElement.pdfViewerTab = this.pdfViewerTabForViewer;
			MainWindowElement.imageTab = this.imageTabForViewer;
			MainWindowElement.viewerTab = this.viewerTabForViewer;
			MainWindowElement.switchModelViewButton = this.switchModelViewButtonXPS;
			MainWindowElement.docViewerStackPanel = this.docViewerOnStackPanel;
			MainWindowElement.viewerStackPanel = this.viewerOnStackPanel;
			MainWindowElement.pdfViewerStackPanel = this.pdfViewerOnStackPanel;
			MainWindowElement.imageStackPanel = this.imageOnStackPanel;
			MainWindowElement.progressConvertation = progressOnConvertation;
			MainWindowElement.managerPanel = managerOnPanel;
			MainWindowElement.window = this;
			ManagerViewer.textbox = this.textboxDisplay;
			CurrentDataFileSystem.ComboboxFileSystem = this.ComboboxChooseFileSystem;
			CurrentDataFileSystem.treeViewCatalog = treeViewCatalogFileSystem;

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
					CurrentDataFileSystem.alianceIdPolicy[
						CurrentDataFileSystem.ComboboxFileSystem.SelectedItem.ToString()].Item1);
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
			TreeViewItem item = ProcessTreeViewItem.getSearchItemOnCurrentWay(currentWay + "\\" + element.name);
			if (item == null)
			{
				item = ProcessTreeViewItem.getTreeViewItem(element.name, true);
				System.Windows.Controls.ContextMenu docMenu = new System.Windows.Controls.ContextMenu();
				item.MouseDoubleClick += EventsForContextMenuTreeView.selectFile;
				item.KeyDown += Item_KeyDown;
				CurrentDataFileSystem.listTreeView.Add(item, new Tuple<bool, string, bool>(element.file,
					currentWay + "\\" + element.name, true));

				System.Windows.Controls.MenuItem open = new System.Windows.Controls.MenuItem();
				open.Click += EventsForContextMenuTreeView.Open_Click; ;
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
				item.ContextMenuOpening += EventsForContextMenuTreeView.Item_ContextMenuOpening;
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
				EventsForContextMenuTreeView.selectFile(null, null);
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

			ContextMenuForTreeView.setContextMenuToTreeViewItem(item, element, deep);

			return item;
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
		/// Обработчик события, вызывается по изменению ComboBox.
		/// По указанному combobox получает содержимое файловой системы
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ComboboxFileSystem_SelectionChanged(object sender, 
			SelectionChangedEventArgs e)
		{
			ManagerViewer.callGarbage();
			getContentFileSystem();
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
			ManagerViewer.ChangeSizeViewer();
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
				ManagerViewer.LoadToViewer(CurrentDataOpenFile.openFile, ManagerViewer.currentView);
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
				if (ManagerViewer.currentView.Equals("view/temp1"))
				{
					ManagerViewer.currentView = "view/temp2";
				}
				else
				{
					ManagerViewer.currentView = "view/temp1";
				}
				MainWindowElement.progressConvertation.Visibility = Visibility.Hidden;
			}
		}

		private void switchModelViewButtonXPS_Click(object sender, RoutedEventArgs e)
		{
			string extension = Path.GetExtension(CurrentDataOpenFile.openFile);
			if ((bool) this.switchModelViewButtonXPS.IsChecked)
			{
				this.switchModelViewButtonXPS.Background = 
					new SolidColorBrush(Colors.DarkCyan);
			}
			else
			{
				this.switchModelViewButtonXPS.Background = 
					new SolidColorBrush(Colors.White);
			}

			if (extension.Equals(".docx") || extension.Equals(".doc"))
			{
				try
				{
					ManagerViewer.LoadToViewer(CurrentDataOpenFile.openFile,
						ManagerViewer.currentView);
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
					if (ManagerViewer.currentView.Equals("view/temp1"))
					{
						ManagerViewer.currentView = "view/temp2";
					}
					else
					{
						ManagerViewer.currentView = "view/temp1";
					}
					MainWindowElement.progressConvertation.Visibility = Visibility.Hidden;
				}
			}
		}

		private void registrButton_Click(object sender, RoutedEventArgs e)
		{
			if ((bool) this.switchModelViewButtonXPS.IsChecked)
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