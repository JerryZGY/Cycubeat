using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            Btn_Beat.IsHitTestVisible = true;
            Tbx_Beat.Text = "Touch";
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
                    });
                }
            });
        }

        public Ctrl_Beat()
        {
            InitializeComponent();
            DataContext = this;
            Tbx_Beat.Opacity = 0;
            Tbx_Beat.RenderTransform = new ScaleTransform(0, 0);
            Grid_Main.Opacity = 0;
            Eps_Effect.Opacity = 0;
            Eps_Effect.RenderTransform = new ScaleTransform(0, 0);
            Eps_Effect.Fill = new SolidColorBrush(Colors.Red);
            beatTimer.Tick += beatTimerTimer_Tick;
            perfectBeatTimer.Tick += perfectBeatTimer_Tick;
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
            isClicked = true;
            Btn_Beat.IsHitTestVisible = false;
            perfectBeatTimer.Enabled = false;
            if (!isPerfect)
            {
                Tbx_Beat.Text = "";
                BeatEvent(1000);
                StoryHandler.Begin(this, "Beat");
            }
            else
            {
                Tbx_Beat.Text = "Perfect";
                BeatEvent(2000);
                StoryHandler.Begin(this, "PerfectBeat");
            }
        }
    }
}