﻿using contact_center_application.core.background_task;
using contact_center_application.graphic_user_interface.form;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace contact_center_application.core.storage_dynamic_data
{
	class MainWindowElement
	{
		public static TabControl tabControl;
		public static TabItem viewerTab;
		public static TabItem otherTabViewer;
		public static TabItem textboxTab;
		public static TabItem docViewerTab;
		public static TabItem pdfViewerTab;
		public static TabItem imageTab;
		public static ToggleButton switchModelViewButton;
		public static Cursor cursor;
		public static MainWindow window;
		public static StackPanel docViewerStackPanel;
		public static StackPanel viewerStackPanel;
		public static StackPanel pdfViewerStackPanel;
		public static StackPanel imageStackPanel;
		public static StackPanel progressConvertation;
		public static StackPanel managerPanel;
		public static ToggleButton registrButton;
		public static ToggleButton switchModeViewButtonXPS;
		public static ToggleButton openFolders;
		public static TextBlock versionTextBlock;
		public static TextBlock textBlockPopupMessenger;
		public static Button loadFileToServer;
		public static Button buttonRefresh;
		public static StackPanel stackPanelMessenger;
		public static TextBlock textBlockMessenger;
		public static ProgressBar progressBarMessenger;
		public static Popup popupMessenger;
		public static BackgroundWorker backgroundWorkerDownload;
		public static BackgroundWorker backgroundWorkerUpload;
		public static BackgroundWorker backgroundWorkerMessenger;
		public static TextBlock nameLoadFile;

		public static void initialize(MainWindow wnd)
		{
			MainWindowElement.cursor = wnd.Cursor;
			MainWindowElement.tabControl = wnd.tabControlForViewer;
			MainWindowElement.textboxTab = wnd.textboxTabForViewer;
			MainWindowElement.docViewerTab = wnd.docViewerTabForViewer;
			MainWindowElement.pdfViewerTab = wnd.pdfViewerTabForViewer;
			MainWindowElement.imageTab = wnd.imageTabForViewer;
			MainWindowElement.viewerTab = wnd.viewerTabForViewer;
			MainWindowElement.switchModelViewButton = wnd.switchModeViewButtonXPS;
			MainWindowElement.docViewerStackPanel = wnd.docViewerOnStackPanel;
			MainWindowElement.viewerStackPanel = wnd.viewerOnStackPanel;
			MainWindowElement.pdfViewerStackPanel = wnd.pdfViewerOnStackPanel;
			MainWindowElement.imageStackPanel = wnd.imageOnStackPanel;
			MainWindowElement.progressConvertation = wnd.progressOnConvertation;
			MainWindowElement.managerPanel = wnd.managerOnPanel;
			MainWindowElement.window = wnd;
			MainWindowElement.registrButton = wnd.registrOnButton;
			MainWindowElement.switchModeViewButtonXPS = wnd.switchModeViewButtonXPS;
			MainWindowElement.openFolders = wnd.openOnFolders;
			MainWindowElement.openFolders.IsChecked = true;
			MainWindowElement.versionTextBlock = wnd.versionOnTextBlock;
			MainWindowElement.loadFileToServer = wnd.loadToFileToServer;
			MainWindowElement.otherTabViewer = wnd.otherTabForViewer;

			MainWindowElement.stackPanelMessenger = wnd.stackPanelMessenger;
			MainWindowElement.textBlockMessenger = wnd.textBlockMessenger;
			MainWindowElement.progressBarMessenger = wnd.progressBarMessenger;
			MainWindowElement.buttonRefresh = wnd.buttonRefresh;
			MainWindowElement.backgroundWorkerDownload = ((BackgroundWorker)wnd.FindResource("backgroundWorkerDownload"));
			MainWindowElement.backgroundWorkerDownload.DoWork += BackgroundDownload.backgroundWorkerDownload_DoWork;
			MainWindowElement.backgroundWorkerDownload.ProgressChanged += BackgroundDownload.backgroundWorkerDownload_ProgressChanged;
			MainWindowElement.backgroundWorkerDownload.RunWorkerCompleted += BackgroundDownload.backgroundWorkerDownload_RunWorkerCompleted;
			MainWindowElement.backgroundWorkerUpload = ((BackgroundWorker)wnd.FindResource("backgroundWorkerUpload"));
			MainWindowElement.backgroundWorkerUpload.DoWork += BackgroundUpload.backgroundWorkerUpload_DoWork;
			MainWindowElement.backgroundWorkerUpload.ProgressChanged += BackgroundUpload.backgroundWorkerUploadProgressChanged;
			MainWindowElement.backgroundWorkerUpload.RunWorkerCompleted += BackgroundUpload.backgroundWorkerUpload_RunWorkerCompleted;
			MainWindowElement.backgroundWorkerMessenger = ((BackgroundWorker)wnd.FindResource("backgroundWorkerMessenger"));
			MainWindowElement.nameLoadFile = wnd.nameLoadFile;

			if (SettingsData.getRightWrite() == 0)
			{
				MainWindowElement.loadFileToServer.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

	}
}
