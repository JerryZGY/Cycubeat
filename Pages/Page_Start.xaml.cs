using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Cycubeat.Pages
{
    public partial class Page_Start : UserControl, ISwitchable
    {
        public Page_Start()
        {
            InitializeComponent();
        }

        public void EnterStory()
        {
            StoryHandler.Begin(this, "Enter", () =>
            {
                StoryHandler.Begin(this, "Fade_Title_Shine");
                Btn_Start.IsHitTestVisible = true;
            });
            StoryHandler.BeginLoop(this, new string[] { "Loop_Fade_Bg", "Loop_Scale_Bg" });
        }

        public void ExitStory(Action callback)
        {
            StoryHandler.Begin(this, "Exit", () => callback());
        }

        public void InitializeProperty()
        {
            Img_Title.Opacity = 0;
            Tbx_Copyright.Opacity = 0;
            initRect();
        }

        private const double size = 100;

        private const double distance = 50;

        private static double[] scaleMap = { 1, .8, .8, .8, .8, .64, .64, .64, .64 };

        private static string[] colorsMap =
        {
            "#FF696969",
            "#FFA7A7A7","#FFA7A7A7","#FFA7A7A7","#FFA7A7A7",
            "#FFFFFFFF", "#FFFFFFFF", "#FFFFFFFF", "#FFFFFFFF"
        };

        private static Point[] offsetMap =
        {
            new Point(0, 0), //Center
            new Point(-(size * scaleMap[1] + distance), 0), //Left
            new Point(size * scaleMap[2] + distance, 0), //Right
            new Point(0, -(size * scaleMap[3] + distance)), //Top
            new Point(0, size * scaleMap[4] + distance), //Down
            new Point(-(size * scaleMap[5] + distance), -(size * scaleMap[5] + distance)), //TopLeft
            new Point(size * scaleMap[6] + distance, -(size * scaleMap[6] + distance)), //TopRight
            new Point(-(size * scaleMap[7] + distance), size * scaleMap[7] + distance), //DownLeft
            new Point(size * scaleMap[8] + distance, size * scaleMap[8] + distance), //DownRight
        };

        private void Page_Start_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeProperty();
            EnterStory();
        }

        private void initRect()
        {
            var rects = new Rectangle[9];
            for (int i = 0; i < rects.Length; i++)
            {
                rects[i] = new Rectangle();
                rects[i].Opacity = 0;
                rects[i].Fill = (SolidColorBrush)(new BrushConverter().ConvertFromString(colorsMap[i]));
                rects[i].Width = rects[i].Height = size * scaleMap[i];
                Canvas.SetLeft(rects[i], (1366 - rects[i].Width) / 2 + offsetMap[i].X);
                Canvas.SetTop(rects[i], (768 - rects[i].Height) / 2 + offsetMap[i].Y);
                Cnv_Title.Children.Add(rects[i]);
            }
            StoryHandler.SetChildren(this, "Enter", rects);
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            Btn_Start.IsHitTestVisible = false;
            Switcher.Switch(new Page_Play());
        }
    }
}