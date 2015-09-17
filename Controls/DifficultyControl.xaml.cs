using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cycubeat.Controls
{
    public partial class DifficultyControl : UserControl
    {
        public event DifficultyEventHandler DifficultyEvent;

        public event DifficultyExitDelegate ExitEvent;

        public TimeSpan BeginTime;

        public int Difficulty;

        private string[] fillColors = { "#FF86FF86", "#FFFFFF00", "#FFFF5D5D" };

        private enum difficultyList { Easy, Normal, Hard };

        public DifficultyControl(int Difficulty, TimeSpan BeginTime)
        {
            InitializeComponent();
            this.Difficulty = Difficulty;
            this.BeginTime = BeginTime;
            initOriginProperty();
            initStory();
        }

        private void initOriginProperty()
        {
            Opacity = 0;
            Tbx_Touch.Opacity = 0;
            var size = SystemParameters.PrimaryScreenWidth / 7.68;
            Width = Btn_Toucher.Width = Eps_Effect.Width = Height = Btn_Toucher.Height = Eps_Effect.Height = size;
            Tbx_Touch.Text = ((difficultyList)Difficulty).ToString();
            Eps_Effect.Opacity = 0;
            Eps_Effect.RenderTransformOrigin = new Point(.5, .5);
            Eps_Effect.RenderTransform = new ScaleTransform(0, 0);
            Eps_Effect.Fill = (SolidColorBrush)(new BrushConverter().ConvertFromString(fillColors[Difficulty]));
            Btn_Toucher.IsHitTestVisible = false;
        }

        private void initStory()
        {
            DoubleAnimation start = new DoubleAnimation()
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
                BeginTime = BeginTime
            };
            BeginAnimation(OpacityProperty, start);
            Tbx_Touch.BeginAnimation(OpacityProperty, start);
            start.Completed += (s, e) => Btn_Toucher.IsHitTestVisible = true;
            Eps_Effect.BeginAnimation(OpacityProperty, start);

            DoubleAnimation scale = new DoubleAnimation()
            {
                To = 0.8,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
                BeginTime = BeginTime
            };
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);
        }

        private void touch()
        {
            Btn_Toucher.IsHitTestVisible = false;
            DoubleAnimation scale = new DoubleAnimation()
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(.3),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);
        }

        public void Prep()
        {
            Btn_Toucher.IsHitTestVisible = true;
            DoubleAnimation scale = new DoubleAnimation()
            {
                To = 0.8,
                Duration = TimeSpan.FromSeconds(.3),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);
        }

        public void Exit()
        {
            Btn_Toucher.IsHitTestVisible = false;
            DoubleAnimation start = new DoubleAnimation()
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            BeginAnimation(OpacityProperty, start);
            Tbx_Touch.BeginAnimation(OpacityProperty, start);
            //start.Completed += (s, e) => ExitEvent();
            Eps_Effect.BeginAnimation(OpacityProperty, start);

            DoubleAnimation scale = new DoubleAnimation()
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
            Eps_Effect.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);
        }

        private void Btn_Toucher_Click(object sender, RoutedEventArgs e)
        {
            touch();
            DifficultyEvent(Difficulty);
        }
    }
}
