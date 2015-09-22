using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Cycubeat.Controls;

namespace Cycubeat.Pages
{
    public partial class Page_Play : UserControl, ISwitchable
    {
        public Page_Play()
        {
            InitializeComponent();
        }

        public void EnterStory()
        {
            StoryHandler.Begin(this, "Enter", () => countDownTimer.Enabled = true);
            initDifficulty();
        }

        public void ExitStory(Action callback)
        {
            IsHitTestVisible = false;
            StoryHandler.Begin(this, "Exit", () => callback());
        }

        public void InitializeProperty()
        {
            isPeeking = false;
            countdownTimes = 14;
            score = 0;
            stageID = 0;
            touchers = new Ctrl_Difficulty[3];
            beaters = new Ctrl_Beat[9];
            numbers = new Ctrl_Numpad[10];
            countDownTimer = new System.Windows.Forms.Timer() { Interval = 1000 };
            Grid_Left.Opacity = 0;
            Grid_Right.Opacity = 0;
            Cnv_Main.Children.Clear();
            Grid_Right.Children.Remove(Btn_Extra);
            Tbx_Timer.Text = (countdownTimes + 1).ToString();
            countDownTimer.Tick += countDownTimer_Tick;
        }

        private bool isPeeking;

        private int score;

        private int stageID;

        private enum Difficulty { Easy = 0, Normal, Hard };

        private Ctrl_Difficulty btn_Next;

        private SolidColorBrush[] colorsMap = { Brushes.GreenYellow, Brushes.Gold, Brushes.Tomato };

        private Ctrl_Difficulty[] touchers;

        private Ctrl_Beat[] beaters;

        private Ctrl_Numpad[] numbers;

        private int countdownTimes = 14;

        private System.Windows.Forms.Timer countDownTimer;

        private void countDownTimer_Tick(object sender, EventArgs e)
        {
            if (countDownTimer != null)
            {
                if (countdownTimes > 0)
                {
                    Tbx_Timer.Text = countdownTimes.ToString();
                    countdownTimes--;
                }
                else
                {
                    Tbx_Timer.Text = "";
                    countDownTimer.Enabled = false;
                    countDownOver();
                }
            }
        }

        private Point[] controlsMap =
        {
            new Point(343, 44), new Point(593, 44), new Point(843, 44),
            new Point(343, 294), new Point(593, 294), new Point(843, 294),
            new Point(343, 544), new Point(593, 544), new Point(843, 544)
        };


        private void loaded(object sender, RoutedEventArgs e)
        {
            InitializeProperty();
            EnterStory();
        }

        private void countDownOver()
        {
            switch (stageID)
            {
                case 0://Difficulty
                    countDownTimer = null;
                    Switcher.Switch(new Page_Start());
                    break;
                case 1://Play
                    Ctrl_Music.Stop();
                    exitStory(beaters);
                    if (score >= 1000)
                        initPassResult();
                    else
                        initResult();
                    stageID++;
                    break;
                case 2://Result
                    if (score >= 1000)
                    {
                        btn_Next.ExitStory(() => Grid_Right.Children.Remove(btn_Next));
                        exitStage();
                    }
                    else
                    {
                        countDownTimer = null;
                        Switcher.Switch(new Page_Start());
                    }
                    break;
                case 3://Numpad
                case 4://Lottery
                    countDownTimer = null;
                    Switcher.Switch(new Page_Start());
                    break;
            }
        }

        private void exitStage()
        {
            countDownTimer.Enabled = false;
            switch (stageID)
            {
                case 0:
                    StoryHandler.Begin(this, "Shine");
                    StoryHandler.Begin(this, "StartEnter", () => initBeater());
                    break;
                case 2:
                    StoryHandler.Begin(this, "ExitResult", () => initNumpad());
                    break;
                case 3:
                    initLottery();
                    break;
            }
            stageID++;
        }

        private void initDifficulty()
        {
            for (int i = 0; i < touchers.Length; i++)
            {
                var text = ((Difficulty)i).ToString();
                var foreground = colorsMap[i];
                touchers[i] = new Ctrl_Difficulty(text, foreground);
                touchers[i].GroupName = "Difficulty";
                touchers[i].TouchEvent += () => { Tbx_Difficulty.Text = text; Tbx_Difficulty.Foreground = foreground; };
                touchers[i].EnterStory(i * 0.05 + 1);
                Canvas.SetLeft(touchers[i], 343 + i * 250);
                Canvas.SetTop(touchers[i], 294);
                Cnv_Main.Children.Add(touchers[i]);
            }

            var btn_Start = new Ctrl_Difficulty("Start", Brushes.White);
            btn_Start.TouchEvent += () =>
            {
                btn_Start.ExitStory(() => Grid_Right.Children.Remove(btn_Start));
                exitStory(touchers);
                exitStage();
            };
            btn_Start.EnterStory(1.15, () => IsHitTestVisible = true);
            Grid.SetRow(btn_Start, 1);
            Grid_Right.Children.Add(btn_Start);
        }

        private void initBeater()
        {
            Tbx_Timer.Text = "85";
            countdownTimes = 84;
            countDownTimer.Enabled = true;
            Img_Difficulty.Width = Img_Score.Width = 270;
            Img_Difficulty.Margin = Img_Score.Margin = new Thickness(0);
            Img_Difficulty.HorizontalAlignment = Img_Score.HorizontalAlignment = HorizontalAlignment.Center;
            Img_Difficulty.VerticalAlignment = Img_Score.VerticalAlignment = VerticalAlignment.Center;

            for (int i = 0; i < beaters.Length; i++)
            {
                beaters[i] = new Ctrl_Beat();
                Canvas.SetLeft(beaters[i], controlsMap[i].X);
                Canvas.SetTop(beaters[i], controlsMap[i].Y);
                beaters[i].BeatEvent += (score) => updateScore(score);
                beaters[i].PopEvent += () => isPeeking = false;
                beaters[i].EnterStory(i * 0.05);
                Cnv_Main.Children.Add(beaters[i]);
            }

            Ctrl_Music.PeekEvent += (i) =>
            {
                if (!isPeeking)
                {
                    isPeeking = true;
                    beaters[i].StartBeat(() =>
                    {
                        isPeeking = false;
                    });
                }
            };

            //Ctrl_Music.PeekEvent += (i) => beaters[i].StartBeat();
            Ctrl_Music.Play();
            StoryHandler.Begin(this, "StartExit");
        }

        private void initResult()
        {
            Tbx_Timer.Text = "15";
            countdownTimes = 14;
            countDownTimer.Enabled = true;
            Tbx_Subtitle.Text = "挑戰失敗";
            Tbx_Cotent.FontSize = 60;
            Tbx_Cotent.Text = "請再接再厲";
            StoryHandler.Stop(this, "Shine");
            StoryHandler.Begin(this, "EnterResult");
            var btn_next = new Ctrl_Difficulty("Next", Brushes.White);
            btn_next.EnterStory();
            btn_next.TouchEvent += () =>
            {
                countDownTimer = null;
                Switcher.Switch(new Page_Start());
            };
            Grid.SetRow(btn_next, 1);
            Grid_Right.Children.Add(btn_next);
        }

        private void initPassResult()
        {
            Tbx_Timer.Text = "15";
            countdownTimes = 14;
            countDownTimer.Enabled = true;
            StoryHandler.Stop(this, "Shine");
            StoryHandler.Begin(this, "EnterResult");
            btn_Next = new Ctrl_Difficulty("Next", Brushes.White);
            btn_Next.EnterStory();
            btn_Next.TouchEvent += () =>
            {
                btn_Next.ExitStory(() => Grid_Right.Children.Remove(btn_Next));
                exitStage();
            };
            Grid.SetRow(btn_Next, 1);
            Grid_Right.Children.Add(btn_Next);
        }

        private void initNumpad()
        {
            Tbx_Timer.Text = "40";
            countdownTimes = 39;
            countDownTimer.Enabled = true;
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = new Ctrl_Numpad(i.ToString());
                numbers[i].NumEvent += (num) => enterNumber(num);
                numbers[i].EnterStory(i * 0.05);
                if (i == 0)
                {
                    Grid.SetRow(numbers[i], 1);
                    Grid_Right.Children.Add(numbers[i]);
                }
                else
                {
                    Canvas.SetLeft(numbers[i], controlsMap[i - 1].X);
                    Canvas.SetTop(numbers[i], controlsMap[i - 1].Y);
                    Cnv_Main.Children.Add(numbers[i]);
                }
            }
            var btn_Return = new Ctrl_Function("Return", Brushes.GreenYellow);
            btn_Return.FuncEvent += () =>
            {
                if (Tbx_StudentID.Text.Length > 0)
                    Tbx_StudentID.Text = Tbx_StudentID.Text.Remove(Tbx_StudentID.Text.Length - 1);
            };
            btn_Return.EnterStory(0);
            Grid.SetRow(btn_Return, 0);
            Grid_Right.Children.Add(btn_Return);

            var btn_Enter = new Ctrl_Function("Enter", Brushes.Gold);
            btn_Enter.FuncEvent += () =>
            {
                btn_Return.ExitStory(() => Grid_Right.Children.Remove(btn_Return));
                btn_Enter.ExitStory(() => Grid_Right.Children.Remove(btn_Enter));
                exitStory(numbers);
                exitStage();
            };
            btn_Enter.EnterStory(0);
            Grid.SetRow(btn_Enter, 2);
            Grid_Right.Children.Add(btn_Enter);
        }

        private void initLottery()
        {
            Tbx_Timer.Text = "15";
            countdownTimes = 14;
            countDownTimer.Enabled = true;
            StoryHandler.Begin(this, "EnterLottery");
            var btn_next = new Ctrl_Difficulty("Next", Brushes.White);
            btn_next.EnterStory();
            btn_next.TouchEvent += () =>
            {
                btn_next.ExitStory(() =>
                {
                    countDownTimer = null;
                    Grid_Right.Children.Remove(btn_next); 
                    Switcher.Switch(new Page_Start());
                });
            };
            Grid.SetRow(btn_next, 1);
            Grid_Right.Children.Add(btn_next);
        }

        private void exitStory(ITouchable[] controls)
        {
            Array.ForEach(controls, x => x.ExitStory(() => x.RemoveSelf()));
        }

        private void updateScore(int score)
        {
            this.score += score;
            Ctrl_Score.UpdateScore(string.Format("{0:0000000}", this.score));
        }

        private void enterNumber(int num)
        {
            if (Tbx_StudentID.Text.Length < 8)
                Tbx_StudentID.Text += num.ToString();
        }
    }
}