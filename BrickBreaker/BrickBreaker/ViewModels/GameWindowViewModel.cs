using BrickBreaker.Experts;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BrickBreaker.ViewModels
{
    /// <summary>
    /// class that implements the viewmodel of a game
    /// </summary>
    public class GameWindowViewModel : ViewModelBase
    {
        #region variables
        public IContract con;
        public DispatcherTimer timer;
        public bool closeSecondGame;
        public int p1Points = 0;
        public int p2Points = 0;
        #endregion

        #region commands
        public ICommand MoveLeftCommand { get; set; }
        public ICommand MoveRightCommand { get; set; }
        public ICommand EscCommand { get; set; }
        public ICommand WindowLoadedCommand { get; set; }
        public ICommand ClosingCommand { get; set; }
        #endregion

        #region collections
        public ObservableCollection<UIElement> items { get; set; }
        #endregion

        #region binding properties
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

        private BitmapImage source1;
        public BitmapImage Source1
        {
            get { return source1; }
            set
            {
                source1 = value;
                OnPropertyChanged(nameof(Source1));
            }
        }

        private BitmapImage source2;
        public BitmapImage Source2
        {
            get { return source2; }
            set
            {
                source2 = value;
                OnPropertyChanged(nameof(Source2));
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
        #endregion

        /// <summary>
        /// constructor, which initializes all objects with initial values
        /// </summary>
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

            items = new ObservableCollection<UIElement>();

            Top2 = 10;
            Bottom1 = 10;
            Right2 = 150;
            Left1 = 150;

            CreateTwoPlayers();
            CreateBall();
        }

        /// <summary>
        /// create two players
        /// </summary>
        public void CreateTwoPlayers()
        {
            Image player1 = new Image();
            player1.Height = 15;
            player1.Width = 50;

            #region -- binding bottom --
            Binding bind_bottom1 = new Binding();
            BindingSettings(bind_bottom1);
            bind_bottom1.Path = new PropertyPath("Bottom1");
            player1.SetBinding(Canvas.BottomProperty, bind_bottom1);
            #endregion

            #region -- binding left --
            Binding bind_left1 = new Binding();
            BindingSettings(bind_left1);
            bind_left1.Path = new PropertyPath("Left1"); ;
            player1.SetBinding(Canvas.LeftProperty, bind_left1);
            #endregion

            #region -- binding source --
            Binding source1 = new Binding();
            BindingSettings(source1);
            source1.Path = new PropertyPath("Source1"); ;
            player1.SetBinding(Image.SourceProperty, source1);
            #endregion

            Image player2 = new Image();
            player2.Height = 15;
            player2.Width = 50;

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

            #region -- binding source --
            Binding source2 = new Binding();
            BindingSettings(source2);
            source2.Path = new PropertyPath("Source2"); ;
            player2.SetBinding(Image.SourceProperty, source2);
            #endregion

            items.Add(player1);
            items.Add(player2);
        }

        /// <summary>
        /// set binding settings
        /// </summary>
        /// <param name="bind"></param>
        public void BindingSettings(Binding bind)
        {
            bind.Source = this;
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }

        /// <summary>
        /// create a ball
        /// </summary>
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

        /// <summary>
        /// player move left
        /// </summary>
        /// <param name="obj"></param>
        private void MoveLeftCommandExecute(object obj)
        {
            con.MoveLeft();
        }

        /// <summary>
        /// player move right
        /// </summary>
        /// <param name="obj"></param>
        private void MoveRightCommandExecute(object obj)
        {
            con.MoveRight();
        }

        /// <summary>
        /// pause call
        /// </summary>
        /// <param name="obj"></param>
        private void EscCommandExecute(object obj)
        {
            con.Pause();
        }

        /// <summary>
        /// load bricks and game objects, when the gamewindow was loaded
        /// </summary>
        /// <param name="obj"></param>
        private void WindowLoadedCommandExecute(object obj)
        {
            con.LoadBricks();
            con.LoadGame();
            timer.Start();
        }

        /// <summary>
        /// close game
        /// </summary>
        /// <param name="obj"></param>
        private void ClosingCommandExecute(object obj)
        {
            timer.Stop();
            if (closeSecondGame)
                con.CloseGame();
        }

        /// <summary>
        /// action performed at a predetermined time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            con.BallCoordinatesProcesing();
        }
    }
}
