using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cycubeat.Controls
{
    public partial class Ctrl_Beat : UserControl
    {
        private System.Windows.Forms.Timer beatTimer = new System.Windows.Forms.Timer() { Interval = 2000 };

        private System.Windows.Forms.Timer perfectBeatTimer = new System.Windows.Forms.Timer() { Interval = 100 };

        private int perfectTimes = 0;

        private bool isPerfect = false;

        private bool isClicked = false;

        public event TouchDelegate TouchEvent;

        public event ExitDelegate ExitEvent;

        public void EnterStory(double beginTime)
        {
            StoryHandler.Begin(this, "Enter", beginTime, () =>
            {
                IsHitTestVisible = true;
                beatTimer.Enabled = true;
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

        public void ExitStory()
        {
            IsHitTestVisible = false;
            if (ExitEvent != null)
                StoryHandler.Begin(this, "Exit", () => ExitEvent());
            else
                StoryHandler.Begin(this, "Exit");
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
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Tbx_Beat.Opacity = 0;
                Tbx_Beat.RenderTransform = new ScaleTransform(0, 0);
                Grid_Main.Opacity = 0;
                Eps_Effect.Opacity = 0;
                Eps_Effect.RenderTransform = new ScaleTransform(0, 0);
                Eps_Effect.Fill = new SolidColorBrush(Colors.Red);
                beatTimer.Tick += beatTimerTimer_Tick;
                perfectBeatTimer.Tick += perfectBeatTimer_Tick;
            }
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
                StoryHandler.Begin(this, "Beat");
            }
            else
            {
                Tbx_Beat.Text = "Perfect";
                StoryHandler.Begin(this, "PerfectBeat");
            }
        }

        private void touch()
        {
            Btn_Beat.IsHitTestVisible = false;
            DoubleAnimation start = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(.3),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
                AutoReverse = true
            };
            DoubleAnimation scale = new DoubleAnimation()
            {
                From = 0.8,
                To = 1,
                Duration = TimeSpan.FromSeconds(.3),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
                AutoReverse = true
            };
            ColorAnimation perfectColor = new ColorAnimation()
            {
                From = Colors.Red,
                To = Colors.Orange,
                Duration = TimeSpan.FromSeconds(.3),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
                AutoReverse = true
            };
            ColorAnimation normalColor = new ColorAnimation()
            {
                From = Colors.Red,
                To = Colors.Blue,
                Duration = TimeSpan.FromSeconds(.3),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
                AutoReverse = true
            };
            Eps_Effect.Opacity = 0;
            Eps_Effect.RenderTransformOrigin = new Point(.5, .5);
            Eps_Effect.RenderTransform = new ScaleTransform();
            Eps_Effect.BeginAnimation(OpacityProperty, start);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);
            if (isPerfect)
            {
                Eps_Effect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, perfectColor);
                Tbx_Beat.Text = "Perfect";
            }
            else
            {
                Eps_Effect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, normalColor);
                Tbx_Beat.Text = "";
            }
            Tbx_Beat.Opacity = 0;
            Tbx_Beat.RenderTransformOrigin = new Point(.5, .5);
            Tbx_Beat.RenderTransform = new ScaleTransform();
            Tbx_Beat.BeginAnimation(OpacityProperty, start);
            Tbx_Beat.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, start);
            Tbx_Beat.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, start);
        }
    }
}