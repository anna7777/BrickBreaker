using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BrickBreaker.Experts
{
    class DesignExpert
    {
        public static string project_path = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.Length - 9);
        public static double fieldWidth = 350;
        public static double fieldHeight = 518;
        public static double playerWidth = 50;
        public static double playerHeight = 15;
        public static double ball = 12;
        public static List<Brush> colors = new List<Brush>() { Brushes.Red, Brushes.SkyBlue, Brushes.LightGreen, Brushes.Orange };
        static Random random = new Random();

        public static BitmapImage GetPlayerBackground(ref int number)
        {
            number = random.Next(number, 2+number);
            return new BitmapImage(new Uri(project_path + $"pictures/players/player{number}.jpg"));
        }

        public static BitmapImage GetPlayerBackgroundByNumber(int number)
        {
            return new BitmapImage(new Uri(project_path + $"pictures/players/player{number}.jpg"));
        }

        public static ImageBrush GetBrick()
        {
            return new ImageBrush() { ImageSource = new BitmapImage(new Uri(project_path + $"pictures/brick.jpg")) };
        }
    }
}
