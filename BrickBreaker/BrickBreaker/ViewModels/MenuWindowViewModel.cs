using Contract;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;

namespace BrickBreaker.ViewModels
{
    public class MenuWindowViewModel : ViewModelBase
    {
        IContract con;
        public ObservableCollection<Button> rooms = new ObservableCollection<Button>();
        public ICommand SelectRoomCommand { get; set; }
        public ICommand WindowLoadedCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand WindowClosedCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public ObservableCollection<Message> ChatContent { get; set; }

        ListBox lbChat;

        private string nickname;
        public string Nickname
        {
            get { return nickname; }
            set
            {
                nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public MenuWindowViewModel()
        {
            WindowLoadedCommand = new RelayCommand(WindowLoadedCommandExecute);
            SelectRoomCommand = new RelayCommand(SelectRoomCommandExecute);
            SendMessageCommand = new RelayCommand(SendMessageCommandExecute);
            WindowClosedCommand = new RelayCommand(WindowClosedCommandExecute);
            ExitCommand = new RelayCommand(ExitCommandExecute);

            ChatContent = new ObservableCollection<Message>();

            con = new ClientNet(this);
            con.Connect();
        }

        private void WindowLoadedCommandExecute(object obj)
        {
            lbChat = obj as ListBox;
        }

        private void SendMessageCommandExecute(object obj)
        {
            if (Nickname == null || Nickname.Trim() == "")
                Nickname = "Anonim";
            con.SendMessage(Nickname, Message);
            Message = "";
        }

        public void Scroll(Message message)
        {
            lbChat.ScrollIntoView(message);
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
