using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace Patcher
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			setData(0, "Все компоненты программы обновлены");
			RequestDataFromServer.createConnection();
			string version = SettingsData.getVersion();
			string[] requirementVersionForUpdate = RequestDataFromServer.getDifferenceVersion(version);
			if (requirementVersionForUpdate == null || requirementVersionForUpdate.Length == 0)
			{
				MessageBox.Show("Приложение имеет актуальную версию", "");
				setData(100, "Все компоненты программы обновлены");
			} else {
				string versionString = "";
				foreach (var arg in requirementVersionForUpdate)
				{
					versionString += arg + ", ";
				}
				versionString = versionString.Remove(versionString.Length - 2, 2);
				if (MessageBox.Show("Приложение будет обновлено до версии: "
					+ versionString,
					"Обновление", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
				{
					setData(10, "Начало обновления");
					int i = 0;
					foreach (var arg in requirementVersionForUpdate)
					{
						setData((int) (10 + 90.0 * i / requirementVersionForUpdate.Length), 
							"Обновление до версии: " + arg);
						updateFilesApplication(RequestDataFromServer.getVersion(arg), arg);
						i++;
					}
					SettingsData.setVersion(requirementVersionForUpdate[requirementVersionForUpdate.Length - 1]);
					setData(100, "Все компоненты программы обновлены");
				}
				else
				{
					this.Close();
				};
			}
		}

		private void updateFilesApplication(string[] json, string version, string currentWay = "")
		{
			CatalogForSerialization catalogSerialization;
			foreach (var arg1 in json)
			{
				catalogSerialization =
					JsonConvert.DeserializeObject<CatalogForSerialization>(arg1);
				byte[] dataWriteToFile;
				if (catalogSerialization.file)
				{
					if (!Directory.Exists(".\\" + currentWay))
					{
						Directory.CreateDirectory(".\\" + currentWay);
					}
					dataWriteToFile = RequestDataFromServer.getFileForUpdateNewVersion(version, currentWay);
					File.WriteAllBytes(".\\" + currentWay + "\\" + catalogSerialization.name, dataWriteToFile);
				}
				if (catalogSerialization.content != null)
				{
					updateFilesApplication(JsonConvert.DeserializeObject<string[]>(catalogSerialization.content), version, 
						currentWay + "\\" + catalogSerialization.name);
				}
			}
		}

		private void setData(int value, string info)
		{
			this.bar.Value = value;
			this.info.Text = info;
		}
	}
}
