using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// class, which is a theoretical model of the room
    /// </summary>
    class Room
    {
        #region properties
        public Player player1 { get; set; }
        public Player player2 { get; set; }
        public double ball_speed { get; set; }
        public double ball_top { get; set; }
        public double ball_left { get; set; }
        public int direction { get; set; }
        public double k { get; set; }
        public Goal goal { get; set; }
        public List<Brick> bricks_p1 { get; set; }
        public List<Brick> bricks_p2 { get; set; }
        #endregion

        /// <summary>
        /// constructor, which initializes all objects with initial values
        /// </summary>
        public Room()
        {
            player1 = new Player();
            player2 = new Player();
            goal = new Goal();
            ball_speed = 6;
            ball_top = 25;
            ball_left = 169;
            direction = 1;
            k = 0;
        }

        /// <summary>
        /// change ball direction
        /// </summary>
        public void ChangeDirection()
        {
            direction *= -1;
        }
    }
}
