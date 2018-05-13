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
    /// <summary>
    /// class that implements the viewmodel of a menu
    /// </summary>
    public class MenuWindowViewModel : ViewModelBase
    {
        #region variables
        IContract con;
        #endregion

        #region commands
        public ICommand SelectRoomCommand { get; set; }
        public ICommand WindowLoadedCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand WindowClosedCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        #endregion

        #region collections
        public ObservableCollection<Button> rooms = new ObservableCollection<Button>();
        public ObservableCollection<Message> ChatContent { get; set; }
        ListBox lbChat;
        #endregion

        #region binding properties
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
        #endregion

        /// <summary>
        /// constructor, which initializes all objects with initial values
        /// and connects to the server
        /// </summary>
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

        /// <summary>
        /// initializes chat, when the menuwindow was loaded
        /// </summary>
        /// <param name="obj"></param>
        private void WindowLoadedCommandExecute(object obj)
        {
            lbChat = obj as ListBox;
        }

        /// <summary>
        /// send message, when we press "Send" or Enter
        /// </summary>
        /// <param name="obj"></param>
        private void SendMessageCommandExecute(object obj)
        {
            if (Nickname == null || Nickname.Trim() == "")
                Nickname = "Anonim";
            con.SendMessage(Nickname, Message);
            Message = "";
        }

        /// <summary>
        /// scroll to the last message in chat
        /// </summary>
        /// <param name="message">last message</param>
        public void Scroll(Message message)
        {
            lbChat.ScrollIntoView(message);
        }

        /// <summary>
        /// exit command
        /// </summary>
        /// <param name="obj"></param>
        private void ExitCommandExecute(object obj)
        {
            Button exit = obj as Button;
            ChangeVisibilityWhenExit(exit);
            con.Exit();
        }

        /// <summary>
        /// select room, when player press roombutton
        /// </summary>
        /// <param name="obj"></param>
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

        /// <summary>
        /// hides the exit button after leaving the room
        /// </summary>
        /// <param name="exit"></param>
        public void ChangeVisibilityWhenExit(Button exit)
        {
            exit.Visibility = Visibility.Hidden;
        }

        //disconnect from the server, when we close game
        private void WindowClosedCommandExecute(object obj)
        {
            con.Disconnect();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// exit the game room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Exit(object sender, RoutedEventArgs e)
        {
            Button exit = sender as Button;
            ChangeVisibilityWhenExit(exit);
            con.Exit();
            e.Handled = true;
        }
    }

}
