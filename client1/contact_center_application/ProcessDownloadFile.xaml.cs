using contact_center_application.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace contact_center_application
{
	/// <summary>
	/// Логика взаимодействия для ProcessDownloadFile.xaml
	/// </summary>
	public partial class ProcessDownloadFile : Page
	{
		public ProcessDownloadFile()
		{
			InitializeComponent();
		}

		public byte[] getContentFile(int index, string relativeWay)
		{
			setStateCurrentProgress(5, "Подготовка к загрузке");
			setStateCurrentProgress(10,	"Загрузка файла...");

			byte[] contentFile = RequestDataFromServer.getContentFile(
				index.ToString(),
				relativeWay, progressBarControllProcess, textBlockControllProcess);
			setStateCurrentProgress(90, "Сохранение файла...");
			return contentFile;
		}

		public void setStateCurrentProgress(
			int valueInPersisten, string decribeState)
		{
			Thread.Sleep(500);
			progressBarControllProcess.Value = valueInPersisten;
			textBlockControllProcess.Text = decribeState;
		}
	}
}
