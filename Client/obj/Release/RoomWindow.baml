   M S B A M L   `   `   `  ���� A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient      Client.Rooms   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Client       � Title$  Rooms�� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterOwner=� � SizeToContent$ WidthAndHeight=� � Icon Z�  J� CommandBindings )  #System.Windows.Input.CommandBinding  -      Command$ Delete��  -   $
 Open��.��� .+��� ���#F1F2F4.+��� -    ��Name$ updateButton��$��30��$��30��$��Right=� � ToolTip$ Update button������ .R�� .+��� 	 ��Name$	 buttonSurface��$��0�����	#FF919191��Z� �� 
 ��Name$
 contentSurface��$	��⟳��
���   � �� TextAlignment$ Center=�$��Normal"�$��20(� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ���  �� 
TargetName$ buttonSurface�� �� Property � �� �� Value �	#FF7CC5C5�� $ contentSurface�� � ��
 ������� -    ��Name$ listBox��$	��250��$	��300��$��2q�$$ Right click to create new roon�� � ContextMenu �� .@�m� -    m�Name$ createButton��$��Create�� � 	Resources  �� 
TargetType �� Triggers m� Command��%t  '��       �� " ��.���  �@�ToolTip$ Double click to connect����  � ��
 ��İ���  � ��$ 1q���  � ��$ 5q� P�  �@��
IsSelected . . ���  � �� �	#FF838383��  � ��$ 1q���  � ��
 �   ���  � ��$ 2q���  �@�ContextMenu �� .@�m� $��Connect��$
 Open��m� $��Delete��$ Delete��