using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Cycubeat.Controls
{
    public partial class PlayControl : UserControl
    {
        public event NotifyDelegate NotifyEvent;

        private System.Windows.Forms.Timer idleCountdownTimer = new System.Windows.Forms.Timer() { Interval = 1000 };

        private int idleCountdownTimes = 30;

        private bool isStart = false;

        private double resWidth = SystemParameters.PrimaryScreenWidth;

        private double resHeight = SystemParameters.PrimaryScreenHeight;

        private const double size = 100;

        private const double distance = 50;

        private Rectangle[] rectangles = new Rectangle[9];

        private static string[] fontColorsMap = { "#FF86FF86", "#FFFFFF00", "#FFFF5D5D" };

        public PlayControl()
        {
            InitializeComponent();
        }

        private void PlayControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            initOriginProperty();
            initRect();
            initEnterStory();
        }

        private void initOriginProperty()
        {
            Opacity = 0;
            Cnv_Main.Width = resWidth;
            Cnv_Main.Height = resHeight;
            Cnv_Main.RenderTransformOrigin = new Point(.5, .5);
            Cnv_Main.RenderTransform = new RotateTransform();
            Img_Background.RenderTransformOrigin = new Point(.5, .5);
            Img_Background.RenderTransform = new ScaleTransform(.01, .01);
            Grid_Left.RenderTransform = new TranslateTransform(-450, 0);
            Grid_Right.RenderTransform = new TranslateTransform(450, 0);
            Img_Choose.Opacity = 0;
            Img_Difficulty.Opacity = 0;
            Img_Highest.Opacity = 0;
            Img_Score.Opacity = 0;
            Tbx_Difficulty.Opacity = 0;
            Tbx_Score.Opacity = 0;
            Img_BackgroundShine.RenderTransformOrigin = new Point(.5, .5);
            Img_BackgroundShine.RenderTransform = new ScaleTransform();
            Img_UserView.Opacity = 0;
            Btn_Extra.Opacity = 0;
            Tbx_Timer.Opacity = 0;
            Tbx_Timer.Text = idleCountdownTimes.ToString();
            idleCountdownTimer.Tick += idleCountdownTimer_Tick;
        }

        private void initRect()
        {
            //for (int i = 0; i < rectangles.Length; i++)
            //{
            //    rectangles[i] = new Rectangle();
            //    rectangles[i].Opacity = 0;
            //    rectangles[i].Fill = (SolidColorBrush)(new BrushConverter().ConvertFromString(colorsMap[i]));
            //    rectangles[i].Width = rectangles[i].Height = size * scaleMap[i];
            //    setRectPosition(rectangles[i], offsetMap[i].X, offsetMap[i].Y);
            //    cnv_Title.Children.Add(rectangles[i]);
            //}
        }

        private void initEnterStory()
        {
            DoubleAnimation scale = new DoubleAnimation()
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            Img_Background.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
            Img_Background.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);

            DoubleAnimation fadeIn = new DoubleAnimation()
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            fadeIn.Completed += (s, e) =>
            {
                DoubleAnimation translate = new DoubleAnimation()
                {
                    To = 0,
                    Duration = TimeSpan.FromSeconds(1),
                    EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
                };
                Grid_Left.RenderTransform.BeginAnimation(TranslateTransform.XProperty, translate);
                translate.Completed += (se, ev) =>
                {
                    initBackgroundStory();
                    DoubleAnimation innerFadeIn = new DoubleAnimation()
                    {
                        To = 1,
                        Duration = TimeSpan.FromSeconds(.4),
                        EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
                    };

                    Img_Choose.BeginAnimation(OpacityProperty, innerFadeIn);
                    Img_UserView.BeginAnimation(OpacityProperty, innerFadeIn);
                    innerFadeIn.BeginTime = TimeSpan.FromSeconds(.15);
                    Img_Difficulty.BeginAnimation(OpacityProperty, innerFadeIn);
                    innerFadeIn.BeginTime = TimeSpan.FromSeconds(.3);
                    Tbx_Difficulty.BeginAnimation(OpacityProperty, innerFadeIn);
                    innerFadeIn.BeginTime = TimeSpan.FromSeconds(.45);
                    Btn_Extra.BeginAnimation(OpacityProperty, innerFadeIn);
                    Img_Highest.BeginAnimation(OpacityProperty, innerFadeIn);
                    innerFadeIn.BeginTime = TimeSpan.FromSeconds(.6);
                    Img_Score.BeginAnimation(OpacityProperty, innerFadeIn);
                    innerFadeIn.BeginTime = TimeSpan.FromSeconds(.75);
                    Tbx_Timer.BeginAnimation(OpacityProperty, innerFadeIn);
                    innerFadeIn.Completed += InnerFadeIn_Completed;
                    Tbx_Score.BeginAnimation(OpacityProperty, innerFadeIn);
                };
                Grid_Right.RenderTransform.BeginAnimation(TranslateTransform.XProperty, translate);
            };
            BeginAnimation(OpacityProperty, fadeIn);
        }

        private void initBackgroundStory()
        {
            DoubleAnimation fadeInEffect = new DoubleAnimation()
            {
                From = 0,
                To = 0.5,
                Duration = TimeSpan.FromSeconds(.15),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseInOut },
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            Img_BackgroundShine.BeginAnimation(OpacityProperty, fadeInEffect);

            DoubleAnimation scaleEffect = new DoubleAnimation()
            {
                From = 1,
                To = 1.02,
                Duration = TimeSpan.FromSeconds(.15),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseInOut },
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            Img_BackgroundShine.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleEffect);
            Img_BackgroundShine.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleEffect);
        }

        private void InnerFadeIn_Completed(object sender, EventArgs e)
        {
            
            idleCountdownTimer.Start();
        }

        private void idleCountdownTimer_Tick(object sender, EventArgs e)
        {
            if (idleCountdownTimes > 0)
            {
                Tbx_Timer.Text = idleCountdownTimes.ToString();
                idleCountdownTimes--;
            }
            else
            {
                Tbx_Timer.Text = "";
                idleCountdownTimer.Stop();
                Exit();
            }
        }

        public void Exit()
        {
            IsHitTestVisible = false;
            Img_BackgroundShine.Visibility = Visibility.Collapsed;

            DoubleAnimation fadeOutFore = new DoubleAnimation()
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            Grid_Foreground.BeginAnimation(OpacityProperty, fadeOutFore);

            DoubleAnimation scale = new DoubleAnimation()
            {
                To = 10,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseIn }
            };
            Img_Background.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
            Img_Background.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);

            DoubleAnimation fadeOut = new DoubleAnimation()
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseIn }
            };
            fadeOut.Completed += (s, e) => NotifyEvent();
            BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
