   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5   	   G CBrickBreaker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:BrickBreaker.ViewModelsBrickBreaker.ViewModels 5   	      BrickBreaker.GameWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   7local%clr-namespace:BrickBreaker.ViewModels     5   	     � Title$  
GameWindow��5	   	    � SizeToContent$ WidthAndHeight=�6    � 
ResizeMode$ NoResize=�6;    � DataContext 5
      1  +BrickBreaker.ViewModels.GameWindowViewModel  5   
    J� InputBindings 5      �� 5   
   
 �� Key$
 Left��6    �� Command 6    �� *MoveLeftCommand+�� 5   
   $ Right��6    6!   �� *MoveRightCommand+�� 5   
   $	 Esc��6    6   �� *
EscCommand+.   (System.Windows.Interactivity.Interaction   Triggers 5      /   )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ Loaded��6   .   (System.Windows.Interactivity.TriggerBase	   Actions.	 5      6   0System.Windows.Interactivity.InvokeCommandAction  
   Command
 6$   �� *WindowLoadedCommand+5        5   
   $ Closed��6   .	 5        
 6$   �� *ClosingCommand+5      .��5       ��  �� Orientation$ 
Horizontal=�6   
���   �.+�5"   
   ��  K� ZIndex$ 2��6   ��6(   �� *items+��5#      �� 5$      .R�5%      �� $��field��5(      $	��518��5%      $	��350��5&      $��1q�5'      
����  �5)      5*      ��5,      �� 5-      .R�5.      �� .+�5/      �� ��6!   �� *Width+��69   �� *Height+50      51      53      �� 55   
   .+�56      �� $��40��6   $��27��57      
 �� Top$ 10��58       �� Left$ 3��59      ��5:      �� *Background1+ �� UpdateSourceTrigger$ PropertyChanged=���5;      �� *Player1Points+$ PropertyChanged=��� 5<      $��40��6   $��27��5=      $ 30��5>      $ 3��5?      ��5@      �� *Background2+$ PropertyChanged=���5A      �� *Player2Points+$ PropertyChanged=��� 5B      $��Center=�5C      $��Center=�5D      $
��0 20q�5E      $��18(�5F      
����� �$��Medium"�5H      ��5B      �� *Map+$ PropertyChanged=� � LayoutTransform 5I      �� 5J       �� Angle$
 -270Q�6&   65   5L      5M      5N      5O      