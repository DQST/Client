   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	     � Title$  	Chat v0.1��5   	   $	��600��5	   	   $	��400��6   $	��600��5
   	   $	��400��6    � WindowStartupLocation$ CenterScreen=�5   	   ���#F1F2F4.��5      � $��5,0,5,0q�6    � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $��* �6   �� 5      $��80 �6   .+�5   
   o� $	��Top=�6   $��0��6'   $��0��67   $��	-3,0,-3,0q�6D   .@�5      m� $
��File��6   .@�5      m� -   5        $��roomsButton��5      $��Rooms��6.   6=   �� 5      m� 5      -   5        $��
exitButton��5      $
��Exit��6-   5      m� 5      $
��Edit��6   .@�5      m� -   5        $��settingsButton��5      $��Settings��61   6C   5      5      �� 5   
   -   5         ��Name$ 
tabControl��5      $��1��6)   � 5   
   $��2��6   $��0,5,0,5q�6    � ColumnDefinitions 5      �� 5       $��* �6#   �� 5!      $��80 �6#   .+�5#      �� -   5         ��Name$ inputTextBox��5#       �� TextWrapping$
 Wrap=�5$       �� AcceptsReturn . 5%      �� 5&      -   5         ��Name$ 
sendButton��5&      $��1��5'      $
��Send��5(      ��5*      �� 5+      .R�5,      � .+�5-      A� 	 A�Name$	 buttonSurface��6&   ���	#FF65D4F9$��70��5/   &   $��70��50   &   $��Center=�51   &   $��Center=�52   &   $��0��53   &   ���	#FF96C75054   &   �� 55      
 ��Name$
 contentSurface��6$   $
��Send��56   $   $��Center=�57   $   
���   �$��Center=�59   $   $
��Bold"�5:   $   5;       �� Triggers 5<      P� 5=       P� Property �@J�IsMouseOver P� Value . . �5>   "   ��  �� 
TargetName$ buttonSurface��6)    �� Property � �� �� Value �	#FF7FD8F56T   �� 5?   "   $ buttonSurface��6)    � ��$ 2��6_   �� 5@   "   $ contentSurface��6)    � ��
 �����6[   5A      5C      5E      5F      5G      5H      