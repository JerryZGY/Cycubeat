using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cycubeat.Controls;
using Microsoft.Kinect;

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
            Focus();
            isPeeking = false;
            countdownTimes = 84;//84
            score = 0;
            stageID = 0;
            snd = new SoundHandler();
            beaters = new Ctrl_Beat[9];
            numbers = new Ctrl_Numpad[10];
            beaterClick = new List<Action>();
            countDownTimer = new System.Windows.Forms.Timer() { Interval = 1000 };
            Grid_Left.Opacity = 0;
            Canvas_Pointer.Opacity = 0;
            Cnv_Main.Children.Clear();
            Tbx_Timer.Text = (countdownTimes + 1).ToString();
            countDownTimer.Tick += countDownTimer_Tick;
            Switcher.pageSwitcher.Canvas_Pointer.Opacity = 0;
            Switcher.pageSwitcher.TouchEvent += touchEvent;
            Switcher.pageSwitcher.kinect.HandEvent += handEvent;
            Switcher.pageSwitcher.kinect.BodyEvent += (b) => Img_UserView.Source = b;
        }

        private void handEvent(Point lHand, Point rHand, bool isLOpen, bool isROpen)
        {
            if (stageID == 0 && isBeaterInit)
            {
                Canvas.SetLeft(Cur_Main, rHand.X);
                Canvas.SetTop(Cur_Main, rHand.Y);
                Canvas.SetLeft(Cur_Sub, lHand.X);
                Canvas.SetTop(Cur_Sub, lHand.Y);
                TouchMapHandler.CheckTouch(rHand, isROpen, beaterClick);
                TouchMapHandler.CheckTouch(lHand, isLOpen, beaterClick);
            }
        }

        private Action beatAction(Ctrl_Beat btn)
        {
            return () => btn.Btn_Beat.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private Action clickNumAction(Ctrl_Numpad btn)
        {
            return () => btn.Btn_Beat.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private Action releaseNumAction(Ctrl_Numpad btn)
        {
            return () => btn.IsHitTestVisible = true;
        }

        private Action releaseFuncAction(Ctrl_Function btn)
        {
            return () => btn.IsHitTestVisible = true;
        }

        private Action clickFuncAction(Ctrl_Function btn)
        {
           return () => btn.Btn_Function.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private MouseEventArgs leave = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseLeaveEvent };
        private MouseEventArgs enter = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseEnterEvent };
        private List<Action> numpadLeave;
        private List<Action> numpadEnter;
        private List<Action> numpadClick;
        private List<Action> numpadRelease;
        private List<Action> beaterClick;

        private void touchEvent(KinectInputArgs e)
        {
            try
            {
                if (stageID == 1 || stageID == 3)
                {
                    TouchMapHandler.CheckTouch(e.Posotion, (e.InputState == InputState.Open),
                        () => { btn_Next.RdBtn.RaiseEvent(leave); },
                        () => { btn_Next.RdBtn.RaiseEvent(enter); },
                        () =>
                        {
                            if (btn_Next.IsHitTestVisible)
                                btn_Next.RdBtn.RaiseEvent(new RoutedEventArgs(RadioButton.CheckedEvent));
                        }
                    );
                }
                else if (stageID == 2)
                {
                    TouchMapHandler.CheckTouch(e.Posotion, (e.InputState == InputState.Open), numpadLeave, numpadEnter, numpadClick, numpadRelease);
                }
            }
            catch (Exception) { }
        }

        private bool isPeeking;

        private int score;

        private int stageID;

        private Ctrl_Difficulty btn_Next;

        private Ctrl_Beat[] beaters;

        private Ctrl_Numpad[] numbers;

        private Ctrl_Function btn_Return;

        private Ctrl_Function btn_Enter;

        private int countdownTimes;

        private SoundHandler snd;

        private const int rank_A = 30000;

        private const int rank_S = 70000;

        private bool isBeaterInit = false;

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
                    if (score >= rank_A)
                        initPassResult();
                    else
                        initResult();
                    stageID++;
                    break;
                case 1://Result
                    if (score >= rank_A)
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
        }

        private void initBeater()
        {
            Switcher.pageSwitcher.Bounds = Switcher.pageSwitcher.Map.PlayMap_Beat;
            for (int i = 0; i < beaters.Length; i++)
            {
                beaters[i] = new Ctrl_Beat();
                Canvas.SetLeft(beaters[i], controlsMap[i].X);
                Canvas.SetTop(beaters[i], controlsMap[i].Y);
                beaters[i].BeatEvent += (score) => updateScore(score);
                beaters[i].PopEvent += () => isPeeking = false;
                beaters[i].EnterStory(i * 0.05);
                beaterClick.Add(beatAction(beaters[i]));
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
            Ctrl_Music.Play();
            isBeaterInit = true;
            Canvas_Pointer.Opacity = 1;
        }

        private void initResult()
        {
            Canvas_Pointer.Opacity = 0;
            Switcher.pageSwitcher.Canvas_Pointer.Opacity = 1;
            Switcher.pageSwitcher.Bounds = Switcher.pageSwitcher.Map.PlayMap_Next;
            snd.PlayBooSound();
            Tbx_Timer.Text = "15";
            countdownTimes = 14;
            countDownTimer.Enabled = true;
            Tbx_Subtitle.Text = "挑戰失敗";
            Tbx_Cotent.FontSize = 60;
            Tbx_Cotent.Text = "請再接再厲";
            Img_Content.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Lose.png", UriKind.Relative));
            StoryHandler.Begin(this, "EnterResult");
            btn_Next = new Ctrl_Difficulty("Next", Brushes.White);
            btn_Next.EnterStory();
            btn_Next.TouchEvent += () =>
            {
                countDownTimer = null;
                Switcher.Switch(new Page_Start());
            };
            Canvas.SetLeft(btn_Next, 1133);
            Canvas.SetTop(btn_Next, 294);
            Cnv_Main.Children.Add(btn_Next);
        }

        private void initPassResult()
        {
            Canvas_Pointer.Opacity = 0;
            Switcher.pageSwitcher.Canvas_Pointer.Opacity = 1;
            Switcher.pageSwitcher.Bounds = Switcher.pageSwitcher.Map.PlayMap_Next;
            snd.PlayCheerSound();
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
            Switcher.pageSwitcher.Bounds = Switcher.pageSwitcher.Map.PlayMap_Numpad;
            Tbx_Timer.Text = "60";
            countdownTimes = 59;
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
            btn_Return = new Ctrl_Function("Return", Brushes.DarkGreen);
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

            btn_Enter = new Ctrl_Function("Enter", Brushes.Navy);
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
            numpadLeave = new List<Action>()
            {
                () => numbers[1].Btn_Beat.RaiseEvent(leave), () => numbers[2].Btn_Beat.RaiseEvent(leave), () => numbers[3].Btn_Beat.RaiseEvent(leave), () => btn_Return.Btn_Function.RaiseEvent(leave),
                () => numbers[4].Btn_Beat.RaiseEvent(leave), () => numbers[5].Btn_Beat.RaiseEvent(leave), () => numbers[6].Btn_Beat.RaiseEvent(leave), () => numbers[0].Btn_Beat.RaiseEvent(leave),
                () => numbers[7].Btn_Beat.RaiseEvent(leave), () => numbers[8].Btn_Beat.RaiseEvent(leave), () => numbers[9].Btn_Beat.RaiseEvent(leave), () => btn_Enter.Btn_Function.RaiseEvent(leave)
            };

            numpadEnter = new List<Action>()
            {
                () => numbers[1].Btn_Beat.RaiseEvent(enter), () => numbers[2].Btn_Beat.RaiseEvent(enter), () => numbers[3].Btn_Beat.RaiseEvent(enter), () => btn_Return.Btn_Function.RaiseEvent(enter),
                () => numbers[4].Btn_Beat.RaiseEvent(enter), () => numbers[5].Btn_Beat.RaiseEvent(enter), () => numbers[6].Btn_Beat.RaiseEvent(enter), () => numbers[0].Btn_Beat.RaiseEvent(enter),
                () => numbers[7].Btn_Beat.RaiseEvent(enter), () => numbers[8].Btn_Beat.RaiseEvent(enter), () => numbers[9].Btn_Beat.RaiseEvent(enter), () => btn_Enter.Btn_Function.RaiseEvent(enter)
            };

            numpadClick = new List<Action>()
            {
                clickNumAction(numbers[1]), clickNumAction(numbers[2]), clickNumAction(numbers[3]), clickFuncAction(btn_Return),
                clickNumAction(numbers[4]), clickNumAction(numbers[5]), clickNumAction(numbers[6]), clickNumAction(numbers[0]),
                clickNumAction(numbers[7]), clickNumAction(numbers[8]), clickNumAction(numbers[9]), clickFuncAction(btn_Enter)
            };

            numpadRelease = new List<Action>()
            {
                releaseNumAction(numbers[1]), releaseNumAction(numbers[2]), releaseNumAction(numbers[3]), releaseFuncAction(btn_Return),
                releaseNumAction(numbers[4]), releaseNumAction(numbers[5]), releaseNumAction(numbers[6]), releaseNumAction(numbers[0]),
                releaseNumAction(numbers[7]), releaseNumAction(numbers[8]), releaseNumAction(numbers[9]), releaseFuncAction(btn_Enter)
            };
            stageID++;
        }

        private void initLottery()
        {
            writeScoreData();
            Tbx_Timer.Text = "15";
            countdownTimes = 14;
            countDownTimer.Enabled = true;
            StoryHandler.Begin(this, "EnterLottery");
            btn_Next = new Ctrl_Difficulty("Next", Brushes.White);
            btn_Next.EnterStory();
            btn_Next.TouchEvent += () =>
            {
                countDownTimer = null;
                Switcher.Switch(new Page_Start());
            };
            Canvas.SetLeft(btn_Next, 1133);
            Canvas.SetTop(btn_Next, 294);
            Cnv_Main.Children.Add(btn_Next);
            stageID++;
        }

        private void exitStory(ITouchable[] controls)
        {
            Array.ForEach(controls, x => x.ExitStory(() => x.RemoveSelf()));
        }

        private void updateScore(int score)
        {
            this.score += score;
            Ctrl_Score.UpdateScore(string.Format("{0:0000000}", this.score));
            if (this.score >= 0 && this.score < rank_A)
                Img_RankIcon.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Rank_B.png", UriKind.Relative));
            else if (this.score >= rank_A && this.score < rank_S)
                Img_RankIcon.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Rank_A.png", UriKind.Relative));
            else if (this.score >= rank_S)
                Img_RankIcon.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Rank_S.png", UriKind.Relative));
        }

        private void writeScoreData()
        {
            var id = Tbx_StudentID.Text;
            var user = Switcher.pageSwitcher.usersData[id];
            if (user != null)
                new ScoreDataHandler(id, user["Name"].ToString(), user["Depart"].ToString(), score);
            else
            {
                Tbx_Lottery.Text = "哎呀！真糟糕";
                Tbx_Info.Text = $"看樣子你沒有輸入資料{Environment.NewLine}或是輸入了錯誤的資料{Environment.NewLine}沒關係，你可以再挑戰一次！";
                Img_Info.Source = new BitmapImage(new Uri("/Cycubeat;component/Materials/Lose.png", UriKind.Relative));
            }
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

        private void RenderPointer(PointF position)
        {
            double y = position.Y * 768;
            y = (y > 720) ? 720 : y;
            Canvas.SetLeft(Cur_Main, position.X * 1366 - 25);
            Canvas.SetTop(Cur_Main, position.Y * 768 - 35);
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (stageID == 2)
            {
                switch (e.Key)
                {
                    case Key.D0:
                    case Key.NumPad0:
                        enterNumber(0);
                        break;
                    case Key.D1:
                    case Key.NumPad1:
                        enterNumber(1);
                        break;
                    case Key.D2:
                    case Key.NumPad2:
                        enterNumber(2);
                        break;
                    case Key.D3:
                    case Key.NumPad3:
                        enterNumber(3);
                        break;
                    case Key.D4:
                    case Key.NumPad4:
                        enterNumber(4);
                        break;
                    case Key.D5:
                    case Key.NumPad5:
                        enterNumber(5);
                        break;
                    case Key.D6:
                    case Key.NumPad6:
                        enterNumber(6);
                        break;
                    case Key.D7:
                    case Key.NumPad7:
                        enterNumber(7);
                        break;
                    case Key.D8:
                    case Key.NumPad8:
                        enterNumber(8);
                        break;
                    case Key.D9:
                    case Key.NumPad9:
                        enterNumber(9);
                        break;
                    case Key.Back:
                        if (Tbx_StudentID.Text.Length > 0)
                        {
                            Tbx_StudentID.Text = Tbx_StudentID.Text.Remove(Tbx_StudentID.Text.Length - 1);
                            searchUserData();
                        }
                        break;
                    case Key.Enter:
                        btn_Return.ExitStory(() => Cnv_Main.Children.Remove(btn_Return));
                        btn_Enter.ExitStory(() => Cnv_Main.Children.Remove(btn_Enter));
                        exitStory(numbers);
                        exitStage();
                        break;
                }
            }
        }
    }
}