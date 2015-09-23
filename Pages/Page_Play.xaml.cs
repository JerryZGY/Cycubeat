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
            var kinect = new KinectHandler();
            kinect.BodyEvent += (b) => Img_UserView.Source = b;
            kinect.HandEvent += (l, r, iL, iR) =>
            {
                Canvas.SetLeft(PointerFirst, l.X);
                Canvas.SetTop(PointerFirst, l.Y);
                Canvas.SetLeft(PointerSecond, r.X);
                Canvas.SetTop(PointerSecond, r.Y);
                Tbx_L.Text = $"X:{Convert.ToInt32(l.X).ToString()} Y:{Convert.ToInt32(l.Y).ToString()}";
                Tbx_R.Text = $"X:{Convert.ToInt32(r.X).ToString()} Y:{Convert.ToInt32(l.X).ToString()}";
                //new Point(343, 44), new Point(593, 44), new Point(843, 44),
                //new Point(343, 294), new Point(593, 294), new Point(843, 294),
                //new Point(343, 544), new Point(593, 544), new Point(843, 544)
                if (iL && beaters[8] != null)
                {
                    var e = new RoutedEventArgs(Button.ClickEvent);
                    if (l.X >= 343 && l.X <= 523)
                    {
                        if (l.Y >= 44 && l.Y <= 224)
                        {
                            beaters[0].Btn_Beat.RaiseEvent(e);
                        }
                        else if (l.Y >= 294 && l.Y <= 474)
                        {
                            beaters[3].Btn_Beat.RaiseEvent(e);
                        }
                        else if (l.Y >= 544 && l.Y <= 724)
                        {
                            beaters[6].Btn_Beat.RaiseEvent(e);
                        }
                    }
                    else if (l.X >= 593 && l.X <= 773)
                    {
                        if (l.Y >= 44 && l.Y <= 224)
                        {
                            beaters[1].Btn_Beat.RaiseEvent(e);
                        }
                        else if (l.Y >= 294 && l.Y <= 474)
                        {
                            beaters[4].Btn_Beat.RaiseEvent(e);
                        }
                        else if (l.Y >= 544 && l.Y <= 724)
                        {
                            beaters[7].Btn_Beat.RaiseEvent(e);
                        }
                    }
                    else if (l.X >= 843 && l.X <= 1023)
                    {
                        if (l.Y >= 44 && l.Y <= 224)
                        {
                            beaters[2].Btn_Beat.RaiseEvent(e);
                        }
                        else if (l.Y >= 294 && l.Y <= 474)
                        {
                            beaters[5].Btn_Beat.RaiseEvent(e);
                        }
                        else if (l.Y >= 544 && l.Y <= 724)
                        {
                            beaters[8].Btn_Beat.RaiseEvent(e);
                        }
                    }
                }

                if (iR && beaters[8] != null)
                {
                    var e = new RoutedEventArgs(Button.ClickEvent);
                    if (r.X >= 343 && r.X <= 523)
                    {
                        if (r.Y >= 44 && r.Y <= 224)
                        {
                            beaters[0].Btn_Beat.RaiseEvent(e);
                        }
                        else if (r.Y >= 294 && r.Y <= 474)
                        {
                            beaters[3].Btn_Beat.RaiseEvent(e);
                        }
                        else if (r.Y >= 544 && r.Y <= 724)
                        {
                            beaters[6].Btn_Beat.RaiseEvent(e);
                        }
                    }
                    else if (r.X >= 593 && r.X <= 773)
                    {
                        if (r.Y >= 44 && r.Y <= 224)
                        {
                            beaters[1].Btn_Beat.RaiseEvent(e);
                        }
                        else if (r.Y >= 294 && r.Y <= 474)
                        {
                            beaters[4].Btn_Beat.RaiseEvent(e);
                        }
                        else if (r.Y >= 544 && r.Y <= 724)
                        {
                            beaters[7].Btn_Beat.RaiseEvent(e);
                        }
                    }
                    else if (r.X >= 843 && r.X <= 1023)
                    {
                        if (r.Y >= 44 && r.Y <= 224)
                        {
                            beaters[2].Btn_Beat.RaiseEvent(e);
                        }
                        else if (r.Y >= 294 && r.Y <= 474)
                        {
                            beaters[5].Btn_Beat.RaiseEvent(e);
                        }
                        else if (r.Y >= 544 && r.Y <= 724)
                        {
                            beaters[8].Btn_Beat.RaiseEvent(e);
                        }
                    }
                }
            };
            Img_UserView.Source = Switcher.pageSwitcher.bodyIndexBitmap;
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

        private SolidColorBrush[] colorsMap = { Brushes.GreenYellow, Brushes.Gold, Brushes.Tomato };

        public Ctrl_Difficulty btn_Next;

        public Ctrl_Difficulty Btn_Start;

        public Ctrl_Difficulty[] touchers;

        public Ctrl_Beat[] beaters;

        public Ctrl_Numpad[] numbers;

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

            Btn_Start = new Ctrl_Difficulty("Start", Brushes.White);
            Btn_Start.TouchEvent += () =>
            {
                Btn_Start.RdBtn = null;
                Btn_Start.ExitStory(() =>
                {
                    Grid_Right.Children.Remove(Btn_Start);
                    Btn_Start = null;
                });
                
                exitStory(touchers);
                exitStage();
            };
            Btn_Start.EnterStory(1.15, () => IsHitTestVisible = true);
            Grid.SetRow(Btn_Start, 1);
            Grid_Right.Children.Add(Btn_Start);
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