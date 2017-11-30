using contact_center_application.graphic_user_interface.form;
using System;
using System.Collections.Generic;
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

		public static void initialize(MainWindow wnd)
		{
			MainWindowElement.cursor = wnd.Cursor;
			MainWindowElement.tabControl = wnd.tabControlForViewer;
			MainWindowElement.textboxTab = wnd.textboxTabForViewer;
			MainWindowElement.docViewerTab = wnd.docViewerTabForViewer;
			MainWindowElement.pdfViewerTab = wnd.pdfViewerTabForViewer;
			MainWindowElement.imageTab = wnd.imageTabForViewer;
			MainWindowElement.viewerTab = wnd.viewerTabForViewer;
			MainWindowElement.switchModelViewButton = wnd.switchModelViewButtonXPS;
			MainWindowElement.docViewerStackPanel = wnd.docViewerOnStackPanel;
			MainWindowElement.viewerStackPanel = wnd.viewerOnStackPanel;
			MainWindowElement.pdfViewerStackPanel = wnd.pdfViewerOnStackPanel;
			MainWindowElement.imageStackPanel = wnd.imageOnStackPanel;
			MainWindowElement.progressConvertation = wnd.progressOnConvertation;
			MainWindowElement.managerPanel = wnd.managerOnPanel;
			MainWindowElement.window = wnd;
			MainWindowElement.registrButton = wnd.registrOnButton;
		}

	}
}
