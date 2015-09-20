using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Cycubeat.Controls;

namespace Cycubeat.Pages
{
    public partial class Page_Play : UserControl, ISwitchable
    {
        public event TouchDelegate TouchEvent;

        public event BeatDelegate BeatEvent;

        public Page_Play()
        {
            InitializeComponent();
        }

        public void EnterStory()
        {
            StoryHandler.Begin(this, "Enter", () => idleCountdownTimer.Enabled = true);
            initDifficulty();
        }

        public void ExitStory(Action callback)
        {
            IsHitTestVisible = false;
            StoryHandler.Begin(this, "Exit", () => callback());
        }

        public void InitializeProperty()
        {
            Grid_Left.Opacity = 0;
            Grid_Right.Opacity = 0;
            Cnv_Main.Children.Clear();
            Grid_Right.Children.Remove(Btn_Extra);
            Tbx_Timer.Text = (idleCountdownTimes + 1).ToString();
            idleCountdownTimer.Tick += IdleCountdownTimer_Tick;
        }

        private int stage = 0;

        private int idleCountdownTimes = 29;

        private void IdleCountdownTimer_Tick(object sender, EventArgs e)
        {
            if (idleCountdownTimes > 0)
            {
                Tbx_Timer.Text = idleCountdownTimes.ToString();
                idleCountdownTimes--;
            }
            else
            {
                Tbx_Timer.Text = "";
                idleCountdownTimer.Enabled = false;
                if (stage == 0)
                    Switcher.Switch(new Page_Start());
                else
                {
                    BeatEvent();
                    StoryHandler.Stop(this, "Shine");
                }
            }
        }

        private Point[] beaterMaps =
        {
            new Point(343, 44), new Point(593, 44), new Point(843, 44),
            new Point(343, 294), new Point(593, 294), new Point(843, 294),
            new Point(343, 544), new Point(593, 544), new Point(843, 544)
        };

        private System.Windows.Forms.Timer idleCountdownTimer = new System.Windows.Forms.Timer() { Interval = 1000 };

        private void loaded(object sender, RoutedEventArgs e)
        {
            InitializeProperty();
            EnterStory();
        }

        private void initDifficulty()
        {
            var Btn_Start = new Ctrl_Touch()
            {
                Text = "Start",
                Foreground = Brushes.White
            };
            Btn_Start.TouchEvent += () =>
            {
                StoryHandler.Begin(this, "Shine");
                stage++;
                TouchEvent();
                Btn_Start.ExitStory();
                Grid_Right.Children.Remove(Btn_Start);
                idleCountdownTimes = 10;
                StoryHandler.Begin(this, "StartEnter", () =>
                {
                    Img_Difficulty.Width = Img_Score.Width = 270;
                    Img_Difficulty.Margin = Img_Score.Margin = new Thickness(0);
                    Img_Difficulty.HorizontalAlignment = Img_Score.HorizontalAlignment = HorizontalAlignment.Center;
                    Img_Difficulty.VerticalAlignment = Img_Score.VerticalAlignment = VerticalAlignment.Center;
                    initBeater();
                    StoryHandler.Begin(this, "StartExit");
                });
            };
            Btn_Start.EnterStory(1.15, () => IsHitTestVisible = true);
            Grid.SetRow(Btn_Start, 1);
            Grid_Right.Children.Add(Btn_Start);

            Ctrl_Touch[] touchers =
            {
                new Ctrl_Touch()
                {
                    Text = "Easy",
                    Foreground = Brushes.GreenYellow
                },
                new Ctrl_Touch()
                {
                    Text = "Normal",
                    Foreground = Brushes.Gold
                },
                new Ctrl_Touch()
                {
                    Text = "Hard",
                    Foreground = Brushes.Tomato
                }
            };
            foreach (var x in touchers.Select((v, i) => new { i = i, v = v }))
            {
                x.v.GroupName = "Difficulty";
                Canvas.SetLeft(x.v, 343 + x.i * 250);
                Canvas.SetTop(x.v, 294);
                x.v.TouchEvent += () =>
                {
                    Tbx_Difficulty.Text = x.v.Text;
                    Tbx_Difficulty.Foreground = x.v.Foreground;
                };
                x.v.ExitEvent += () => Cnv_Main.Children.Remove(x.v);
                x.v.EnterStory(x.i * 0.05 + 1);
                Cnv_Main.Children.Add(x.v);
            }
            TouchEvent += () => Array.ForEach(touchers, x => x.ExitStory());
        }

        private void initBeater()
        {
            var Btn_End = new Ctrl_Touch()
            {
                Text = "Exit",
                Foreground = Brushes.White
            };
            Btn_End.TouchEvent += () =>
            {
                BeatEvent();
                Btn_End.ExitStory();
                Grid_Right.Children.Remove(Btn_End);
                idleCountdownTimes = 60;
            };
            Btn_End.EnterStory(0.45, () => IsHitTestVisible = true);
            Grid.SetRow(Btn_End, 1);
            Grid_Right.Children.Add(Btn_End);

            Ctrl_Beat[] beaters = new Ctrl_Beat[9];
            for (int i = 0; i < beaters.Length; i++)
            {
                beaters[i] = new Ctrl_Beat();
                Canvas.SetLeft(beaters[i], beaterMaps[i].X);
                Canvas.SetTop(beaters[i], beaterMaps[i].Y);
                beaters[i].TouchEvent += () => { };
                beaters[i].ExitEvent += () => { };
                beaters[i].EnterStory(i * 0.05);
                Cnv_Main.Children.Add(beaters[i]);
            }
            BeatEvent += () => Array.ForEach(beaters, x => x.ExitStory());
        }
    }
}