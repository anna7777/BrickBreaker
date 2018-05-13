using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BrickBreaker.Experts
{
    /// <summary>
    /// DesignExpert is a class, which contains different design parameters and background images
    /// </summary>
    class DesignExpert
    {
        #region variables
        public static string project_path = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 9);
        public static List<Brush> colors = new List<Brush>() { Brushes.Red, Brushes.SkyBlue, Brushes.LightGreen, Brushes.Orange };
        public static double fieldWidth = 350;
        public static double fieldHeight = 518;
        public static double playerWidth = 50;
        public static double playerHeight = 15;
        public static double ball = 12;
        static ImageBrush brick = new ImageBrush() { ImageSource = new BitmapImage(new Uri(project_path + $"pictures/brick.jpg")) };
        static BitmapImage esc = new BitmapImage(new Uri(project_path + $"pictures/esc.png"));
        static Random random = new Random();
        #endregion

        /// <summary>
        /// generate backgroundimage for player by his id
        /// </summary>
        /// <param name="number">player_id</param>
        /// <returns></returns>
        public static BitmapImage GetPlayerBackground(ref int number)
        {
            number = random.Next(number, 2+number);
            return new BitmapImage(new Uri(project_path + $"pictures/players/player{number}.jpg"));
        }

        /// <summary>
        /// return a backgroundimage by it's id
        /// </summary>
        /// <param name="number">color_id</param>
        /// <returns></returns>
        public static BitmapImage GetPlayerBackgroundByNumber(int number)
        {
            return new BitmapImage(new Uri(project_path + $"pictures/players/player{number}.jpg"));
        }

        /// <summary>
        /// return an image, called "brick", which is used for bricks background
        /// </summary>
        /// <returns></returns>
        public static ImageBrush GetBrick()
        {
            return brick;
        }

        /// <summary>
        /// return an image, called "esc", which is used for pause background
        /// </summary>
        /// <returns></returns>
        public static BitmapImage GetPause()
        {
            return esc;
        }
    }
}
