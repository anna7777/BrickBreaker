using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Contract;
using System.IO;
using System.Windows.Controls;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using BrickBreaker.ViewModels;
using BrickBreaker.Experts;

namespace BrickBreaker
{
    /// <summary>
    /// class that sends requests to the server and, receiving a response, processes it
    /// </summary>
    public class ClientNet : IContract
    {
        #region variables
        List<Brush> colors = new List<Brush> { Brushes.Blue, Brushes.Yellow, Brushes.Red };
        MenuWindowViewModel menuwindowmodel;
        GameWindowViewModel brickbreakermodel;
        GameWindow brickbreaker;
        Image esc = new Image();
        bool pause = false;
        int room_index;
        TcpClient tcp;
        Thread thread;
        #endregion

        public ClientNet(MenuWindowViewModel mw)
        {
            tcp = new TcpClient();
            brickbreaker = new GameWindow();
            brickbreakermodel = brickbreaker.DataContext as GameWindowViewModel;
            menuwindowmodel = mw;
            SetPauseParams();
        }

        public void SetPauseParams()
        {
            esc.Source = DesignExpert.GetPause();
            Canvas.SetTop(esc, 0);
            Canvas.SetLeft(esc, 0);
            esc.Width = 350;
            esc.Height = 518;
        }

        /// <summary>
        /// connect to server
        /// </summary>
        public void Connect()
        {
            tcp.Connect(new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 11000));
            thread = new Thread(ClientListener);
            thread.IsBackground = true;
            thread.Start();
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.Connect);
        }

        /// <summary>
        /// select a room with the specified index
        /// </summary>
        /// <param name="index">room index in list of rooms</param>
        public void SelectARoom(int index)
        {
            room_index = index;
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.SelectARoom);
            bw.Write((byte)index);
        }

        /// <summary>
        /// player move left
        /// </summary>
        public void MoveLeft()
        {
            if (pause)
                return;
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.Left);
        }

        /// <summary>
        /// player move right
        /// </summary>
        public void MoveRight()
        {
            if (pause)
                return;
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.Right);
            bw.Write(DesignExpert.fieldWidth);
        }

        /// <summary>
        /// send the command to the server, which opens the game window for both players
        /// </summary>
        public void LoadGame()
        {
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.LoadGame);
            bw.Write(DesignExpert.fieldWidth);
        }
        /// <summary>
        /// send the command to the server, which will generate a map 
        /// and return the coordinates of the bricks on it, which it read from the database
        /// </summary>
        public void LoadBricks()
        {
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.LoadBricks);
        }

        /// <summary>
        /// send the command to the server, which will close the game
        /// </summary>
        public void CloseGame()
        {
            brickbreaker.Dispatcher.Invoke(() =>
            {
                menuwindowmodel.ChangeVisibilityWhenExit(menuwindowmodel.rooms[room_index].Content as Button);
            });
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.CloseGame);
        }

        /// <summary>
        /// send the command to the server, which will process ball coordinates 
        /// and return new ball coordinates 
        /// </summary>
        public void BallCoordinatesProcesing()
        {
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.BallCoordinatesProcesing);
            bw.Write(DesignExpert.fieldWidth);
            bw.Write(DesignExpert.fieldHeight);
            bw.Write(DesignExpert.playerWidth);
            bw.Write(DesignExpert.playerHeight);
        }

        /// <summary>
        /// send the command to the server, which will close the game
        /// </summary>
        public void Exit()
        {
            if (brickbreaker.IsLoaded)
            {
                brickbreaker.Dispatcher.Invoke(() =>
                {
                    brickbreaker.Close();
                });
            }
            else
            {
                var bw = new BinaryWriter(tcp.GetStream());
                bw.Write((byte)Commands.Exit);
            }
        }

        /// <summary>
        /// send the command to the server, which will send message to all players
        /// </summary>
        /// <param name="nickname">your nickname</param>
        /// <param name="message">your message</param>
        public void SendMessage(string nickname, string message)
        {
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.SendMessage);
            bw.Write(nickname);
            bw.Write(message);
        }

        /// <summary>
        /// send your color to another player in room
        /// </summary>
        /// <param name="color_id"></param>
        public void PaintPlayer(int color_id)
        {
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.PaintPlayer);
            bw.Write(color_id);
        }

        /// <summary>
        /// pauses both players
        /// </summary>
        public void Pause()
        {
            var bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.Pause);
        }

        /// <summary>
        /// method, which always listens for commands coming from the server
        /// </summary>
        private void ClientListener()
        {
            try
            {
                while (true)
                {
                    var br = new BinaryReader(tcp.GetStream());
                    var command = (Commands)br.ReadByte();
                    switch (command)
                    {
                        case Commands.LoadGame:
                            SetBallCoordinates(command);
                            break;
                        case Commands.LoadBricks:
                            SetBricks();
                            break;
                        case Commands.SelectARoom:
                            SelectRoom();
                            break;
                        case Commands.Left:
                            Move();
                            break;
                        case Commands.Right:
                            Move();
                            break;
                        case Commands.BallCoordinatesProcesing:
                            SetBallCoordinates(command);
                            break;
                        case Commands.CloseGame:
                            brickbreaker.Dispatcher.Invoke(() =>
                            {
                                menuwindowmodel.ChangeVisibilityWhenExit(menuwindowmodel.rooms[room_index].Content as Button);
                                brickbreakermodel.closeSecondGame = false;
                                brickbreaker.Close();
                            });
                            break;
                        case Commands.SendMessage:
                            ReceiveMessage();
                            break;
                        case Commands.Paint:
                            brickbreaker.Dispatcher.Invoke(() =>
                            {
                                menuwindowmodel.rooms[br.ReadByte()].Background = colors[br.ReadByte()];
                            });
                            break;
                        case Commands.Goal:
                            SetGoal();
                            break;
                        case Commands.RemoveBrick:
                            RemoveBrick();
                            break;
                        case Commands.PaintPlayer:
                            SetPlayerColor();
                            break;
                        case Commands.Pause:
                            SetPause();
                            break;
                        case Commands.Disconnect:
                            brickbreaker.Dispatcher.Invoke(() =>
                              {
                                  menuwindowmodel.ChangeVisibilityWhenExit(menuwindowmodel.rooms[room_index].Content as Button);
                                  brickbreakermodel.closeSecondGame = false;
                                  brickbreaker.Close();
                              });
                            break;
                        default:
                            break;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// execute a command, which pauses player
        /// </summary>
        private void SetPause()
        {
            if (!pause)
            {
                brickbreaker.Dispatcher.Invoke(() =>
                {
                    brickbreakermodel.items.Add(esc);
                    brickbreakermodel.timer.Stop();
                });
                pause = true;
                return;
            }
            brickbreaker.Dispatcher.Invoke(() =>
            {
                brickbreakermodel.items.Remove(esc);
                brickbreakermodel.timer.Start();
            });
            pause = false;
        }

        /// <summary>
        /// set the color of the other player
        /// </summary>
        private void SetPlayerColor()
        {
            BinaryReader br = new BinaryReader(tcp.GetStream());
            int player_id = br.ReadInt32();
            int color_id = br.ReadInt32();
            brickbreaker.Dispatcher.Invoke(() =>
            {
                if (player_id == 1)
                {
                    brickbreakermodel.Source2 = DesignExpert.GetPlayerBackgroundByNumber(color_id);
                    brickbreakermodel.Background1 = DesignExpert.colors[color_id - 1];
                    return;
                }
                brickbreakermodel.Source2 = DesignExpert.GetPlayerBackgroundByNumber(color_id);
                brickbreakermodel.Background2 = DesignExpert.colors[color_id - 1];
            });
        }

        /// <summary>
        /// receive message from another player
        /// </summary>
        private void ReceiveMessage()
        {
            BinaryReader br = new BinaryReader(tcp.GetStream());
            brickbreaker.Dispatcher.Invoke(() =>
            {
                var message = new Message();
                (message.DataContext as MessageViewModel).SetMessage(500, br.ReadString(), br.ReadString());
                menuwindowmodel.ChatContent.Add(message);
                if (menuwindowmodel.ChatContent.Count > 30)
                    menuwindowmodel.ChatContent.RemoveAt(0);

                menuwindowmodel.Scroll(message);
            });
        }

        /// <summary>
        /// remove brick, when ball hit it
        /// </summary>
        private void RemoveBrick()
        {
            BinaryReader br = new BinaryReader(tcp.GetStream());
            int y = br.ReadInt32();
            int x = br.ReadInt32();
            bool side = br.ReadBoolean();
            brickbreaker.Dispatcher.Invoke(() =>
            {
                Button btn = new Button();
                if (side)
                    btn = (Button)brickbreakermodel.items.FirstOrDefault(b => Canvas.GetLeft(b) == y * 45 + 25 && Canvas.GetBottom(b) == x * 45 + 80);
                else
                    btn = (Button)brickbreakermodel.items.FirstOrDefault(b => Canvas.GetLeft(b) == y * 45 + 25 && Canvas.GetTop(b) == x * 45 + 80);
                brickbreakermodel.items.Remove(btn);
            });
        }

        /// <summary>
        /// load all bricks at the beginning of the game
        /// </summary>
        private void SetBricks()
        {
            BinaryReader br = new BinaryReader(tcp.GetStream());
            string map = br.ReadString();
            brickbreakermodel.Map = map;
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int y = br.ReadInt32();
                int x = br.ReadInt32();
                brickbreaker.Dispatcher.Invoke(() =>
                {
                    Button temp = new Button() { Width = 30, Height = 15 };
                    temp.Background = DesignExpert.GetBrick();
                    Canvas.SetLeft(temp, y * 45 + 25);
                    Canvas.SetTop(temp, x * 45 + 80);
                    brickbreakermodel.items.Add(temp);

                    temp = new Button() { Width = 30, Height = 15 };
                    temp.Background = DesignExpert.GetBrick();
                    Canvas.SetLeft(temp, y * 45 + 25);
                    Canvas.SetBottom(temp, x * 45 + 80);
                    brickbreakermodel.items.Add(temp);
                });
            }
        }

        /// <summary>
        /// scores one point to the player who scored the goal
        /// </summary>
        private void SetGoal()
        {
            BinaryReader br = new BinaryReader(tcp.GetStream());
            if (br.ReadByte() == 1)
            {
                brickbreaker.Dispatcher.Invoke(() =>
                {
                    brickbreakermodel.Player1Points++;
                    brickbreakermodel.Left1 = 150;
                    brickbreakermodel.Right2 = 150;
                });
            }
            else
            {
                brickbreaker.Dispatcher.Invoke(() =>
                {
                    brickbreakermodel.Player2Points++;
                    brickbreakermodel.Left1 = 150;
                    brickbreakermodel.Right2 = 150;
                });
            }
        }
        /// <summary>
        /// confirms the selection of the room by the player
        /// if there are 2 players in the room, opens the playing field
        /// </summary>
        private void SelectRoom()
        {
            var br = new BinaryReader(tcp.GetStream());
            bool freeroom = br.ReadBoolean();
            bool temp = br.ReadBoolean();
            if (freeroom)
            {
                if (temp)
                {
                    brickbreaker.Dispatcher.Invoke(() =>
                    {
                        brickbreaker = new GameWindow();
                        brickbreakermodel = brickbreaker.DataContext as GameWindowViewModel;
                        brickbreakermodel.con = this;
                        brickbreaker.Show();
                    });
                }
            }
            else
                MessageBox.Show("This room is busy.", "System information", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        /// <summary>
        /// sets new ball coordinates
        /// </summary>
        /// <param name="com"> LoadGame or BallCoordinatesProcessing</param>
        private void SetBallCoordinates(Commands com)
        {
            var br = new BinaryReader(tcp.GetStream());
            bool player1 = br.ReadBoolean();
            if (player1)
            {
                brickbreaker.Dispatcher.Invoke(() =>
                {
                    brickbreakermodel.BallBottom = br.ReadDouble();
                    brickbreakermodel.BallLeft = br.ReadDouble();
                    if (com == Commands.LoadGame)
                    {
                        int number = 1;
                        brickbreakermodel.Source1 = DesignExpert.GetPlayerBackground(ref number);
                        brickbreakermodel.Background1 = DesignExpert.colors[number - 1];
                        PaintPlayer(number);
                    }
                });
            }
            else
            {
                brickbreaker.Dispatcher.Invoke(() =>
                {
                    brickbreakermodel.BallBottom = DesignExpert.fieldHeight - br.ReadDouble() - DesignExpert.ball;
                    brickbreakermodel.BallLeft = DesignExpert.fieldWidth - br.ReadDouble() - DesignExpert.ball;
                    if (com == Commands.LoadGame)
                    {
                        int number = 3;
                        brickbreakermodel.Source1 = DesignExpert.GetPlayerBackground(ref number);
                        brickbreakermodel.Background2 = DesignExpert.colors[number - 1];
                        PaintPlayer(number);
                    }
                });
            }
        }

        /// <summary>
        /// move player 
        /// </summary>
        private void Move()
        {
            var br = new BinaryReader(tcp.GetStream());
            bool res = br.ReadBoolean();
            if (res)
                brickbreaker.Dispatcher.Invoke(() =>
                {
                    brickbreakermodel.Left1 = br.ReadDouble();
                });
            else
                brickbreaker.Dispatcher.Invoke(() =>
                {
                    brickbreakermodel.Right2 = br.ReadDouble();
                });
        }

        /// <summary>
        /// disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            BinaryWriter bw = new BinaryWriter(tcp.GetStream());
            bw.Write((byte)Commands.Disconnect);

            if (brickbreaker.IsLoaded)
                bw.Write(true);
            else
                bw.Write(false);

            tcp.Close();
        }
    }
}
