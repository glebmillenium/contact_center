using System.Windows;
using System.Windows.Controls;

namespace contact_center_application.graphic_user_interface.manage_graphical_component
{
	public class TemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			//получаем вызывающий контейнер
			FrameworkElement element = container as FrameworkElement;

			if (element != null && item != null && item is int)
			{
				int currentItem = 0;

				int.TryParse(item.ToString(), out currentItem);

				if (currentItem == 0)
					return element.FindResource("ButtonTemplate") as DataTemplate;
				if (currentItem == 1)
					return element.FindResource("TextBlockTemplate") as DataTemplate;
				if (currentItem == 2)
					return element.FindResource("RadioButtonsTemplate") as DataTemplate;
				if (currentItem == 3)
					return element.FindResource("LabelTemplate") as DataTemplate;
			}
			return null;
		}
	}
}
