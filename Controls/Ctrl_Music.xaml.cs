using System;
using System.Windows.Controls;

namespace Cycubeat.Controls
{
    public partial class Ctrl_Music : UserControl
    {
        public event PeekDelegate PeekEvent;

        public void Play()
        {
            peekTimer.Enabled = true;
            Music.Play();
        }

        public void Stop()
        {
            peekTimer.Enabled = false;
            Music.Stop();
        }

        private System.Windows.Forms.Timer peekTimer = new System.Windows.Forms.Timer() { Interval = 1 };

        private int i = 0;

        public Ctrl_Music()
        {
            InitializeComponent();
        }

        private void loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Music.Source = new Uri("Music/BGM.mp3", UriKind.Relative);
            Music.Volume = 2;
            peekTimer.Tick += tick;
        }

        private void tick(object sender, EventArgs e)
        {
            if (i < 70)
            {
                var musicTimingPoint = Convert.ToInt32(Music.Position.TotalMilliseconds);
                var isPeeking = false;
                foreach (var timingPoint in Switcher.pageSwitcher.TimingPoints)
                {
                    if (musicTimingPoint - timingPoint >= -10 && musicTimingPoint - timingPoint < 10 && !isPeeking)
                    {
                        isPeeking = true;
                        PeekEvent(Switcher.pageSwitcher.HitPoints[i]);
                        i++;
                        break;
                    }
                }
            }
        }
    }
}