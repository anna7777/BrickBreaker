using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Server
{
    /// <summary>
    /// class that implements the receipt of commands from the client and server computing
    /// </summary>
    class ServerNet
    {
        #region variables
        static List<Room> rooms = new List<Room>();
        static List<TcpClient> clients = new List<TcpClient>();
        static object locker1 = new object();
        static object locker2 = new object();
        static object locker_for_db = new object();

        Random random = new Random();
        int player_width = 50;
        int canvas_leftmargin = 3;
        int coef_of_deviation = 7;
        int ball_width = 12;
        int indent = 25;

        BrickBreakerEntities _db = new BrickBreakerEntities();
        #endregion

        /// <summary>
        /// constructor, which creates 75 theoretical room models
        /// </summary>
        public ServerNet()
        {
            int n = 75;
            if (rooms.Count() < n)
            {
                for (int i = 0; i < n; i++)
                    rooms.Add(new Room());
            }
        }

        /// <summary>
        /// method, which always listens for commands coming from clients
        /// </summary>
        public void ListenerClient(Object ob)
        {
            TcpClient tcp = ob as TcpClient;
            BinaryReader br = new BinaryReader(tcp.GetStream());
            while (true)
            {
                var command = (Commands)br.ReadByte();
                switch (command)
                {
                    case Commands.Connect:
                        ConnectAndPaint(tcp);
                        break;
                    case Commands.Disconnect:
                        bool close;
                        try
                        {
                            close = br.ReadBoolean();
                        }
                        catch { close = true; }
                        Disconnect(tcp, close);
                        return;
                    case Commands.SelectARoom:
                        SelectRoom(tcp);
                        break;
                    case Commands.Left:
                        MoveLeft(tcp);
                        break;
                    case Commands.Right:
                        MoveRight(tcp);
                        break;
                    case Commands.LoadGame:
                        Room room = rooms.FirstOrDefault(x => x.player1.tcp == tcp || x.player2.tcp == tcp);
                        SendBallCoordinates(room, command, br.ReadDouble());
                        break;
                    case Commands.LoadBricks:
                        SendBricks(tcp);
                        break;
                    case Commands.BallCoordinatesProcesing:
                        BallCoordinatesProcesing(tcp);
                        break;
                    case Commands.CloseGame:
                        CloseGame(tcp);
                        break;
                    case Commands.SendMessage:
                        SendMessage(tcp);
                        break;
                    case Commands.PaintPlayer:
                        PaintPlayer(tcp);
                        break;
                    case Commands.Pause:
                        Pause(tcp);
                        break;
                    case Commands.Exit:
                        Exit(tcp);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// send a command, which pauses both players
        /// </summary>
        /// <param name="tcp"></param>
        private void Pause(TcpClient tcp)
        {
            Room room = rooms.FirstOrDefault(x => x.player1.tcp == tcp || x.player2.tcp == tcp);
            BinaryWriter bw = new BinaryWriter(room.player1.tcp.GetStream());
            bw.Write((byte)Commands.Pause);
            bw = new BinaryWriter(room.player2.tcp.GetStream());
            bw.Write((byte)Commands.Pause);
        }

        /// <summary>
        /// send your color to another player in room
        /// </summary>
        /// <param name="tcp"></param>
        private void PaintPlayer(TcpClient tcp)
        {
            BinaryReader br = new BinaryReader(tcp.GetStream());
            int color_id = br.ReadInt32();
            Room room = rooms.FirstOrDefault(x => x.player1.tcp == tcp || x.player2.tcp == tcp);
            BinaryWriter bw;
            if (tcp == room.player1.tcp)
            {
                bw = new BinaryWriter(room.player2.tcp.GetStream());
                bw.Write((byte)Commands.PaintPlayer);
                bw.Write(1); // player1
                bw.Write(color_id);
                return;
            }
            bw = new BinaryWriter(room.player1.tcp.GetStream());
            bw.Write((byte)Commands.PaintPlayer);
            bw.Write(2); // player2
            bw.Write(color_id);
        }

        /// <summary>
        /// randomly generates a card
        /// and sends it to both players in the room 
        /// </summary>
        /// <param name="tcp"></param>
        private void SendBricks(TcpClient tcp)
        {
            Room room = rooms.FirstOrDefault(x => x.player1.tcp == tcp || x.player2.tcp == tcp);
            if (tcp == room.player2.tcp)
                return;
            List<Brick> bricks;
            string map;
            int map_id = random.Next(1, 7);
            lock (locker_for_db)
            {
                map = _db.Maps.FirstOrDefault(x => x.id == map_id).title;
                bricks = _db.Bricks.Where(x => x.map_id == map_id).ToList();
            }
            BinaryWriter bw = new BinaryWriter(room.player1.tcp.GetStream());
            SendBricksToPlayer(bw, bricks, map);
            bw = new BinaryWriter(room.player2.tcp.GetStream());
            SendBricksToPlayer(bw, bricks, map);
            room.bricks_p1 = new List<Brick>();
            room.bricks_p2 = new List<Brick>();
            room.bricks_p1.AddRange(bricks);
            room.bricks_p2.AddRange(bricks);
        }

        /// <summary>
        /// send list of bricks and map title to player
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="list">list of bricks</param>
        /// <param name="map">map title</param>
        public void SendBricksToPlayer(BinaryWriter bw, List<Brick> list, string map)
        {
            bw.Write((byte)Commands.LoadBricks);
            bw.Write(map);
            bw.Write(list.Count);
            foreach (var item in list)
            {
                bw.Write((int)item.y);
                bw.Write((int)item.x);
            }
        }

        /// <summary>
        /// connect to the server and paint rooms
        /// </summary>
        /// <param name="tcp"></param>
        private void ConnectAndPaint(TcpClient tcp)
        {
            Console.WriteLine("Hello, user!");
            clients.Add(tcp);
            BinaryWriter bw = new BinaryWriter(tcp.GetStream());
            foreach (var room in rooms)
            {
                int index = rooms.IndexOf(room);
                if (room.player1.busy == true && room.player2.busy == true)
                {
                    bw.Write((byte)Commands.Paint);
                    bw.Write((byte)index);
                    bw.Write(2);
                }
                else if (room.player1.busy == true || room.player2.busy == true)
                {
                    bw.Write((byte)Commands.Paint);
                    bw.Write((byte)index);
                    bw.Write(1);
                }
            }
        }

        /// <summary>
        /// disconnect from server
        /// </summary>
        /// <param name="tcp"></param>
        /// <param name="close"></param>
        private void Disconnect(TcpClient tcp, bool close)
        {
            try
            {
                Console.WriteLine("Bye, user");
                Room room = rooms.FirstOrDefault(x => x.player1.tcp == tcp || x.player2.tcp == tcp);
                BinaryWriter bw;
                if (room != null)
                {
                    int index = rooms.IndexOf(room);
                    foreach (var client in clients)
                    {
                        if (client != tcp)
                        {
                            bw = new BinaryWriter(client.GetStream());
                            bw.Write((byte)Commands.Paint);
                            bw.Write((byte)index);
                            bw.Write(0);
                        }
                    }
                }
                if (close)
                {
                    if (room.player1.tcp == tcp)
                        bw = new BinaryWriter(room.player2.tcp.GetStream());
                    else
                        bw = new BinaryWriter(room.player1.tcp.GetStream());

                    bw.Write((byte)Commands.Disconnect);

                    room.player1 = new Player();
                    room.player2 = new Player();
                }

                clients.Remove(tcp);
                tcp.Close();
            }
            catch { }
        }

        /// <summary>
        /// send message from one player to other players
        /// </summary>
        /// <param name="tcp"></param>
        private void SendMessage(TcpClient tcp)
        {
            BinaryReader br = new BinaryReader(tcp.GetStream());
            string nickname = br.ReadString();
            string message = br.ReadString();
            BinaryWriter bw;
            foreach (var client in clients)
            {
                bw = new BinaryWriter(client.GetStream());
                bw.Write((byte)Commands.SendMessage);
                bw.Write("[" + nickname + "]: ");
                bw.Write(message);
            }
        }

        /// <summary>
        /// close gamewindow and paint this room in a blue color
        /// </summary>
        /// <param name="tcp"></param>
        private void Exit(TcpClient tcp)
        {
            try
            {
                BinaryWriter bw;
                Room room = rooms.FirstOrDefault(x => x.player1.tcp == tcp || x.player2.tcp == tcp);

                if (room.player1.tcp == tcp)
                    room.player1 = new Player();
                else
                    room.player2 = new Player();

                int index = rooms.IndexOf(room);
                foreach (var client in clients)
                {
                    bw = new BinaryWriter(client.GetStream());
                    bw.Write((byte)Commands.Paint);
                    bw.Write((byte)index);
                    bw.Write(0);
                }
            }
            catch { }
        }

        /// <summary>
        /// close game
        /// </summary>
        /// <param name="tcp"></param>
        private void CloseGame(TcpClient tcp)
        {
            try
            {
                Room room = rooms.FirstOrDefault(x => x.player1.tcp == tcp || x.player2.tcp == tcp);
                BinaryWriter bw;

                //send a signal to the second user to close the window
                if (room.player1.tcp == tcp)
                {
                    bw = new BinaryWriter(room.player2.tcp.GetStream());
                    bw.Write((byte)Commands.CloseGame);
                }
                else
                {
                    bw = new BinaryWriter(room.player1.tcp.GetStream());
                    bw.Write((byte)Commands.CloseGame);
                }
                int index = rooms.IndexOf(room);
                rooms[index] = new Room();

                foreach (var client in clients)
                {
                    bw = new BinaryWriter(client.GetStream());
                    bw.Write((byte)Commands.Paint);
                    bw.Write((byte)index);
                    bw.Write(0);
                }
            }
            catch { }
        }

        /// <summary>
        /// process ball coordinates and returns new one
        /// </summary>
        /// <param name="tcp"></param>
        private void BallCoordinatesProcesing(TcpClient tcp)
        {
            #region --- read data ---
            BinaryReader br = new BinaryReader(tcp.GetStream());
            double window_width = br.ReadDouble();
            double window_height = br.ReadDouble();
            double player_width = br.ReadDouble();
            double player_height = br.ReadDouble();
            #endregion
            lock (locker2)
            {
                Room room = rooms.FirstOrDefault(x => x.player1.tcp == tcp || x.player2.tcp == tcp);

                if (room != null)
                {
                    room.ball_top += room.ball_speed * room.direction;

                    #region --- change direction ---
                    if ((room.ball_top < indent) || (room.ball_top > window_height - (indent + ball_width)))
                    {
                        room.goal.goal = true;
                        if (room.ball_top < window_height / 2 && (room.player1.left - ball_width < room.ball_left && room.player1.left + player_width > room.ball_left))
                        {
                            room.k = (room.ball_left + ball_width / 2 - room.player1.left - player_width / 2) / (player_width / 2) * coef_of_deviation;
                            room.goal.goal = false;
                            room.ChangeDirection();
                        }

                        else if (room.ball_top > window_height / 2 && (room.player2.left < (window_width - room.ball_left) && room.player2.left + player_width + ball_width > (window_width - room.ball_left)))
                        {
                            room.k = (room.ball_left + ball_width / 2 - (window_width - room.player2.left) + player_width / 2) / (player_width / 2) * coef_of_deviation;
                            room.goal.goal = false;
                            room.ChangeDirection();
                        }

                        if (room.ball_top < indent)
                        {
                            if (room.goal.goal == false)
                                room.ball_top = indent;
                            else
                            {
                                room.goal.player2 = true;
                                SendGoal(room, 2);
                            }
                        }
                        else
                        {
                            if (room.goal.goal == false)
                                room.ball_top = window_height - (indent + ball_width);
                            else
                            {
                                room.goal.player1 = true;
                                SendGoal(room, 1);
                            }
                        }
                    }
                    else if (room.bricks_p1 != null)
                    {
                        List<Brick> bricks = new List<Brick>();
                        bool side = false;
                        if (room.ball_top < window_height / 2)
                        {
                            bricks = room.bricks_p1;
                            side = true;
                        }
                        else
                            bricks = room.bricks_p2;

                        foreach (var item in bricks)
                        {
                            bool condition1 = (room.ball_top + ball_width < item.x * 45 + 80) || (room.ball_top > item.x * 45 + 80 + 15);
                            bool condition2 = (room.ball_top > window_height - (item.x * 45 + 80)) || (room.ball_top + ball_width < window_height - (item.x* 45 + 80 + 15));
                            if (condition1 && condition2)
                                continue;
                            if ((item.y * 45 + indent - ball_width < room.ball_left && item.y * 45 + indent + 30 > room.ball_left))
                            {
                                room.k = (room.ball_left + ball_width / 2 - (window_width - (int)item.y * 45 - indent) + 30 / 2) / (30 / 2) * coef_of_deviation;
                                RemoveBrick(room, item, window_height, side);
                                bricks.Remove(item);
                                room.ChangeDirection();
                                break;
                            }
                        }
                    }
                    #endregion

                    if (room.goal.goal == false)
                    {
                        if (room.k > coef_of_deviation)
                            room.k = coef_of_deviation;
                        else if (room.k < -coef_of_deviation)
                            room.k = -coef_of_deviation;
                        room.ball_left += room.k;
                        if (room.ball_left < 0 || room.ball_left > window_width - ball_width)
                        {
                            room.k *= -1;
                            if (room.ball_left < 0)
                                room.ball_left = 0;
                            else
                                room.ball_left = window_width - ball_width;
                        }
                    }
                    else
                        SetInitialCoordinates(room, window_height, player_width);
                    SendBallCoordinates(room, Commands.BallCoordinatesProcesing, window_width);
                }
            }
        }

        /// <summary>
        /// remove brick, when ball hits it
        /// </summary>
        /// <param name="room">room, in which we need to remove the brick</param>
        /// <param name="brick"> brick, which we need to remove</param>
        /// <param name="window_height"> height of the gamewindow</param>
        /// <param name="side">side in which ball hits the ball(player1 side or player2 side)</param>
        public void RemoveBrick(Room room, Brick brick, double window_height, bool side)
        {
            BinaryWriter bw = new BinaryWriter(room.player1.tcp.GetStream());
            bw.Write((byte)Commands.RemoveBrick);
            bw.Write((int)brick.y);
            bw.Write((int)brick.x);
            bw.Write(side);
            bw = new BinaryWriter(room.player2.tcp.GetStream());
            bw.Write((byte)Commands.RemoveBrick);
            bw.Write((int)brick.y);
            bw.Write((int)brick.x);
            bw.Write(!side);
        }

        /// <summary>
        /// send a command to score one point to the player who scored the goal
        /// </summary>
        /// <param name="room">room, in which player scored th goal</param>
        /// <param name="g">player</param>
        private void SendGoal(Room room, byte g)
        {
            BinaryWriter bw = new BinaryWriter(room.player1.tcp.GetStream());
            bw.Write((byte)Commands.Goal);
            bw.Write(g);
            bw = new BinaryWriter(room.player2.tcp.GetStream());
            bw.Write((byte)Commands.Goal);
            bw.Write(g);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room">room, in which we set initial coordinates</param>
        /// <param name="window_height">height of the gamewindow</param>
        /// <param name="player_width">players width</param>
        private void SetInitialCoordinates(Room room, double window_height, double player_width)
        {
            room.player1.left = 150;
            room.player2.left = 150;
            if (room.goal.player1 == true)
            {
                room.ball_top = indent;
                room.ball_left = room.player2.left + player_width / 2 - ball_width / 2;
            }
            else if (room.goal.player2 == true)
            {
                room.ball_top = window_height - (indent + ball_width);
                room.ball_left = room.player1.left + player_width / 2 - ball_width / 2;
            }

            room.ChangeDirection();
            room.k = 0;
        }

        /// <summary>
        /// send new ball coordinates to players
        /// </summary>
        /// <param name="room">room, in which we send new ball coordinates</param>
        /// <param name="com">command</param>
        /// <param name="window_width">width of the gamewindow</param>
        private void SendBallCoordinates(Room room, Commands com, double window_width)
        {
            try
            {
                lock (locker1)
                {
                    BinaryWriter bw = new BinaryWriter(room.player1.tcp.GetStream());
                    bw.Write((byte)com);
                    bw.Write(true); //for player 1 (bottom , left)
                    bw.Write(room.ball_top);
                    bw.Write(room.ball_left);

                    bw = new BinaryWriter(room.player2.tcp.GetStream());
                    bw.Write((byte)com);
                    bw.Write(false); //for player 2 (top , right)
                    bw.Write(room.ball_top);
                    bw.Write(room.ball_left);
                }
            }
            catch { }
        }

        /// <summary>
        /// we reflect that the room is occupied by it's theoretical model
        /// </summary>
        /// <param name="tcp"></param>
        private void SelectRoom(TcpClient tcp)
        {
            BinaryReader br = new BinaryReader(tcp.GetStream());
            byte index = br.ReadByte();
            BinaryWriter bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.SelectARoom);
            Room room = rooms[index];
            if (room.player1.busy && room.player2.busy)
            {
                bw.Write(false); // room isn't free
            }
            else if (!room.player1.busy)
            {
                room.player1.tcp = tcp;
                room.player1.busy = true;
                bw.Write(true); // room is free
                bw.Write(false); // start game
                foreach (var client in clients)
                {
                    bw = new BinaryWriter(client.GetStream());
                    bw.Write((byte)Commands.Paint);
                    bw.Write((byte)index);
                    bw.Write(1);
                }
            }
            else
            {
                room.player2.tcp = tcp;
                room.player2.busy = true;
                bw.Write(true);  // room is free
                bw.Write(true);  // start game
                bw = new BinaryWriter(room.player1.tcp.GetStream());
                bw.Write((byte)Commands.SelectARoom);
                bw.Write(true);
                bw.Write(true);

                foreach (var client in clients)
                {
                    bw = new BinaryWriter(client.GetStream());
                    bw.Write((byte)Commands.Paint);
                    bw.Write((byte)index);
                    bw.Write(2);
                }
            }
        }
        /// <summary>
        /// we reflect that the player move left by it's theoretical model
        /// </summary>
        /// <param name="tcp"></param>
        private void MoveLeft(TcpClient tcp)
        {
            Room room = rooms.FirstOrDefault(x => x.player1.tcp == tcp || x.player2.tcp == tcp);
            BinaryWriter bw = new BinaryWriter(tcp.GetStream());
            if (room.player1.tcp == tcp)
            {
                if (room.player1.left - room.player1.speed > 0 - canvas_leftmargin)
                    room.player1.left -= room.player1.speed;
                bw.Write((byte)Commands.Left);
                bw.Write(true);
                bw.Write(room.player1.left);
                bw = new BinaryWriter(room.player2.tcp.GetStream());
                bw.Write((byte)Commands.Left);
                bw.Write(false);
                bw.Write(room.player1.left);
            }
            else
            {
                if (room.player2.left - room.player2.speed > 0 - canvas_leftmargin)
                    room.player2.left -= room.player2.speed;
                bw.Write((byte)Commands.Left);
                bw.Write(true);
                bw.Write(room.player2.left);
                bw = new BinaryWriter(room.player1.tcp.GetStream());
                bw.Write((byte)Commands.Left);
                bw.Write(false);
                bw.Write(room.player2.left);
            }
        }

        /// <summary>
        /// we reflect that the player move right by it's theoretical model
        /// </summary>
        /// <param name="tcp"></param>
        private void MoveRight(TcpClient tcp)
        {
            Room room = rooms.FirstOrDefault(x => x.player1.tcp == tcp || x.player2.tcp == tcp);
            BinaryWriter bw = new BinaryWriter(tcp.GetStream());
            BinaryReader br = new BinaryReader(tcp.GetStream());

            double window_width = br.ReadDouble() - player_width;
            if (room.player1.tcp == tcp)
            {
                if (room.player1.left + room.player1.speed < window_width + 10)
                    room.player1.left += room.player1.speed;
                bw.Write((byte)Commands.Right);
                bw.Write(true);
                bw.Write(room.player1.left);
                bw = new BinaryWriter(room.player2.tcp.GetStream());
                bw.Write((byte)Commands.Right);
                bw.Write(false);
                bw.Write(room.player1.left);
            }
            else
            {
                if (room.player2.left + room.player2.speed < window_width + 10)
                    room.player2.left += room.player2.speed;
                bw.Write((byte)Commands.Right);
                bw.Write(true);
                bw.Write(room.player2.left);
                bw = new BinaryWriter(room.player1.tcp.GetStream());
                bw.Write((byte)Commands.Right);
                bw.Write(false);
                bw.Write(room.player2.left);
            }
        }

    }
}
