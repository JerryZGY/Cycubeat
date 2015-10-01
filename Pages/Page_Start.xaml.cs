using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            Img_Logo.Opacity = 0;
            Img_Title.Opacity = 0;
            Tbx_Copyright.Opacity = 0;
            initRect();
            Switcher.pageSwitcher.kinect.TrackingBody = null;
            Switcher.pageSwitcher.TouchEvent += touchEvent;
            Switcher.pageSwitcher.Bounds = Switcher.pageSwitcher.Map.StartMap;
            refreshTimer.Tick += (s, e) => Switcher.pageSwitcher.kinect.TrackingBody = null;
            refreshTimer.Enabled = true;

        }

        private void touchEvent(KinectInputArgs e)
        {
            var leave = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseLeaveEvent };
            var enter = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseEnterEvent };
            TouchMapHandler.CheckTouch(e.Posotion, (e.InputState == InputState.Open),
                () => { Btn_Start.RaiseEvent(leave); },
                () => { Btn_Start.RaiseEvent(enter); },
                () =>
                {
                    if (Btn_Start.IsHitTestVisible)
                        Btn_Start.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            );
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

        private System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer() { Interval = 5000 };

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
            refreshTimer.Enabled = false;
            refreshTimer.Stop();
            Switcher.Switch(new Page_Play());
        }

        private void Music_MediaOpened(object sender, RoutedEventArgs e)
        {
            Music.Play();
        }

        private void Music_MediaEnded(object sender, RoutedEventArgs e)
        {
            Music.Position = TimeSpan.Zero;
            Music.Play();
        }
    }
}