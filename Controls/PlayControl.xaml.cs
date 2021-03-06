﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cycubeat.Controls
{
    public partial class PlayControl : UserControl
    {
        public event NotifyDelegate NotifyEvent;

        private System.Windows.Forms.Timer idleCountdownTimer = new System.Windows.Forms.Timer() { Interval = 1000 };

        private int idleCountdownTimes = 30;

        private int score = 0;

        private int stage = 0;

        private bool isStart = false;

        private double resWidth = SystemParameters.PrimaryScreenWidth;

        private double resHeight = SystemParameters.PrimaryScreenHeight;

        private const double size = 100;

        private const double distance = 50;

        private TouchControl[] touchers = new TouchControl[9];

        private DifficultyControl[] difficulties = new DifficultyControl[3];

        private static string[] fontColorsMap = { "#FF86FF86", "#FFFFFF00", "#FFFF5D5D" };

        public PlayControl()
        {
            InitializeComponent();
        }

        private void PlayControl_Loaded(object sender, RoutedEventArgs e)
        {
            initOriginProperty();
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
            Img_Choose.Width = resWidth / 7;
            Img_Difficulty.Opacity = 0;
            Img_Difficulty.Width = resWidth / 7;
            Img_Highest.Opacity = 0;
            Img_Highest.Width = resWidth / 7;
            Img_Score.Opacity = 0;
            Img_Score.Width = resWidth / 7;
            Tbx_Difficulty.Opacity = 0;
            Tbx_Difficulty.FontSize = resWidth / 22;
            Ctrl_Score.Opacity = 0;
            //Tbx_Score.FontSize = resWidth / 22;
            Img_BackgroundShine.RenderTransformOrigin = new Point(.5, .5);
            Img_BackgroundShine.RenderTransform = new ScaleTransform();
            Img_UserView.Opacity = 0;
            Btn_Extra.Opacity = 0;
            Tbx_Timer.Opacity = 0;
            Tbx_Timer.FontSize = resWidth / 18;
            Tbx_Timer.Text = idleCountdownTimes.ToString();
            idleCountdownTimer.Tick += idleCountdownTimer_Tick;
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
                    //initRect();
                    initDifficulty();
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
                    Ctrl_Score.BeginAnimation(OpacityProperty, innerFadeIn);
                };
                Grid_Right.RenderTransform.BeginAnimation(TranslateTransform.XProperty, translate);
            };
            BeginAnimation(OpacityProperty, fadeIn);
        }

        private void initDifficulty()
        {
            var intervalX = resWidth / 19.2;
            var intervalY = resHeight / 10.8;
            for (int i = 0; i < difficulties.Length; i++)
            {
                var x = i % 3;
                difficulties[i] = new DifficultyControl(i, TimeSpan.FromSeconds(i * 0.3));
                difficulties[i].DifficultyEvent += new DifficultyEventHandler(selectDifficulty);
                //difficulties[i].ExitEvent += new DifficultyExitDelegate(initRect);
                switch (x)
                {
                    case 0:
                        Canvas.SetLeft(difficulties[i], (resWidth - difficulties[i].Width) / 2 - (difficulties[i].Width + intervalX));
                        break;
                    case 1:
                        Canvas.SetLeft(difficulties[i], (resWidth - difficulties[i].Width) / 2);
                        break;
                    case 2:
                        Canvas.SetLeft(difficulties[i], (resWidth - difficulties[i].Width) / 2 + (difficulties[i].Width + intervalX));
                        break;
                }
                
                Canvas.SetTop(difficulties[i], (resHeight - difficulties[i].Height) / 2);
                Cnv_Main.Children.Add(difficulties[i]);
            }
        }

        private void initRect()
        {
            for (int i = 0; i < touchers.Length; i++)
            {
                touchers[i] = new TouchControl(TimeSpan.FromSeconds(i * 0.1));
                touchers[i].ScoreEvent += new ScoreEventHandler(updateScore);
                var x = i % 3;
                var intervalX = resWidth / 19.2;
                var intervalY = resHeight / 10.8;
                switch (x)
                {
                    case 0:
                        Canvas.SetLeft(touchers[i], (resWidth - touchers[i].Width) / 2 - (touchers[i].Width + intervalX));
                        break;
                    case 1:
                        Canvas.SetLeft(touchers[i], (resWidth - touchers[i].Width) / 2);

                        break;
                    case 2:
                        Canvas.SetLeft(touchers[i], (resWidth - touchers[i].Width) / 2 + (touchers[i].Width + intervalX));
                        break;
                }
                if (i < 3)
                {
                    Canvas.SetTop(touchers[i], (resHeight - touchers[i].Height) / 2 - (touchers[i].Height + intervalY));
                }
                else if (i < 6)
                {
                    Canvas.SetTop(touchers[i], (resHeight - touchers[i].Height) / 2);
                }
                else
                {
                    Canvas.SetTop(touchers[i], (resHeight - touchers[i].Height) / 2 + (touchers[i].Height + intervalY));
                }
                Cnv_Main.Children.Add(touchers[i]);
            }
            idleCountdownTimes = 60;
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

        private void updateScore(int score)
        {
            this.score += score;
            Ctrl_Score.UpdateScore(string.Format("{0:0000000}", this.score));
        }

        private void selectDifficulty(int difficulty)
        {
            Tbx_Difficulty.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFromString(fontColorsMap[difficulty]));
            switch (difficulty)
            {
                case 0:
                    Tbx_Difficulty.Text = "Easy";
                    difficulties[1].Prep();
                    difficulties[2].Prep();
                    break;
                case 1:
                    Tbx_Difficulty.Text = "Normal";
                    difficulties[0].Prep();
                    difficulties[2].Prep();
                    break;
                case 2:
                    Tbx_Difficulty.Text = "Hard";
                    difficulties[0].Prep();
                    difficulties[1].Prep();
                    break;
            }
        }

        private void stage0Story()
        {
            DoubleAnimation fadeOut = new DoubleAnimation()
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(.2),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            Btn_Extra.BeginAnimation(OpacityProperty, fadeOut);
            Img_Choose.BeginAnimation(OpacityProperty, fadeOut);
            Img_Difficulty.BeginAnimation(OpacityProperty, fadeOut);
            Img_Highest.BeginAnimation(OpacityProperty, fadeOut);
            fadeOut.Completed += (s, e) =>
            {
                Img_Difficulty.Width = Img_Score.Width  = resWidth / 6;
                Img_Difficulty.HorizontalAlignment = HorizontalAlignment.Center;
                Img_Difficulty.VerticalAlignment = VerticalAlignment.Center;
                Img_Score.HorizontalAlignment = HorizontalAlignment.Center;
                Img_Score.VerticalAlignment = VerticalAlignment.Center;
                DoubleAnimation fadeIn = new DoubleAnimation()
                {
                    To = 1,
                    Duration = TimeSpan.FromSeconds(.2),
                    EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
                };
                Img_Difficulty.BeginAnimation(OpacityProperty, fadeIn);
                fadeIn.Completed += (se, ev) => initRect();
                Img_Score.BeginAnimation(OpacityProperty, fadeIn);
            };
            Img_Score.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void clearCnv()
        {
            Cnv_Main.Children.Clear();
        }

        private void Btn_Extra_Click(object sender, RoutedEventArgs e)
        {
            Btn_Extra.IsHitTestVisible = false;
            if (stage == 0)
            {
                stage++;
                foreach (var ctrl in difficulties)
                {
                    ctrl.Exit();
                    ctrl.IsHitTestVisible = false;
                }
                difficulties = null;
                stage0Story();
            }
        }
    }
}
