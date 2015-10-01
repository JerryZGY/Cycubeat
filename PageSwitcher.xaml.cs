using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
    public delegate void RefreshDelegate();

    public delegate void DifficultyDelegate();

    public delegate void BeatDelegate(int score);

    public delegate void PeekDelegate(int id);

    public delegate void PopDelegate();

    public delegate void NumpadDelegate(int num);

    public delegate void FunctionDelegate();

    public delegate void BodyDelegate(WriteableBitmap bodyIndexBitmap);

    public delegate void KinectUpdateEventHandler(bool isControling);

    public delegate void KinectInputEventHandler(KinectInputArgs e);

    public delegate void KinectTouchEventHandler(KinectInputArgs e);

    public delegate void HandDelegate(Point lHand, Point rHand, bool isLOpen, bool isROpen);

    public partial class PageSwitcher : Window
    {
        public JObject usersData;

        public List<int> TimingPoints;

        public List<int> HitPoints;

        public KinectHandler kinect;

        private bool isKinectControl = false;

        public KinectTouchMap Map;

        public List<TouchBounds> Bounds;


        public event KinectTouchEventHandler TouchEvent;

        public PageSwitcher()
        {
            InitializeComponent();
            TimingPoints = new List<int>();
            HitPoints = new List<int>();
            readTimingPoints();
            usersData = getUsersData();
            kinect = new KinectHandler();
            kinect.KinectUpdateEvent += (b) => isKinectControl = b;
            kinect.KinectInputEvent += kinectInputEvent;
            Map = new KinectTouchMap();
            CompositionTarget.Rendering += (s, e) =>
            {
                if (!isKinectControl)
                {
                    Cur.Visibility = Visibility.Visible;
                    updateCursor();
                }
            };
            Switcher.pageSwitcher = this;
            Switcher.Switch(new Page_Start());
        }

        private void kinectInputEvent(KinectInputArgs e)
        {
            if (e.IsValid)
            {
                TouchEvent(e);
                Cur.Visibility = Visibility.Visible;
                Canvas.SetLeft(Cur, e.Posotion.X);
                Canvas.SetTop(Cur, e.Posotion.Y);

                if (e.InputState == InputState.Open)
                    Cur.Down();
                else
                    Cur.Up();
            }
            else
                Cur.Visibility = Visibility.Collapsed;
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