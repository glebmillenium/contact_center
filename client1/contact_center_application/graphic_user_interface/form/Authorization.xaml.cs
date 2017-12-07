using contact_center_application.core;
using contact_center_application.core.storage_dynamic_data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			Tuple<String, int, String> result = RequestDataFromServer.getRightAccess(loginTextBox.Text, passwordTextBox.Password);
			string version = SettingsData.getVersion();
			if (!result.Item1.Equals(version))
			{
				MessageBox.Show("Текущая версия клиента: " + version
					+ " является устаревшей. Актуальная версия: "
					+ result.Item1 + " Вам необходимо произвести обновление!", "Требуется обновление");
			} else {
				if (result.Item2 == -1)
				{

				}
				else
				{
					MainWindow mw = new MainWindow();
					mw.Show();
					this.Close();
				}
			}
		}
	}
}
