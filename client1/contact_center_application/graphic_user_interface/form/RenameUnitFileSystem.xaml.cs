using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace contact_center_application.graphic_user_interface.form
{
	/// <summary>
	/// Логика взаимодействия для RenameUnitFileSystem.xaml
	/// </summary>
	public partial class RenameUnitFileSystem
	{
		public RenameUnitFileSystem(string nameFile)
		{
			InitializeComponent();
			this.newNameFile.Text = nameFile;
			this.newNameFile.Focus();
			this.newNameFile.SelectionStart = nameFile.Length;
			this.newNameFile.SelectAll();
		}

		public RenameUnitFileSystem(bool create)
		{
			InitializeComponent();
			if (create)
			{
				this.activeButton.Content = "Создать";
				this.textView.Text = "Введите имя папки";
				this.newNameFile.Focus();
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

		private void newNameFile_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (e.Text.Equals(@"\") || e.Text.Equals(@"/") || e.Text.Equals(@":") ||
				e.Text.Equals(@"*") || e.Text.Equals(@"?") || e.Text.Equals("\"") ||
				e.Text.Equals("<") || e.Text.Equals(">") || e.Text.Equals("|"))
			{
				this.warningSymbols.IsOpen = true;
				e.Handled = true;
			}
		}
	}
}
