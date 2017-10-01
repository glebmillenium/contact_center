using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using contact_center_application.core;
using Newtonsoft.Json;
using contact_center_application.serialization;

namespace contact_center_application
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			string[] aliance = RequestDataFromServer.primaryExchangeWithSocket();
			for (int i = 0; i < aliance.Length; i++)
			{
				ComboboxFileSystem.Items.Add(aliance[i]);
			}
			ComboboxFileSystem.SelectedItem = aliance[0];

			string answer = 
				RequestDataFromServer.getCatalogFileSystem(ComboboxFileSystem.SelectedItem.ToString());

			fullingTreeView(answer);

		}

		private void fullingTreeView(string json)
		{
			List<TreeViewItem> listItems = getItemsCatalogsFromJson(json);

			if (listItems.Count != 0)
			{
				foreach (var category in listItems)
				{
					this.treeViewCatalog.Items.Add(category);
				}
			}
		}

		private List<TreeViewItem> getItemsCatalogsFromJson(string json)
		{
				List<TreeViewItem> result = new List<TreeViewItem>();
			List<string> listCatalog =
				   JsonConvert.DeserializeObject<List<string>>(json);
			foreach (var catalog in listCatalog)
			{
				CatalogForSerialization element =
					JsonConvert.DeserializeObject<CatalogForSerialization>(catalog);
				TreeViewItem item = new TreeViewItem();
				item.Header = element.name;
				if (!element.file)
				{
					List<TreeViewItem> listChildren = 
						getItemsCatalogsFromJson(element.content);
					if (listChildren.Count != 0)
					{
						foreach (var children in listChildren)
						{
							item.Items.Add(children);
						}
					}

				}
				result.Add(item);
			}
			return result;
		}

		private void ComboboxFileSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}


	}
}
