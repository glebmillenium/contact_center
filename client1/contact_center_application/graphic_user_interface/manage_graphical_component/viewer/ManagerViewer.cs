using contact_center_application.core;
using contact_center_application.core.storage_dynamic_data;
using contact_center_application.graphic_user_interface.manage_graphical_component.tree_view;
using MoonPdfLib;
using MoonPdfLib.MuPdf;
using Spire.Doc;
using Spire.DocViewer.Wpf;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;

namespace contact_center_application.graphic_user_interface.manage_graphical_component.viewer
{
	class ManagerViewer
	{
		public static DocumentViewer viewer = new DocumentViewer(); //viewerTab
		public static TextBlock otherViewer = new TextBlock(); //otherViewer
		public static MoonPdfPanel moonPdfPanel = new MoonPdfPanel(); //background light-gray
		public static DocViewer docViewer = new DocViewer();
		public static Image image = new Image();
		public static XpsDocument doc;
		public static Document simpleDoc;
		public static RichTextBox textbox;
		public static bool updateCatalog = false;
		public static string currentView = "view/temp1";

		/// <summary>
		/// Загружает данные в правую часть окна
		/// </summary>
		/// <param name="way"></param>
		/// <param name="viewWay"></param>
		public static void LoadToViewer(string way, string viewWay)
		{
			if (SettingsData.isOpenNow())
			{
				EventsForButtons.openFile();
			}
			CurrentDataOpenFile.dateOpenFile = File.GetLastWriteTime(way);
			CurrentDataOpenFile.relationWayOpenFile = CurrentDataFileSystem.listTreeView[CurrentDataFileSystem.searchSelectedItem().Item2].Item2;
			MainWindowElement.nameLoadFile.Text = Path.GetFileName(CurrentDataOpenFile.openFile);
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
					if (SettingsData.isViewTxtCsv())
					{
						viewTxtCsvHtmlXml(way);
					}
					else
					{
						notViewContentFile();
					}
				}
				else if (extension.Equals(".jpeg") || extension.Equals(".tiff") ||
					extension.Equals(".jpg") || extension.Equals(".png"))
				{
					if (SettingsData.isViewJpegTiffJpgPng())
					{
						viewJpegTiffJpgPng(way);
					}
					else
					{
						notViewContentFile();
					}
				}
				else if (extension.Equals(".doc") || extension.Equals(".docx"))
				{
					if (SettingsData.isViewDocDocx())
					{
						viewDocDocx(way, viewWay);
					}
					else
					{
						notViewContentFile();
					}
				}
				else if (extension.Equals(".xlsx") || extension.Equals(".xls"))
				{
					if (SettingsData.isViewXlsXlsx())
					{
						viewXlsXlsx(way, viewWay);
					}
					else
					{
						notViewContentFile();
					}
				}
				else if (extension.Equals(".pdf"))
				{
					if (SettingsData.isViewPdf())
					{
						viewPdf(way);
					}
					else
					{
						notViewContentFile();
					}
				}
				else
				{
					MainWindowElement.tabControl.SelectedItem = MainWindowElement.otherTabViewer;
					otherViewer.Text = "Формат загруженного файла неизвестен";
				}
			}
		}

		private static void notViewContentFile()
		{
			MainWindowElement.tabControl.SelectedItem = MainWindowElement.otherTabViewer;
			otherViewer.Text = "Режим отображения файла отключен, " +
				"Содержимое файла можно просмотреть в любом внешнем приложении. " +
				"Для просмотра в программе установите соотвествующий" +
				" флаг в настройках приложения";
		}

		private static void viewPdf(string way)
		{
			Logger.log("Отображение файла будет в pdfviewer");
			MainWindowElement.tabControl.SelectedItem = MainWindowElement.pdfViewerTab;
			byte[] bytes = File.ReadAllBytes(way);
			var source = new MemorySource(bytes);

			ManagerViewer.moonPdfPanel.Open(source);
			ManagerViewer.moonPdfPanel.PageRowDisplay =
				MoonPdfLib.PageRowDisplayType.ContinuousPageRows;
		}

		private static void viewXlsXlsx(string way, string viewWay)
		{
			Logger.log("Отображение файла будет в viewer");
			MainWindowElement.tabControl.SelectedItem = MainWindowElement.viewerTab;
			ManagerViewer.viewer.Document = null;
			ManagerViewer.viewer.DataContext = null;
			if (ManagerViewer.doc != null)
			{
				ManagerViewer.doc.Close();
			}
			ManagerViewer.doc = null;

			try
			{
				convertXlsToXps(way, viewWay);
				ManagerViewer.doc = new XpsDocument(viewWay, FileAccess.Read);
				ManagerViewer.viewer.Document = ManagerViewer.doc.GetFixedDocumentSequence();
				ManagerViewer.doc.Close();
			}
			catch (Exception exp)
			{
				ManagerViewer.doc = null;
				ManagerViewer.viewer.Document = null;
				ManagerViewer.viewer.DataContext = null;
				Logger.log(exp.Message);
			}
		}

		private static void viewDocDocx(string way, string viewWay)
		{
			if ((bool)MainWindowElement.switchModelViewButton.IsChecked)
			{
				Logger.log("Отображение файла будет в DocumentViewer");
				MainWindowElement.tabControl.SelectedItem = MainWindowElement.viewerTab;

				ManagerViewer.viewer.Document = null;
				ManagerViewer.viewer.DataContext = null;
				if (ManagerViewer.doc != null)
				{
					ManagerViewer.doc.Close();
				}

				convertDocxDocToXps(way, viewWay);
				ManagerViewer.doc = new XpsDocument(viewWay, FileAccess.Read);
				ManagerViewer.viewer.Document = ManagerViewer.doc.GetFixedDocumentSequence();
				ManagerViewer.doc.Close();
			}
			else
			{
				Logger.log("Отображение файла будет в docViewer");
				ManagerViewer.docViewer.CloseDocument();
				ManagerViewer.docViewer.LoadFromFile(way);
				MainWindowElement.tabControl.SelectedItem = MainWindowElement.docViewerTab;
			}
		}

		private static void viewJpegTiffJpgPng(string way)
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
			ManagerViewer.image.Source = bitmap33;
			MainWindowElement.tabControl.SelectedItem = MainWindowElement.imageTab;
		}

		private static void viewTxtCsvHtmlXml(string way)
		{
			Logger.log("Отображение файла будет в TextBox");
			MainWindowElement.tabControl.SelectedItem = MainWindowElement.textboxTab;
			displayTextbox(way);
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
				simpleDoc = new Document(way);
				string directoryWay = Path.GetDirectoryName(viewWay);
				Directory.CreateDirectory(directoryWay);
				if (File.Exists(viewWay))
				{
					File.Delete(viewWay);
				}
				simpleDoc.SaveToFile(viewWay, Spire.Doc.FileFormat.XPS);
				simpleDoc.Close();
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
		/// Отображает содержимое файла (текстового) в RichTextBox textbox
		/// </summary>
		/// <param name="viewWay"></param>
		public static void displayTextbox(string viewWay)
		{
			textbox.Height = ManagerViewer.viewer.Height;
			textbox.Width = ManagerViewer.viewer.Width;
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

		public static void callGarbage()
		{
			ManagerViewer.image = null;
			ManagerViewer.docViewer = null;
			ManagerViewer.viewer = null;
			ManagerViewer.moonPdfPanel = null;
			ManagerViewer.doc = null;

			MainWindowElement.window.UpdateLayout();
			GC.Collect(2, GCCollectionMode.Forced);
			GC.WaitForPendingFinalizers();

			ManagerViewer.viewer = new DocumentViewer(); //viewerTab
			ManagerViewer.moonPdfPanel = new MoonPdfPanel(); //background light-gray
			ManagerViewer.docViewer = new DocViewer();
			ManagerViewer.image = new Image();

			MainWindowElement.docViewerStackPanel.Children.Clear();
			MainWindowElement.docViewerStackPanel.Children.Add(ManagerViewer.docViewer);

			MainWindowElement.viewerStackPanel.Children.Clear();
			MainWindowElement.viewerStackPanel.Children.Add(ManagerViewer.viewer);

			MainWindowElement.pdfViewerStackPanel.Children.Clear();
			MainWindowElement.pdfViewerStackPanel.Children.Add(ManagerViewer.moonPdfPanel);

			MainWindowElement.imageStackPanel.Children.Clear();
			MainWindowElement.imageStackPanel.Children.Add(ManagerViewer.image);

			if (File.Exists(ManagerViewer.currentView))
			{
				File.Delete(ManagerViewer.currentView);
			}

			ChangeSizeViewer();
		}

		public static void ChangeSizeViewer()
		{
			if (MainWindowElement.window.ActualHeight - 182 <= 0) return;
			ManagerViewer.viewer.Height = MainWindowElement.window.ActualHeight - 182;
			ManagerViewer.textbox.Height = ManagerViewer.viewer.Height;
			ManagerViewer.textbox.Width = ManagerViewer.viewer.Width;
			ManagerViewer.image.Height = ManagerViewer.viewer.Height;
			ManagerViewer.image.Width = ManagerViewer.viewer.Width;
			ManagerViewer.docViewer.Height = ManagerViewer.viewer.Height;
			ManagerViewer.docViewer.Width = ManagerViewer.viewer.Width;
			ManagerViewer.moonPdfPanel.Height = ManagerViewer.viewer.Height;
			ManagerViewer.moonPdfPanel.Width = ManagerViewer.viewer.Width;
			ManagerViewer.otherViewer.Height = ManagerViewer.viewer.Height;
			ManagerViewer.otherViewer.Width = ManagerViewer.viewer.Width;
		}

		public static void switchViewXPSMode()
		{
			string extension = Path.GetExtension(CurrentDataOpenFile.openFile);
			if ((bool)MainWindowElement.switchModeViewButtonXPS.IsChecked)
			{
				MainWindowElement.switchModeViewButtonXPS.Background =
					new SolidColorBrush(Colors.DarkCyan);
			}
			else
			{
				MainWindowElement.switchModeViewButtonXPS.Background =
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
	}
}
