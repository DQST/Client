   M S B A M L   `   `   `  ���� A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient      Client.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Client       � Title$  	Chat v0.1��$	��600��$	��400��$	��600��$	��400�� � WindowStartupLocation$ CenterScreen=����#F1F2F4.��� $��5,0,5,0q� � RowDefinitions �� $��20 ��� $��* ��� $��80 �.+�o� $	��Top=�$��0��$��0��$��	-3,0,-3,0q�.@�m� $
��File��.@�m� -   $��roomsButton��$��Rooms���� m� -   $��
exitButton��$
��Exit��m� $
��Edit��.@�m� -   $��settingsButton��$��Settings���� -    ��Name$ 
tabControl��$��1��� $��2��$��0,5,0,5q� � ColumnDefinitions �� $��* ��� $��80 �.+��� -    ��Name$ inputTextBox�� �� TextWrapping$
 Wrap=� �� AcceptsReturn . �� -    ��Name$ 
sendButton��$��1��$
��Send������ .R�� .+�A� 	 A�Name$	 buttonSurface�����	#FF65D4F9$��70��$��70��$��Center=�$��Center=�$��0�����	#FF96C750�� 
 ��Name$
 contentSurface��$
��Send��$��Center=�
���   �$��Center=�$
��Bold"� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ���  �� 
TargetName$ buttonSurface�� �� Property � �� �� Value �	#FF7FD8F5�� $ buttonSurface�� � ��$ 2���� $ contentSurface�� � ��
 �����