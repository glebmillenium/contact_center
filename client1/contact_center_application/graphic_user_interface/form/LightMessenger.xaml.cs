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
using System.Windows.Shapes;

namespace contact_center_application.graphic_user_interface.form
{
    /// <summary>
    /// Логика взаимодействия для LightMessenger.xaml
    /// </summary>
    public partial class LightMessenger : Window
    {
        public LightMessenger()
        {
            InitializeComponent();
        }

		public LightMessenger(string message)
		{
			InitializeComponent();
			this.messenger.Text = message;
		}

		public void run()
		{
			for (double i = 0.0; i < 1.0; i += 0.1)
			{
				this.Opacity = i;
				Thread.Sleep(400);
				UpdateLayout();
			}
			this.Close();
		}
	}
}
