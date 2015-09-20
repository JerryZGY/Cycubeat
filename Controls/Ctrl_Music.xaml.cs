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

        private System.Windows.Forms.Timer peekTimer = new System.Windows.Forms.Timer() { Interval = 1 };

        private int i = 0;

        public Ctrl_Music()
        {
            InitializeComponent();
        }

        private void loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Music.Source = new Uri("Music/BGM.mp3", UriKind.Relative);
            Music.Volume = 1;
            peekTimer.Tick += tick;
        }

        private void tick(object sender, EventArgs e)
        {
            switch (i)
            {
                case 400:
                case 550:
                    PeekEvent(0);
                    PeekEvent(2);
                    break;
                case 450:
                case 600:
                    PeekEvent(3);
                    PeekEvent(5);
                    break;
                case 500:
                case 650:
                    PeekEvent(6);
                    PeekEvent(8);
                    break;
                default:
                    break;
            }
            i++;
        }
    }
}