   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Client.ViewClient.View  5          Client.View.UserTabItem   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      +localclr-namespace:Client.View     5      .��5	      �   � ColumnDefinitions  5
   
   �� 5      $	��100��6   $��* �6.   �� 5      $��25 �6   .+�5   
   �� -   5         ��Name$ Header��5      $��TabItem��5      $��12(�5      $��Center=�5      $��Center=�5      �� 5   
   -   5         ��Name$ closeTabButton��5      $��1��5      ��5      �� 5      .R�5      � .+�5      ��  ��Name$ buttonSurface��6$   $��25��5   $   $��25��5   $   $��Center=�5   $   $��Center=�5   $   ��5   $   Z� 5   $   �� 5       ��Name$ contentSurface��6    $	��✖��5       $��Center=�5        
���   �$��Center=�5"       5#       �� Triggers 5$      P� 5%       P� Property �@J�IsMouseOver P� Value . . �5&      ��  �� 
TargetName$ buttonSurface��6%   	 �� Property	 � ��
 �� Value
 �	#FFFB3B3B6P   �� 5'      $ contentSurface��6%   	 � ��

 �����6W   5(      5*      5,      5-      5.      