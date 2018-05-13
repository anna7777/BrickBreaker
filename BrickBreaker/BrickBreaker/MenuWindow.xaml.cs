using BrickBreaker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrickBreaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateRooms();
        }
        /// <summary>
        /// create 50 rooms in menuwindow
        /// </summary>
        private void CreateRooms()
        {
            int n = 75;
            MenuWindowViewModel viewmodel = this.DataContext as MenuWindowViewModel;
            for (int i = 0; i < n; i++)
            {
                #region --- create room --- 
                Button tmp = new Button();
                Grid.SetColumn(tmp, i % 5);
                Grid.SetRow(tmp, i / 5);
                tmp.Margin = new Thickness(5);
                tmp.Command = viewmodel.SelectRoomCommand;
                tmp.CommandParameter = tmp;
                tmp.Background = Brushes.Blue;
                #endregion

                #region  --- exit --- 
                Button tmpchild = new Button();
                tmpchild.MinWidth = 40;
                tmpchild.MinHeight = 15;
                tmpchild.Content = "Exit";
                tmpchild.Visibility = Visibility.Hidden;
                tmpchild.Command = viewmodel.ExitCommand;
                tmpchild.CommandParameter = tmpchild;
                //tmpchild.AddHandler(Button.ClickEvent, new RoutedEventHandler(viewmodel.Exit));
                tmp.Content = tmpchild;
                #endregion

                viewmodel.rooms.Add(tmp);
                gRooms.Children.Add(tmp);
            }
        }
    }


}
