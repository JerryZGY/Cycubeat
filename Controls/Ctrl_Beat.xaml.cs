using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cycubeat.Controls
{
    public partial class Ctrl_Beat : UserControl, ITouchable
    {
        private System.Windows.Forms.Timer beatTimer = new System.Windows.Forms.Timer() { Interval = 2000 };

        private System.Windows.Forms.Timer perfectBeatTimer = new System.Windows.Forms.Timer() { Interval = 100 };

        private int perfectTimes = 0;

        private bool isPerfect = false;

        private bool isClicked = false;

        public event BeatDelegate BeatEvent;

        public event PopDelegate PopEvent;

        public void EnterStory()
        {
            EnterStory(0);
        }

        public void EnterStory(double beginTime)
        {
            StoryHandler.Begin(this, "Enter", beginTime, () =>
            {
                IsHitTestVisible = true;
                //beatTimer.Enabled = true;
            });
        }

        public void EnterStory(double beginTime, Action callback)
        {
            StoryHandler.Begin(this, "Enter", beginTime, () =>
            {
                IsHitTestVisible = true;
                callback();
            });
        }

        public void ExitStory(Action callback)
        {
            IsHitTestVisible = false;
            StoryHandler.Begin(this, "Exit", callback);
        }

        public void RemoveSelf()
        {
            ((Canvas)Parent).Children.Remove(this);
        }

        public void StartBeat()
        {
            StartBeat(() => { });
        }

        public void StartBeat(Action callback)
        {
            IsHitTestVisible = true;
            Btn_Beat.IsHitTestVisible = true;
            Img_Beat.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Touch.png", UriKind.Relative));
            perfectTimes = 0;
            isPerfect = false;
            isClicked = false;
            perfectBeatTimer.Enabled = true;
            StoryHandler.Begin(this, "Peek", () =>
            {
                if (!isClicked)
                {
                    StoryHandler.Begin(this, "Pop", () =>
                    {
                        perfectBeatTimer.Enabled = false;
                        Btn_Beat.IsHitTestVisible = false;
                        IsHitTestVisible = false;
                        callback();
                        PopEvent();
                    });
                }
            });
        }

        public Ctrl_Beat()
        {
            InitializeComponent();
            DataContext = this;
            Img_Beat.Opacity = 0;
            Img_Beat.RenderTransform = new ScaleTransform(0, 0);
            Grid_Main.Opacity = 0;
            Eps_Effect.Opacity = 0;
            Eps_Effect.RenderTransform = new ScaleTransform(0, 0);
            Eps_Effect.Fill = new SolidColorBrush(Colors.Red);
            beatTimer.Tick += beatTimerTimer_Tick;
            perfectBeatTimer.Tick += perfectBeatTimer_Tick;
            Btn_Beat.IsHitTestVisible = false;
        }

        private void beatTimerTimer_Tick(object sender, EventArgs e)
        {
            StartBeat();
        }

        private void perfectBeatTimer_Tick(object sender, EventArgs e)
        {
            isPerfect = (perfectTimes >= 2 && perfectTimes <= 4);
            perfectTimes++;
        }

        private void click(object sender, RoutedEventArgs e)
        {
            if (Btn_Beat.IsHitTestVisible)
            {
                IsHitTestVisible = false;
                Btn_Beat.IsHitTestVisible = false;
                if (!isClicked)
                {
                    perfectBeatTimer.Enabled = false;
                    if (!isPerfect)
                    {
                        Img_Beat.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Miss.png", UriKind.Relative));
                        StoryHandler.Begin(this, "Beat", () => PopEvent());
                    }
                    else
                    {
                        Img_Beat.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Perfect.png", UriKind.Relative));
                        BeatEvent(2000);
                        StoryHandler.Begin(this, "PerfectBeat", () => PopEvent());
                    }
                    isClicked = true;
                }
            }
        }
    }
}