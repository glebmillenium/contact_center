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
	/// Логика взаимодействия для RenameUnitFileSystem.xaml
	/// </summary>
	public partial class RenameUnitFileSystem : Window
	{
		public RenameUnitFileSystem()
		{
			InitializeComponent();
		}

		public RenameUnitFileSystem(bool create)
		{
			InitializeComponent();
			if (create)
			{
				this.activeButton.Content = "Создать";
				this.textView.Text = "Введите имя папки";
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		public string getNameFile()
		{
			return this.newNameFile.Text;
		}
	}
}
