using contact_center_application.core;
using contact_center_application.core.storage_dynamic_data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace contact_center_application.graphic_user_interface.form
{
	/// <summary>
	/// Логика взаимодействия для Authorization.xaml
	/// </summary>
	public partial class Authorization : Window
	{
		public Authorization()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Tuple<String, int, String> result = RequestDataFromServer.getRightAccess(loginTextBox.Text, passwordTextBox.Password);
				string version = SettingsData.getVersion();
				if (result.Item2 == -1)
				{
					infoPopup.IsOpen = true;
					infoTextBlock.Background = Brushes.GhostWhite;
					infoTextBlock.Foreground = Brushes.Black;
					infoTextBlock.Text = "Неверный логин и пароль. " + result.Item3;
				}
				else
				{
					if (!result.Item1.Equals(version))
					{
						if (MessageBox.Show("Текущая версия клиента: " + version
							+ " является устаревшей. Актуальная версия: "
							+ result.Item1 + " Необходимо произвести обновление!",
							"Требуется обновление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
						{
							Process.Start("Patcher.exe");
							this.Close();
						}
						else
						{
							infoPopup.IsOpen = true;
							infoTextBlock.Background = Brushes.GhostWhite;
							infoTextBlock.Foreground = Brushes.Red;
							infoTextBlock.Text = "Версия вашего приложения: "
								+ version + " Актуальная версия: " + result.Item1;
						}
					}
					else
					{
						MainWindow mw = new MainWindow();
						SettingsData.setRightAccess(result.Item2);
						mw.Show();
						this.Close();
					}
				}
			}
			catch (Exception exp)
			{
				infoPopup.IsOpen = true;
				infoTextBlock.Background = Brushes.GhostWhite;
				infoTextBlock.Foreground = Brushes.Black;
				infoTextBlock.Text = "Не удалось подключиться к серверу. Повторите попытку позже";
			}
		}

		private void exitButton_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Вы уверены, что хотите выйти из приложения?",
						"Выход", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				this.Close();
			}
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			new Settings(true).ShowDialog();
		}
	}
}
