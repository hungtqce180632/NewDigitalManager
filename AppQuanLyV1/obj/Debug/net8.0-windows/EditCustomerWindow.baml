   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	    AppQuanLyV1$   AppQuanLyV1.EditCustomerWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	     � Title$  Edit Customer��5   	   $	��350��6   $	��400��6,    � WindowStartupLocation$ CenterOwner=�68    � 	Resources 5      (  "System.Windows.Controls.DatePicker �� 
TargetType �� Property �� Value��%h   '        �� 5   
   "  6   .�5      ��  �@ SelectedDateFormat$ Short=�63   5	      5
      .��5      � $��10q�6   .+�5   
   �� .+�5      �� $��Customer Details��6   $��16(�60   $��0,0,0,15q�6>   � 5       � ColumnDefinitions 5      �� 5      $
��Auto �6'   �� 5      $��* �6'    � RowDefinitions 5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   .+�5      �� $��Name:��6   $��0��6)   $��0��66   $��0,5,10,5q�6F   $��Center=�6X   �� 5      -   5         ��Name$ CustomerNameTextBox��5      $��0��67   $��1��6D   $	��0,5q�6T   $��5q�6a   �� 5      $��Package:��6   $��1��6,   $��0��69   $��0,5,10,5q�6I   $��Center=�6[   �� 5       -   5        	 ��Name$	 CustomerPackageComboBox��5       $��1��6<   $��1��6I   $	��0,5q�6Y   $��5q�6f   .@�5!      �� $
��goi1��6#   �� 5"      $
��goi3��6#   �� 5#      $
��goi6��6#   �� 5$      $��goi12��6#   5%      �� 5'      $��Email/Note:��6   $��2��6/   $��0��6<   $��0,5,10,5q�6L   $��Center=�6^   �� 5(      -   5        $	 CustomerEmailComboBox��5(      $��2��6:   $��1��6G   $	��0,5q�6W   $��5q�6d   
 �� 
IsEditable$

 True��6p   �� 5*      $��Registration Date:��6   $��3��66   $��0��6C   $��0,5,10,5q�6S   $��Center=�6e     5+      -   5          Name$$ CustomerRegistrationDatePicker��5+      $��3��6E   $��1��6R   $	��0,5q�6b    � Language$ vi-VN�5-      �� 5/      $��Expiration Date:��6   $��4��64   $��0��6A   $��0,5,10,5q�6Q   $��Center=�6c     50      -   5        $" CustomerExpirationDatePicker��50      $��4��6C   $��1��6P   $	��0,5q�6`   ${�False��6m   $ vi-VN�51      52      �� 54       �� Orientation$ 
Horizontal=�6   $��Right=�62   $��0,20,0,0q�6N   .+�55      �� -   5         ��Name$ CancelButton��55      $��Cancel��6/   $��80��6@   $��30��6K   $��0,0,10,0q�6W   6i   �� 56      -   5        $ 
SaveButton��56      $
��Save��6-   $��80��6<   $��30��6G   6S   57      58      59      5:      