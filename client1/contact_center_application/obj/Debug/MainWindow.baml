   M S B A M L   `   `   `  ����U  Qcontact_center_application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:contact_center_application.formcontact_center_application.form  5   	   E AMoonPdfLib, Version=0.3.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:MoonPdfLib;assembly=MoonPdfLib
MoonPdfLib 5      ^ ZSpire.DocViewer.Wpf, Version=2.4.25.5241, Culture=neutral, PublicKeyToken=663f351905198cb3V>clr-namespace:Spire.DocViewer.Wpf;assembly=Spire.DocViewer.WpfSpire.DocViewer.Wpf 5      +    %contact_center_application.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ?local-clr-namespace:contact_center_application.form     5   	   <mpp,clr-namespace:MoonPdfLib;assembly=MoonPdfLib     5      Mmy>clr-namespace:Spire.DocViewer.Wpf;assembly=Spire.DocViewer.Wpf     5      -   5          �Name$  window��5
   m    � WindowStartupLocation$ CenterScreen=�5   M    � Title$: 4Информационный портал ЕЭИСЦ��5
   	   $	��800��6-   $	��600��6<   .��5      �  � RowDefinitions 5      �� 5      $��45 �6   6!   �� 5      $��26 �6   6!   �� 5      $��22 �6   6!   �� 5      -   5         ��Name$
 row3��5      6#   �� 5      $��20 �6   6!   .+�5      �� $��0��6   .+�5      �� $��Помощь��6   $��0,5,5,0q�6   $��20��6.   $	��Top=�6:   $��Right=�6R   $��89��6n   �� 5      $2��,Добро пожаловать, Гость!��6   $��0,0,5,0q�6/   $��Right=�6@   5      �� 5      $��1��6    �� Orientation$ 
Horizontal=�6   .+�5      �� $%��Файловый ресурс: ��6   $��30��6'   $
��Left=�63   $	��Top=�6N   �� 5      -   5         ��Name$ ComboboxFileSystem��5      $��23��6*   $
��Left=�5   >   $	��Top=�6Y   $	��300��6q   �� 5      -   5         ��Name$ buttonRefresh��5      $	��150��6#   $��20��6/    � ToolTip$ Button��6;   $
��Left=�6L   $	��Top=�6g   &   System.Windows.Media.TextOptions	   TextFormattingMode$	 Display=�6   .��5      �� -   5        
 ��Name$
 buttonRetryConnection��5      $-��'Повторить соединение��6/    �� TextAlignment$ Center=�6K   $
��Wrap=�6b   $��15��6v   $	��132��6�   5       5!      �� 5#      $��2��6   $ 
Horizontal=�6   .+�5$      �� -   5        $
 UpdateCatalogs��5$      $��5q�6'   $
��Wrap=�62   $ Center=�6F   $��Bottom=�6]   0.Меню просмотра каталогов6�   �� 5%      -   5        $ ButtonUpdateCatalogs��5%      $��Bottom=�6*   $��60��6F   $��20��6Q   $��Обновить��6]   6p   5&      ]� 5)      $��3��6   .+�5+      ]� $
��Left=�6   .+�5,      �� $	��Top=�6   $ 
Horizontal=�6&   .+�5-      �� $��
Поиск��6   �� 5.      -   5        $	��160��5.      $��20��6   $��10(�6'   65   5/      R� 50      -	   5         R�Name$ treeViewCatalog��50   '   $��Bottom=�6   $��5q�6@   $	��200��6K   $	��400��6Z   51      �� 52      -
   5         ��Name$ stackPanelView��52      $��Right=�6(   $ Vertical=�6?   .+�53      �� $ 
Horizontal=�6   $��Right=�6*   $	��550��6F   .+�54      �� -   5         ��Name$ swtichModeView��54   "    d� 	IsChecked$ FalseY�6   $'��!Простой интерфейс��6R   $��0,0,10,0q�6n    � UseLayoutRounding . $
��Left=�6�   $��Bottom=�6�      CheckBoxSwitcher#���   6�   �� 55      -   5        $=��7Открыть во внешнем приложении��55      $��20��66   $��0,0,5,0q�6B   $	��Top=�6S   $	��208��6�   �� 56      -   5        $ swtichInterface��56      $+��%Сохранить изменения��6'   $��20��6E   $��0,0,5,0q�6Q   $	��Top=�6b   $	��141��6z   57      �� 59      -   5         ��Name$ 
tabControl��59      .@�5:      �� -   5         ��Name$ 	viewerTab��5:      $y�	Collapsed=�6"   .��5;      Z� -   5         Z�Name$ viewer��5;   "   $��5q�6   6"   5<      �� 5>      -   5        $ 
textboxTab��5>      $y�	Collapsed=�6#   .��5?      �� -   5         ��Name$ textbox��5?   &    �� 
IsReadOnly . $	��200��67    �� AcceptsReturn . " �� VerticalScrollBarVisibility$
 Auto=�6Y   5@      �� 5B      -   5        $ docViewerTab��5B      $y�	Collapsed=�6%   .��5C      #  Spire.DocViewer.Wpf.DocViewer  -   5          Name$ 	docViewer��5H      $
��Auto��5C      $��Stretch=�5D      $��0,0,0,0q�5E      $��Stretch=�5F      $
��Auto��5G      5H      5I      �� 5K      -   5        $ pdfViewerTab��5K      $y�	Collapsed=�6%   .��5L        MoonPdfLib.MoonPdfPanel  -   5          Name$ moonPdfPanel��5L      
�������5M      5N      �� 5P      -   5        $ imageTab��5P      $y�	Collapsed=�6!   .��5Q      �� $
��Auto=�6   $
��Auto=�6:   .��5R   	   �� -   5         ��Name$ image��5R      5S   	   5T      5U      5W      5X      �� 5Z      $��5q�6   $��4��6   $ 
Horizontal=�6'   $��Right=�6@   5\      5]      5^      