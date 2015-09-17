using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cycubeat.Controls
{
    public partial class TouchControl : UserControl
    {

        public event ScoreEventHandler ScoreEvent;

        private System.Windows.Forms.Timer touchTimer = new System.Windows.Forms.Timer() { Interval = 2000 };

        private System.Windows.Forms.Timer perfectTouchTimer = new System.Windows.Forms.Timer() { Interval = 100 };

        private int perfectTimes = 0;

        private bool isPerfect = false;

        private TimeSpan beginTimes = TimeSpan.FromSeconds(0);

        public TouchControl(TimeSpan beginTimes)
        {
            InitializeComponent();
            initOriginProperty();
            this.beginTimes = beginTimes;
            initStory();
        }

        public void initOriginProperty()
        {
            Opacity = 0;
            Tbx_Touch.Opacity = 0;
            var size = SystemParameters.PrimaryScreenWidth / 7.68;
            Width = Btn_Toucher.Width = Eps_Effect.Width = Height = Btn_Toucher.Height = Eps_Effect.Height = size;
            Btn_Toucher.IsHitTestVisible = false;
            touchTimer.Tick += touchTimer_Tick;
            perfectTouchTimer.Tick += perfectTouchTimer_Tick;
        }

        private void initStory()
        {
            DoubleAnimation start = new DoubleAnimation()
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(.5),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
                BeginTime = beginTimes
            };
            BeginAnimation(OpacityProperty, start);
            start.AutoReverse = true;
            Eps_Effect.Opacity = 0;
            Eps_Effect.RenderTransformOrigin = new Point(.5, .5);
            Eps_Effect.RenderTransform = new ScaleTransform(0, 0);
            Eps_Effect.BeginAnimation(OpacityProperty, start);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, start);
            start.Completed += (s, e) => touchTimer.Start();
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, start);
        }

        private void startTouch()
        {
            Btn_Toucher.IsHitTestVisible = true;
            perfectTimes = 0;
            isPerfect = false;
            perfectTouchTimer.Start();
            perfectTouchTimer.Enabled = true;
            DoubleAnimation start = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(.5),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
                AutoReverse = true
            };
            Eps_Effect.Opacity = 0;
            Eps_Effect.RenderTransformOrigin = new Point(.5, .5);
            Eps_Effect.RenderTransform = new ScaleTransform();
            Eps_Effect.BeginAnimation(OpacityProperty, start);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, start);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, start);
            Tbx_Touch.Opacity = 0;
            Tbx_Touch.Text = "Touch";
            Tbx_Touch.RenderTransformOrigin = new Point(.5, .5);
            Tbx_Touch.RenderTransform = new ScaleTransform();
            Tbx_Touch.BeginAnimation(OpacityProperty, start);
            Tbx_Touch.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, start);
            start.Completed += (s, e) =>
            {
                perfectTouchTimer.Stop();
                Btn_Toucher.IsHitTestVisible = false;
            };
            Tbx_Touch.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, start);
        }

        private void touch()
        {
            Btn_Toucher.IsHitTestVisible = false;
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
            Eps_Effect.Opacity = 0;
            Eps_Effect.RenderTransformOrigin = new Point(.5, .5);
            Eps_Effect.RenderTransform = new ScaleTransform();
            Eps_Effect.BeginAnimation(OpacityProperty, start);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);
            Tbx_Touch.Opacity = 0;
            Tbx_Touch.Text = (isPerfect) ? "Perfect" : "";
            Tbx_Touch.RenderTransformOrigin = new Point(.5, .5);
            Tbx_Touch.RenderTransform = new ScaleTransform();
            Tbx_Touch.BeginAnimation(OpacityProperty, start);
            Tbx_Touch.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, start);
            Tbx_Touch.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, start);
        }

        private void touchTimer_Tick(object sender, EventArgs e)
        {
            startTouch();
        }

        private void perfectTouchTimer_Tick(object sender, EventArgs e)
        {
            isPerfect = (perfectTimes >= 2 && perfectTimes <= 4);
            perfectTimes++;
            Console.WriteLine(isPerfect);
        }

        private void Btn_Toucher_Click(object sender, RoutedEventArgs e)
        {
            touch();
            ScoreEvent((isPerfect) ? 1000 : 500);
        }
    }
}
