using Contract;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;

namespace BrickBreaker
{
    public class MenuWindowViewModel : ViewModelBase
    {
        IContract con;
        public ObservableCollection<Button> rooms = new ObservableCollection<Button>();
        public ICommand SelectRoomCommand { get; set; }
        public ICommand WindowClosedCommand { get; set; }
        public ICommand ExitCommand { get; set; }


        public MenuWindowViewModel()
        {
            SelectRoomCommand = new RelayCommand(SelectRoomCommandExecute);
            WindowClosedCommand = new RelayCommand(WindowClosedCommandExecute);
            ExitCommand = new RelayCommand(ExitCommandExecute);
            con = new ClientNet(this);
            //chat.SetClient((ClientNet)con);
            con.Connect();
        }

        private void ExitCommandExecute(object obj)
        {
            Button exit = obj as Button;
            ChangeVisibilityWhenExit(exit);
            con.Exit();
        }

        private void SelectRoomCommandExecute(object obj)
        {
            Button tmp = obj as Button;
            Button exit = tmp.Content as Button;
            bool select_room = true;
            Button room = rooms.FirstOrDefault(x => (x.Content as Button).Visibility == Visibility.Visible);
            if (room != null)
                select_room = false;
            if (select_room)
            {
                con.SelectARoom(rooms.IndexOf(tmp));
                if (tmp.Background != Brushes.Red)
                    exit.Visibility = Visibility.Visible;
            }
        }

        public void ChangeVisibilityWhenExit(Button exit)
        {
            exit.Visibility = Visibility.Hidden;
        }

        private void WindowClosedCommandExecute(object obj)
        {
            con.Disconnect();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        public void Exit(object sender, RoutedEventArgs e)
        {
            Button exit = sender as Button;
            ChangeVisibilityWhenExit(exit);
            con.Exit();
            e.Handled = true;
        }
    }

}
