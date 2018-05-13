using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker.ViewModels
{

    class MessageViewModel : ViewModelBase
    {
        //private string width;
        //public string Width
        //{
        //    get { return width; }
        //    set
        //    {
        //        width = value;
        //        OnPropertyChanged(nameof(Width));
        //    }
        //}

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

        public void SetMessage(int actualwidth, string nickname, string message)
        {
            //Width = actualwidth;
            Nickname = nickname;
            Message = message;
        }
    }
}
