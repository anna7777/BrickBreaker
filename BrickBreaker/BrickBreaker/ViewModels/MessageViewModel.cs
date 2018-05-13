namespace BrickBreaker.ViewModels
{
    /// <summary>
    /// class that implements the viewmodel of a message
    /// </summary>
    class MessageViewModel : ViewModelBase
    {
        #region binding properties
        private int nick_width;
        public int NickWidth
        {
            get { return nick_width; }
            set
            {
                nick_width = value;
                OnPropertyChanged(nameof(NickWidth));
            }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

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

        public void SetMessage(int actualwidth, string nickname, string message)
        {
            //Width = actualwidth - NickWidth;
            Nickname = nickname;
            Message = message;
        }
    }
}
