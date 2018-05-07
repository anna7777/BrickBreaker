using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Player
    {
        public bool busy { get; set; }
        private TcpClient _tcp;
        public double left { get; set; }
        public double speed { get; set; }
        public TcpClient tcp { get { return _tcp; } set { _tcp = value; } }

        public Player()
        {
            left = 150;
            speed = 15;
        }
    }
}
