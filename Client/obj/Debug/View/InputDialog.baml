   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Client.ViewClient.View  5   	       Client.View.InputDialog   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Client.View     5   	     � Title$  InputDialog��5   	    � SizeToContent$ WidthAndHeight=�6    � WindowStartupLocation$ CenterScreen=�6<    � 
ResizeMode$ NoResize=�6a   .��5	      � $��15q�6    � ColumnDefinitions 5
   
   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� -   5        $��lblQuestion��5      .��6#   	Question:6.   �� 5   
   -   5        $��	txtAnswer��5      $��1��6#   $	��250��60   �� 5   
   -   5        $
��flag��5      $��2��6    d� IsThreeState .  $��Private��6A   �� 5   
   -   5        $��
stackPanel��5      $��3��6'   $y�	Collapsed=�64   .+�5      �� -   5        $��pswLabel��5      .��6$   	Password:6/   G� 5      -   5        $��	pswAnswer��5       G� Password$  ��6+   5      � 5   
   $��4��6   $��2��6!   $��Right=�65   $��0,15,0,0q�6Q   .+�5      �� -   5         ��Name$ btnDialogOk��5      	 �� 	IsDefault	 . $��60��6;   $��0,0,10,0q�6I   .��6[   _Ok6`   �� 5      -   5        $ btnDialogCancel��5      
 �� IsCancel
 . $��60��6>   .��6L   	_Cancel6U   5      5      5       