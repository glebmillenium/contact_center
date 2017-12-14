using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
			try
			{
				setData(0, "Все компоненты программы обновлены");
				Thread.Sleep(1000);
				RequestDataFromServer.createConnection();
				string version = SettingsData.getVersion();
				string[] requirementVersionForUpdate = RequestDataFromServer.getDifferenceVersion(version);
				if (requirementVersionForUpdate == null)
				{
					MessageBox.Show("Не удалось получить сведения из сервера", "");
					setData(0, "");
				}
				else
				{
					if (requirementVersionForUpdate.Length == 0)
					{
						MessageBox.Show("Приложение имеет актуальную версию", "");
						setData(100, "Все компоненты программы обновлены.");
						Process.Start("АРМ оператора контакт-центра.exe");
					}
					else
					{
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
								setData((int)(10 + 90.0 * i / requirementVersionForUpdate.Length),
									"Обновление до версии: " + arg);
								try
								{
									updateFilesApplication(RequestDataFromServer.getVersion(arg), arg);
								}
								catch (Exception e)
								{
									this.info.Text = e.Message + " " + e.InnerException + " " + e.StackTrace;
								}
								i++;
							}
							SettingsData.setVersion(requirementVersionForUpdate[requirementVersionForUpdate.Length - 1]);
							setData(100, "Все компоненты программы успешно обновлены.");
							Process.Start("АРМ оператора контакт-центра.exe");
						}
						else
						{
							this.Close();
						};
					}
				}
			}
			catch (Exception e)
			{
				this.info.Text = e.Message + " " + e.InnerException + " " + e.StackTrace;
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
					dataWriteToFile = RequestDataFromServer.getFileForUpdateNewVersion(version, currentWay + "//" + catalogSerialization.name);
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
