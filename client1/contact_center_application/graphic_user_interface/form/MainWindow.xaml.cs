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
			MainWindowElement.registrButton = registrOnButton;
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
			FilterTreeViewItem.resetFlagsInTreeViewItem();
			getContentFileSystem();
			CurrentDataFileSystem.deleteNotNeedItemsInTreeViewItem(selectedItem);
			selectedItem.IsSelected = true;
			this.updateCatalog = false;
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
			FilterTreeViewItem.setVisibleOnText((sender as TextBox).Text);
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

		private void registrOnButton_Click(object sender, RoutedEventArgs e)
		{
			if ((bool) this.switchModelViewButtonXPS.IsChecked)
			{
				MainWindowElement.registrButton.Background =
					new SolidColorBrush(Colors.Black);
				MainWindowElement.registrButton.ToolTip = "Выключить учет регистра букв";
			}
			else
			{
				MainWindowElement.registrButton.Background =
					new SolidColorBrush(Colors.White);
				MainWindowElement.registrButton.ToolTip = "Включить учет регистра букв";
			}
			FilterTreeViewItem.setVisibleOnText(this.searchTextBox.Text);
		}

		private void buttonSettings_Click(object sender, RoutedEventArgs e)
		{
			new Settings().ShowDialog();
		}
	}
}