using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Cycubeat.Pages;

namespace Cycubeat
{
    public delegate void DifficultyDelegate();

    public delegate void BeatDelegate(int score);

    public delegate void PeekDelegate(int id);

    public delegate void PopDelegate();

    public delegate void NumpadDelegate(int num);

    public delegate void FunctionDelegate();

    public delegate void HandDelegate(Point lHand, Point rHand, bool isLOpen, bool isROpen);

    public partial class PageSwitcher : Window
    {
        public List<int> TimingPoints;

        public List<int> HitPoints;

        public WriteableBitmap bodyIndexBitmap = null;

        public PageSwitcher()
        {
            InitializeComponent();
            TimingPoints = new List<int>();
            HitPoints = new List<int>();
            readTimingPoints();
            var kinect = new KinectHandler();
            kinect.HandEvent += (l, r, iL, iR) =>
            {
                Canvas.SetLeft(PointerFirst, l.X);
                Canvas.SetTop(PointerFirst, l.Y);
                Canvas.SetLeft(PointerSecond, l.X);
                Canvas.SetTop(PointerSecond, l.Y);
            };
            Switcher.pageSwitcher = this;
            Switcher.Switch(new Page_Start());
        }

        public void Navigate(System.Windows.Controls.UserControl nextPage)
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
    }
}