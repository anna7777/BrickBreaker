using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace BrickBreaker
{
    public class GameWindowViewModel : ViewModelBase
    {
        public static double fieldWidth = 350;
        public static double fieldHeight = 518;
        public static double playerWidth = 50;
        public static double playerHeight = 15;
        public static double ball = 12;


        public IContract con;
        public DispatcherTimer timer;
        public bool closeSecondGame;
        public int p1Points = 0;
        public int p2Points = 0;

        public ICommand MoveLeftCommand { get; set; }
        public ICommand MoveRightCommand { get; set; }
        public ICommand EscCommand { get; set; }
        public ICommand WindowLoadedCommand { get; set; }
        public ICommand ClosingCommand { get; set; }

        private Brush background1;
        public Brush Background1
        {
            get { return background1; }
            set
            {
                background1 = value;
                OnPropertyChanged(nameof(Background1));
            }
        }

        private Brush background2;
        public Brush Background2
        {
            get { return background2; }
            set
            {
                background2 = value;
                OnPropertyChanged(nameof(Background2));
            }
        }

        private double top2;
        public double Top2
        {
            get { return top2; }
            set
            {
                top2 = value;
                OnPropertyChanged(nameof(Top2));
            }
        }

        private double right2;
        public double Right2
        {
            get { return right2; }
            set
            {
                right2 = value;
                OnPropertyChanged(nameof(Right2));
            }
        }

        private double bottom1;
        public double Bottom1
        {
            get { return bottom1; }
            set
            {
                bottom1 = value;
                OnPropertyChanged(nameof(Bottom1));
            }
        }

        private double left1;
        public double Left1
        {
            get { return left1; }
            set
            {
                left1 = value;
                OnPropertyChanged(nameof(Left1));
            }
        }

        private double ball_left;
        public double BallLeft
        {
            get { return ball_left; }
            set
            {
                ball_left = value;
                OnPropertyChanged(nameof(BallLeft));
            }
        }

        private double ball_bottom;
        public double BallBottom
        {
            get { return ball_bottom; }
            set
            {
                ball_bottom = value;
                OnPropertyChanged(nameof(BallBottom));
            }
        }

        private double ball_right;
        public double BallRight
        {
            get { return ball_right; }
            set
            {
                ball_right = value;
                OnPropertyChanged(nameof(BallRight));
            }
        }

        private double ball_top;
        public double BallTop
        {
            get { return ball_top; }
            set
            {
                ball_top = value;
                OnPropertyChanged(nameof(BallTop));
            }
        }

        private int player1Points;
        public int Player1Points
        {
            get { return player1Points; }
            set
            {
                player1Points = value;
                OnPropertyChanged(nameof(Player1Points));
            }
        }

        private int player2Points;
        public int Player2Points
        {
            get { return player2Points; }
            set
            {
                player2Points = value;
                OnPropertyChanged(nameof(Player2Points));
            }
        }

        public GameWindowViewModel()
        {
            MoveLeftCommand = new RelayCommand(MoveLeftCommandExecute);
            MoveRightCommand = new RelayCommand(MoveRightCommandExecute);
            EscCommand = new RelayCommand(EscCommandExecute);
            WindowLoadedCommand = new RelayCommand(WindowLoadedCommandExecute);
            ClosingCommand = new RelayCommand(ClosingCommandExecute);

            closeSecondGame = true;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timer.Tick += Timer_Tick;

            Top2 = 10;
            Bottom1 = 10;
            Right2 = 150;
            Left1 = 150;
        }


        private void MoveLeftCommandExecute(object obj)
        {
            con.MoveLeft();
        }

        private void MoveRightCommandExecute(object obj)
        {
            con.MoveRight();
        }

        private void EscCommandExecute(object obj)
        {

        }

        private void WindowLoadedCommandExecute(object obj)
        {
            con.LoadGame();
            timer.Start();
        }

        private void ClosingCommandExecute(object obj)
        {
            timer.Stop();
            if (closeSecondGame)
                con.CloseGame();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            con.BallCoordinatesProcesing();
        }
    }
}
