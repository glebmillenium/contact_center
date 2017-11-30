using contact_center_application.core;
using contact_center_application.core.storage_dynamic_data;
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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;

namespace contact_center_application.graphic_user_interface.manage_graphical_component.viewer
{
	class ManagerViewer
	{
		public static DocumentViewer viewer = new DocumentViewer(); //viewerTab
		public static MoonPdfPanel moonPdfPanel = new MoonPdfPanel(); //background light-gray
		public static DocViewer docViewer = new DocViewer();
		public static Image image = new Image();
		public static XpsDocument doc;
		public static Document simpleDoc;
		public static RichTextBox textbox;
		public static string currentView = "view/temp1";

		/// <summary>
		/// Загружает данные в правую часть окна
		/// </summary>
		/// <param name="way"></param>
		/// <param name="viewWay"></param>
		public static void LoadToViewer(string way, string viewWay)
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
					MainWindowElement.tabControl.SelectedItem = MainWindowElement.textboxTab;
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
					ManagerViewer.image.Source = bitmap33;
					MainWindowElement.tabControl.SelectedItem = MainWindowElement.imageTab;
				}
				else if (extension.Equals(".doc") || extension.Equals(".docx"))
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
				else if (extension.Equals(".xlsx") || extension.Equals(".xls"))
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
				else if (extension.Equals(".pdf"))
				{
					Logger.log("Отображение файла будет в pdfviewer");
					MainWindowElement.tabControl.SelectedItem = MainWindowElement.pdfViewerTab;
					byte[] bytes = File.ReadAllBytes(way);
					var source = new MemorySource(bytes);

					ManagerViewer.moonPdfPanel.Open(source);
					ManagerViewer.moonPdfPanel.PageRowDisplay =
						MoonPdfLib.PageRowDisplayType.ContinuousPageRows;
				}
				else
				{

				}

			}
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
			GC.Collect();
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
			if (MainWindowElement.window.ActualHeight - 165 <= 0) return;
			ManagerViewer.viewer.Height = MainWindowElement.window.ActualHeight - 165;
			ManagerViewer.textbox.Height = ManagerViewer.viewer.Height;
			ManagerViewer.textbox.Width = ManagerViewer.viewer.Width;
			ManagerViewer.image.Height = ManagerViewer.viewer.Height;
			ManagerViewer.image.Width = ManagerViewer.viewer.Width;
			ManagerViewer.docViewer.Height = ManagerViewer.viewer.Height;
			ManagerViewer.docViewer.Width = ManagerViewer.viewer.Width;
			ManagerViewer.moonPdfPanel.Height = ManagerViewer.viewer.Height;
			ManagerViewer.moonPdfPanel.Width = ManagerViewer.viewer.Width;
		}
	}
}
