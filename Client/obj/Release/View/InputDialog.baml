   M S B A M L   `   `   `  ���� A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Client.ViewClient.View      Client.View.InputDialog   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:Client.View       � Title$  InputDialog�� � SizeToContent$ WidthAndHeight=� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ NoResize=�.��� $��15q� � ColumnDefinitions �� $��* � � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -   $��lblQuestion��.��	Question:�� -   $��	txtAnswer��$��1��$	��250���� -   $
��flag��$��2�� d� IsThreeState .  $��Private���� -   $��
stackPanel��$��3��$y�	Collapsed=�.+��� -   $��pswLabel��.��	Password:G� -   $��	pswAnswer�� G� Password$  ��� $��4��$��2��$��Right=�$��0,15,0,0q�.+��� -    ��Name$ btnDialogOk��	 �� 	IsDefault	 . $��60��$��0,0,10,0q�.��_Ok�� -   $ btnDialogCancel��
 �� IsCancel
 . $��60��.��	_Cancel