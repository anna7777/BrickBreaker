using Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
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

        public ObservableCollection<Button> items { get; set; }

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

        private string map;
        public string Map
        {
            get { return map; }
            set
            {
                map = value;
                OnPropertyChanged(nameof(Map));
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

            items = new ObservableCollection<Button>();

            Top2 = 10;
            Bottom1 = 10;
            Right2 = 150;
            Left1 = 150;

            CreateTwoPlayers();
            CreateBall();
        }

        public void CreateTwoPlayers()
        {
            Button player1 = new Button();
            player1.MinHeight = 15;
            player1.MinWidth = 50;

            #region -- binding background --
            Binding bind_background1 = new Binding();
            BindingSettings(bind_background1);
            bind_background1.Path = new PropertyPath("Background1");
            player1.SetBinding(Button.BackgroundProperty, bind_background1);
            #endregion

            #region -- binding bottom --
            Binding bind_bottom1 = new Binding();
            BindingSettings(bind_bottom1);
            bind_bottom1.Path = new PropertyPath("Bottom1");
            player1.SetBinding(Canvas.BottomProperty, bind_bottom1);
            #endregion

            #region -- binding left --
            Binding bind_left1 = new Binding();
            BindingSettings(bind_left1);
            bind_left1.Path = new PropertyPath("Left1");;
            player1.SetBinding(Canvas.LeftProperty, bind_left1);
            #endregion

            Button player2 = new Button();
            player2.MinHeight = 15;
            player2.MinWidth = 50;

            #region -- binding background --
            Binding bind_background2 = new Binding();
            BindingSettings(bind_background2);
            bind_background2.Path = new PropertyPath("Background2");
            player2.SetBinding(Button.BackgroundProperty, bind_background2);
            #endregion

            #region -- binding top --
            Binding bind_top2 = new Binding();
            BindingSettings(bind_top2);
            bind_top2.Path = new PropertyPath("Top2");
            player2.SetBinding(Canvas.TopProperty, bind_top2);
            #endregion

            #region -- binding right --
            Binding bind_right2 = new Binding();
            BindingSettings(bind_right2);
            bind_right2.Path = new PropertyPath("Right2"); ;
            player2.SetBinding(Canvas.RightProperty, bind_right2);
            #endregion

            items.Add(player1);
            items.Add(player2);
        }

        public void BindingSettings(Binding bind)
        {
            bind.Source = this;
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }

        public void CreateBall()
        {
            Button ball = new Button();
            ball.MinHeight = 12;
            ball.MinWidth = 12;

            #region -- binding bottom --
            Binding bindBottom = new Binding();
            BindingSettings(bindBottom);
            bindBottom.Path = new PropertyPath("BallBottom");
            ball.SetBinding(Canvas.BottomProperty, bindBottom);
            #endregion

            #region -- binding left --
            Binding bindLeft = new Binding();
            BindingSettings(bindLeft);
            bindLeft.Path = new PropertyPath("BallLeft");
            ball.SetBinding(Canvas.LeftProperty, bindLeft);
            #endregion

            ControlTemplate ct = new ControlTemplate(typeof(Button));
            ct.VisualTree = new FrameworkElementFactory(typeof(Ellipse));
            ct.VisualTree.SetValue(Ellipse.FillProperty, Brushes.Red);
            ball.Template = ct;
            items.Add(ball);
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
            con.LoadBricks();
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
