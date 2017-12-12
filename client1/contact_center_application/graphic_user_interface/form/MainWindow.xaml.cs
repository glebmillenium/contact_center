﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using contact_center_application.core;
using Newtonsoft.Json;
using contact_center_application.core.serialization;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using contact_center_application.core.storage_dynamic_data;
using contact_center_application.graphic_user_interface.manage_graphical_component.tree_view;
using contact_center_application.graphic_user_interface.manage_graphical_component.viewer;

namespace contact_center_application.graphic_user_interface.form
{
	/// <summary>
	/// Класс, формирующий графический интерфейс отображения содержимое файловой системы.
	/// </summary>
	public partial class MainWindow : Window
	{

		/// <summary>
		/// Конструктор, осуществляет чистку папки временных файлов
		/// </summary>
		public MainWindow()
		{
			Logger.initialize();
			InitializeComponent();
			MainWindowElement.initialize(this);
			ManagerViewer.textbox = this.textboxDisplay;
			ManagerViewer.otherViewer = this.otherOnViewer;
			CurrentDataFileSystem.ComboboxFileSystem = this.ComboboxChooseFileSystem;
			CurrentDataFileSystem.treeViewCatalog = treeViewCatalogFileSystem;
			MainWindowElement.versionTextBlock.Text += SettingsData.getVersion();
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
		/// Метод запускается при включении приложения
		/// </summary>
		/// <param name="state"></param>
		private void firstExchangeWithServer(object state)
		{
			try
			{
				string address = SettingsData.getAddress();
				string[] aliance = RequestDataFromServer.primaryExchangeWithSocket();
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
		/// Обработчик события, вызывается по изменению ComboBox.
		/// По указанному combobox получает содержимое файловой системы
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ComboboxFileSystem_SelectionChanged(object sender, 
			SelectionChangedEventArgs e)
		{
			ManagerViewer.callGarbage();
			CurrentDataFileSystem.getContentFileSystem();
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
				if (SettingsData.getRightWrite() == 1)
				{
					if (!oldTime.Equals(File.GetLastWriteTime(CurrentDataOpenFile.openFile)))
					{
						if (MessageBox.Show("Отправить измененный файл на сервер?", "Файл был изменен",
							MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
						{
							TreatmenterExchangeFileWithServer.sendFileToServer();
						}
					}
				}
				else
				{
					MessageBox.Show("Вы изменили содержимое файла." +
						" Но у вас недостаточно прав для отправки измененного файла на сервер.", "Файл был изменен");
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
		public static void ButtonUpdateCatalogs_Click(object sender, RoutedEventArgs e)
		{
			EventsForButtons.ButtonUpdateCatalogs_Click(sender, e);
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
			TreatmenterExchangeFileWithServer.loadFileToServer();
		}

		private void switchModelViewButtonXPS_Click(object sender, RoutedEventArgs e)
		{
			ManagerViewer.switchViewXPSMode();
		}

		private void registrOnButton_Click(object sender, RoutedEventArgs e)
		{
			if ((bool)MainWindowElement.registrButton.IsChecked)
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

		private void openFolders_Checked(object sender, RoutedEventArgs e)
		{
			if ((bool)MainWindowElement.openFolders.IsChecked)
			{
				MainWindowElement.openFolders.Background =
					new SolidColorBrush(Colors.White);
			}
			else
			{
				MainWindowElement.openFolders.Background =
					new SolidColorBrush(Colors.White);
			}
			FilterTreeViewItem.setVisibleOnText(this.searchTextBox.Text);
		}

		private void exitFromSystem_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void goToAuth_Click(object sender, RoutedEventArgs e)
		{
			Authorization auth = new Authorization();
			auth.Show();
			this.Close();
		}

		private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			RequestDataFromServer.closeConnection();
		}
	}
}