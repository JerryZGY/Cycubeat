using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cycubeat.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cycubeat
{
    public delegate void DifficultyDelegate();

    public delegate void BeatDelegate(int score);

    public delegate void PeekDelegate(int id);

    public delegate void PopDelegate();

    public delegate void NumpadDelegate(int num);

    public delegate void FunctionDelegate();

    public delegate void BodyDelegate(WriteableBitmap bodyIndexBitmap);

    public delegate void HandDelegate(Point lHand, Point rHand, bool isLOpen, bool isROpen);

    public partial class PageSwitcher : Window
    {
        public JObject usersData;

        public List<int> TimingPoints;

        public List<int> HitPoints;

        private int gameTimes = 0;

        public PageSwitcher()
        {
            InitializeComponent();
            TimingPoints = new List<int>();
            HitPoints = new List<int>();
            readTimingPoints();
            usersData = getUsersData();

            //var kinect = new KinectHandler();
            //kinect.BodyEvent += (b) => bodyIndexBitmap = b;
            //kinect.HandEvent += (l, r, iL, iR) =>
            //{
            //Canvas.SetLeft(PointerFirst, l.X * 1.1);
            //Canvas.SetTop(PointerFirst, l.Y * 1.1);
            //Canvas.SetLeft(PointerSecond, r.X * 1.1);
            //Canvas.SetTop(PointerSecond, r.Y * 1.1);
            //Tbx_L.Text = $"X:{Convert.ToInt32(l.X).ToString()} Y:{Convert.ToInt32(l.Y).ToString()}";
            //Tbx_R.Text = $"X:{Convert.ToInt32(r.X).ToString()} Y:{Convert.ToInt32(l.X).ToString()}";

            //    if (Presenter.Content.GetType() == typeof(Page_Start))
            //    {
            //        if (l.X >= 400 && l.X <= 900 && iL)
            //        {
            //            ((Page_Start)Presenter.Content).Btn_Start.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            //        }

            //        if (r.X >= 400 && r.X <= 900 && iR)
            //        {
            //            ((Page_Start)Presenter.Content).Btn_Start.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            //        }
            //    }
            //    else if (Presenter.Content.GetType() == typeof(Page_Play))
            //    {
            //        var page = (Page_Play)Presenter.Content;
            //        var e = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseEnterEvent };
            //        var eL = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseLeaveEvent };
            //        var re = new RoutedEventArgs(RadioButton.CheckedEvent);

            //        if (page.touchers[0] != null && page.Btn_Start != null && page.Btn_Start.RdBtn != null)
            //        {
            //            page.touchers[0].RdBtn.RaiseEvent(eL);
            //            page.touchers[1].RdBtn.RaiseEvent(eL);
            //            page.touchers[2].RdBtn.RaiseEvent(eL);

            //            if (r.X >= 300 && r.X <= 500)
            //            {
            //                page.touchers[0].RdBtn.RaiseEvent(e);
            //            }
            //            else if(r.X >= 550 && r.X <= 750)
            //            {
            //                page.touchers[1].RdBtn.RaiseEvent(e);
            //            }
            //            else if (r.X >= 800 && r.X <= 1000)
            //            {
            //                page.touchers[2].RdBtn.RaiseEvent(e);
            //            }
            //            else if (r.X >= 1000 && r.X <= 1200)
            //            {
            //                page.Btn_Start.RdBtn.RaiseEvent(e);
            //                if (iR && re != null)
            //                {
            //                    page.Btn_Start.RdBtn.RaiseEvent(re);
            //                    re = null;
            //                }

            //            }
            //        }
            //    }
            //};

            CompositionTarget.Rendering += (s, e) =>
            {
                gameTimes++;
                updateCursor();
            };
            Switcher.pageSwitcher = this;
            Switcher.Switch(new Page_Start());
        }

        public void Navigate(UserControl nextPage)
        {
            GC.Collect();
            var prevPage = Presenter.Content as ISwitchable;
            if (prevPage != null)
                prevPage.ExitStory(() => Presenter.Content = nextPage);
            else
                Presenter.Content = nextPage;
        }

        private void readTimingPoints()
        {
            using (StreamReader sr = new StreamReader("Music/BGM.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    TimingPoints.Add(Convert.ToInt32(line) - 500);
                }
            }

            using (StreamReader sr = new StreamReader("Music/BeatMap.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    HitPoints.Add(Convert.ToInt32(line));
                }
            }
        }

        private void updateCursor()
        {
            var pos = Mouse.GetPosition(Canvas_Pointer);
            if (pos.X >= 0 && pos.Y >= 0)
            {
                Canvas.SetLeft(Cur, pos.X - 50);
                Canvas.SetTop(Cur, pos.Y - 50);
            }
        }

        private JObject getUsersData()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cycubeat.Assemblies.Data.json"))
            using (var reader = new StreamReader(stream))
            using (var jreader = new JsonTextReader(reader))
            {
                return new JsonSerializer().Deserialize<JObject>(jreader);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Cur.Down();
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Cur.Up();
        }
    }
}