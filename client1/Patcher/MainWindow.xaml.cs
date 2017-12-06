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

			/**string PathFolderBack = Path.GetDirectoryName(Directory.GetCurrentDirectory());
			SettingsData.setDirectorySettings(PathFolderBack + @"/settings/ip_connect",
				PathFolderBack + @"/settings/port_ftp", PathFolderBack + @"/settings/port_fast",
				PathFolderBack + @"/settings/version");

			string address = SettingsData.getAddress();
			int fastPort = SettingsData.getFastPort();
			int ftpPort = SettingsData.getFtpPort();
			ObjectForSerialization objForSendToFastSocket = new ObjectForSerialization
			{
				command = "test_fast_socket",
			};
			string resultJson = JsonConvert.SerializeObject(objForSendToFastSocket);
			ConnectWithFastSocket.createSocket(address, port);
			string answer = ConnectWithFastSocket.sendMessage(resultJson, 8192);
			ObjectForSerialization objResponseFromFastSocket =
				JsonConvert.DeserializeObject<ObjectForSerialization>(answer);
			if (objResponseFromFastSocket != null
				|| objResponseFromFastSocket.command.Equals("answer_on_test_fast_socket")
				&& objResponseFromFastSocket.param1.Equals("true"))
			{

				Logger.log("Сокет передачи данных по fast_server - открыт");
			}
			else
			{
				Logger.log("Сокет передачи данных по fast_server - закрыт");
			}*/
		}
	}
}
