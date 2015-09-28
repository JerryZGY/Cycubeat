using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            StoryHandler.Begin(this, "Enter", () =>
            {
                countDownTimer.Enabled = true;
                initBeater();
            });
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
            isPeeking = false;
            countdownTimes = 84;
            score = 0;
            stageID = 0;
            beaters = new Ctrl_Beat[9];
            numbers = new Ctrl_Numpad[10];
            countDownTimer = new System.Windows.Forms.Timer() { Interval = 1000 };
            Grid_Left.Opacity = 0;
            Cnv_Main.Children.Clear();
            Tbx_Timer.Text = (countdownTimes + 1).ToString();
            countDownTimer.Tick += countDownTimer_Tick;
        }

        private bool isPeeking;

        private int score;

        private int stageID;

        public Ctrl_Difficulty btn_Next;

        public Ctrl_Beat[] beaters;

        public Ctrl_Numpad[] numbers;

        private int countdownTimes;

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
            new Point(403, 104), new Point(593, 104), new Point(783, 104),
            new Point(403, 294), new Point(593, 294), new Point(783, 294),
            new Point(403, 484), new Point(593, 484), new Point(783, 484)
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
                case 0://Play
                    Ctrl_Music.Stop();
                    exitStory(beaters);
                    if (score >= 38000)
                        initPassResult();
                    else
                        initResult();
                    stageID++;
                    break;
                case 1://Result
                    if (score >= 38000)
                    {
                        btn_Next.ExitStory(() => Cnv_Main.Children.Remove(btn_Next));
                        exitStage();
                    }
                    else
                    {
                        countDownTimer = null;
                        Switcher.Switch(new Page_Start());
                    }
                    break;
                case 2://Numpad
                case 3://Lottery
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
                case 1:
                    StoryHandler.Begin(this, "ExitResult", () => initNumpad());
                    break;
                case 2:
                    initLottery();
                    break;
            }
            stageID++;
        }

        private void initBeater()
        {
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
        }

        private void initResult()
        {
            Tbx_Timer.Text = "10";
            countdownTimes = 9;
            countDownTimer.Enabled = true;
            Tbx_Subtitle.Text = "挑戰失敗";
            Tbx_Cotent.FontSize = 60;
            Tbx_Cotent.Text = "請再接再厲";
            StoryHandler.Begin(this, "EnterResult");
            var btn_next = new Ctrl_Difficulty("Next", Brushes.White);
            btn_next.EnterStory();
            btn_next.TouchEvent += () =>
            {
                countDownTimer = null;
                Switcher.Switch(new Page_Start());
            };
            Canvas.SetLeft(btn_next, 1133);
            Canvas.SetTop(btn_next, 294);
            Cnv_Main.Children.Add(btn_next);
        }

        private void initPassResult()
        {
            Tbx_Timer.Text = "15";
            countdownTimes = 14;
            countDownTimer.Enabled = true;
            StoryHandler.Begin(this, "EnterResult");
            btn_Next = new Ctrl_Difficulty("Next", Brushes.White);
            btn_Next.EnterStory();
            btn_Next.TouchEvent += () =>
            {
                btn_Next.ExitStory(() => Cnv_Main.Children.Remove(btn_Next));
                exitStage();
            };
            Canvas.SetLeft(btn_Next, 1133);
            Canvas.SetTop(btn_Next, 294);
            Cnv_Main.Children.Add(btn_Next);
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
                    Canvas.SetLeft(numbers[i], 1133);
                    Canvas.SetTop(numbers[i], 294);
                    Cnv_Main.Children.Add(numbers[i]);
                }
                else
                {
                    Canvas.SetLeft(numbers[i], controlsMap[i - 1].X);
                    Canvas.SetTop(numbers[i], controlsMap[i - 1].Y);
                    Cnv_Main.Children.Add(numbers[i]);
                }
            }
            var btn_Return = new Ctrl_Function("Return", Brushes.DarkGreen);
            btn_Return.FuncEvent += () =>
            {
                if (Tbx_StudentID.Text.Length > 0)
                {
                    Tbx_StudentID.Text = Tbx_StudentID.Text.Remove(Tbx_StudentID.Text.Length - 1);
                    searchUserData();
                }
            };
            btn_Return.EnterStory();
            Canvas.SetLeft(btn_Return, 1133);
            Canvas.SetTop(btn_Return, 104);
            Cnv_Main.Children.Add(btn_Return);

            var btn_Enter = new Ctrl_Function("Enter", Brushes.Navy);
            btn_Enter.FuncEvent += () =>
            {
                btn_Return.ExitStory(() => Cnv_Main.Children.Remove(btn_Return));
                btn_Enter.ExitStory(() => Cnv_Main.Children.Remove(btn_Enter));
                exitStory(numbers);
                exitStage();
            };
            btn_Enter.EnterStory();
            Canvas.SetLeft(btn_Enter, 1133);
            Canvas.SetTop(btn_Enter, 484);
            Cnv_Main.Children.Add(btn_Enter);
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
                countDownTimer = null;
                Switcher.Switch(new Page_Start());
            };
            Canvas.SetLeft(btn_next, 1133);
            Canvas.SetTop(btn_next, 294);
            Cnv_Main.Children.Add(btn_next);
        }

        private void exitStory(ITouchable[] controls)
        {
            Array.ForEach(controls, x => x.ExitStory(() => x.RemoveSelf()));
        }

        private void updateScore(int score)
        {
            this.score += score;
            Ctrl_Score.UpdateScore(string.Format("{0:0000000}", this.score));
            if (this.score >= 0 && this.score < 38000)
                Img_RankIcon.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Rank_B.png", UriKind.Relative));
            else if (this.score >= 38000 && this.score < 70000)
                Img_RankIcon.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Rank_A.png", UriKind.Relative));
            else if (this.score >= 70000)
                Img_RankIcon.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Rank_S.png", UriKind.Relative));
        }

        private void enterNumber(int num)
        {
            if (Tbx_StudentID.Text.Length < 8)
            {
                Tbx_StudentID.Text += num.ToString();
                searchUserData();
            }
        }

        private void searchUserData()
        {
            Tbx_StudentData.Text = "無資料";
            var user = Switcher.pageSwitcher.usersData[Tbx_StudentID.Text];
            if (user != null)
            {
                var depart = user["Depart"];
                var name = user["Name"];
                Tbx_StudentData.Text = $"{depart} {name}同學";
                Tbx_Info.Text = $"{depart}　{name}同學　你好{Environment.NewLine}活動結束後將進行天梯結算{Environment.NewLine}前100名的同學即可獲得禮券{Environment.NewLine}歡迎持續挑戰追求更高的分數！";
            }
        }
    }
}