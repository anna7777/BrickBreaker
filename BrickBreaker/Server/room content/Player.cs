using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// class, which is a theoretical model of the player
    /// </summary>
    class Player
    {
        public TcpClient tcp { get { return _tcp; } set { _tcp = value; } }
        public bool busy { get; set; }
        public double left { get; set; }
        public double speed { get; set; }
        private TcpClient _tcp;

        public Player()
        {
            left = 150;
            speed = 15;
        }
    }
}
