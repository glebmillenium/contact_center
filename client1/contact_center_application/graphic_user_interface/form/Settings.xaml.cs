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
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
		public static int maxSecondsWaitUploadFileSystem = 5;
        public Settings()
        {
            InitializeComponent();
			setDataInView();
        }

		private void setDataInView()
		{
			this.viewDocDocx.IsChecked = SettingsData.isViewDocDocx();
			this.viewXlsXlsx.IsChecked = SettingsData.isViewXlsXlsx();
			this.viewPdf.IsChecked = SettingsData.isViewPdf();
			this.viewJpegTiffJpgPng.IsChecked = SettingsData.isViewJpegTiffJpgPng();
			this.viewTxtCsv.IsChecked = SettingsData.isViewTxtCsv();
			this.openNow.IsChecked = SettingsData.isOpenNow();

			this.ipTextbox.Text = SettingsData.getAddress();
			this.portFastTextbox.Text = SettingsData.getFastPort().ToString();
			this.portFtpTextbox.Text = SettingsData.getFtpPort().ToString();
			this.portReserveTextbox.Text = SettingsData.getReservePort().ToString();
			this.intervalUpdate.Text = SettingsData.getIntervalUpdate().ToString();
		}

		private void saveSettings(object sender, RoutedEventArgs e)
		{
			SettingsData.setViewDocDocx((bool) this.viewDocDocx.IsChecked);
			SettingsData.setViewXlsXlsx((bool) this.viewXlsXlsx.IsChecked);
			SettingsData.setViewPdf((bool) this.viewPdf.IsChecked);
			SettingsData.setViewJpegTiffJpgPng((bool)this.viewJpegTiffJpgPng.IsChecked);
			SettingsData.setViewTxtCsv((bool)this.viewTxtCsv.IsChecked);
			SettingsData.setOpenNow((bool)this.openNow.IsChecked);

			try
			{
				SettingsData.setAddress(this.ipTextbox.Text);
				SettingsData.setFastPort(Int32.Parse(this.portFastTextbox.Text));
				SettingsData.setFtpPort(Int32.Parse(this.portFtpTextbox.Text));
				SettingsData.setReservePort(Int32.Parse(this.portReserveTextbox.Text));
				SettingsData.setIntervalUpdate(Int32.Parse(this.intervalUpdate.Text));
			}
			catch (FormatException formatException)
			{
				MessageBox.Show("","", MessageBoxButton.YesNo);
			}
		}
	}
}
