   M S B A M L   `   `   `  ���� U  Qcontact_center_application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:contact_center_application.formcontact_center_application.form  E AMoonPdfLib, Version=0.3.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:MoonPdfLib;assembly=MoonPdfLib
MoonPdfLib ^ ZSpire.DocViewer.Wpf, Version=2.4.25.5241, Culture=neutral, PublicKeyToken=663f351905198cb3V>clr-namespace:Spire.DocViewer.Wpf;assembly=Spire.DocViewer.WpfSpire.DocViewer.Wpf +    %contact_center_application.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ?local-clr-namespace:contact_center_application.form     <mpp,clr-namespace:MoonPdfLib;assembly=MoonPdfLib     Mmy>clr-namespace:Spire.DocViewer.Wpf;assembly=Spire.DocViewer.Wpf     -     �Name$  window�� � WindowStartupLocation$ CenterScreen=� � Title$: 4Информационный портал ЕЭИСЦ��$	��800��$	��600��.���  � RowDefinitions �� $��45 ��� $��26 ��� $��22 ��� -    ��Name$
 row3���� $��20 �.+��� $��0��.+��� $��Помощь��$��0,5,5,0q�$��20��$	��Top=�$��Right=�$��89���� $2��,Добро пожаловать, Гость!��$��0,0,5,0q�$��Right=��� $��1�� �� Orientation$ 
Horizontal=�.+��� $%��Файловый ресурс: ��$��30��$
��Left=�$	��Top=��� -    ��Name$ ComboboxFileSystem��$��23��$
��Left=�$	��Top=�$	��300���� -    ��Name$ buttonRefresh��$	��150��$��20�� � ToolTip$ Button��$
��Left=�$	��Top=�&   System.Windows.Media.TextOptions	   TextFormattingMode$	 Display=�.���� -   
 ��Name$
 buttonRetryConnection��$-��'Повторить соединение�� �� TextAlignment$ Center=�$
��Wrap=�$��15��$	��132���� $��2��$ 
Horizontal=�.+��� -   $
 UpdateCatalogs��$��5q�$
��Wrap=�$ Center=�$��Bottom=�0.Меню просмотра каталогов�� -   $ ButtonUpdateCatalogs��$��Bottom=�$��60��$��20��$��Обновить��]� $��3��.+�]� $
��Left=�.+��� $	��Top=�$ 
Horizontal=�.+��� $��
Поиск���� -   $	��160��$��20��$��10(�R� -	    R�Name$ treeViewCatalog��$��Bottom=�$��5q�$	��200��$	��400���� -
    ��Name$ stackPanelView��$��Right=�$ Vertical=�.+��� $ 
Horizontal=�$��Right=�$	��550��.+��� -    ��Name$ swtichModeView�� d� 	IsChecked$ FalseY�$'��!Простой интерфейс��$��0,0,10,0q� � UseLayoutRounding . $
��Left=�$��Bottom=�   CheckBoxSwitcher#���   �� -   $=��7Открыть во внешнем приложении��$��20��$��0,0,5,0q�$	��Top=�$	��208���� -   $ swtichInterface��$+��%Сохранить изменения��$��20��$��0,0,5,0q�$	��Top=�$	��141���� -    ��Name$ 
tabControl��.@��� -    ��Name$ 	viewerTab��$y�	Collapsed=�.��Z� -    Z�Name$ viewer��$��5q��� -   $ 
textboxTab��$y�	Collapsed=�.���� -    ��Name$ textbox�� �� 
IsReadOnly . $	��200�� �� AcceptsReturn . " �� VerticalScrollBarVisibility$
 Auto=��� -   $ docViewerTab��$y�	Collapsed=�.��#  Spire.DocViewer.Wpf.DocViewer  -     Name$ 	docViewer��$
��Auto��$��Stretch=�$��0,0,0,0q�$��Stretch=�$
��Auto���� -   $ pdfViewerTab��$y�	Collapsed=�.��  MoonPdfLib.MoonPdfPanel  -     Name$ moonPdfPanel��
��������� -   $ imageTab��$y�	Collapsed=�.���� $
��Auto=�$
��Auto=�.���� -    ��Name$ image���� $��5q�$��4��$ 
Horizontal=�$��Right=�