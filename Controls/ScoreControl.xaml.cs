using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cycubeat.Controls
{
    public partial class ScoreControl : UserControl
    {
        private TextBlock[] tbxs = new TextBlock[7];

        private System.Windows.Forms.Timer[] updateTimers = new System.Windows.Forms.Timer[7];

        public ScoreControl()
        {
            InitializeComponent();
        }

        public void UpdateScore(string score)
        {
            for (int i = 0; i < tbxs.Length; i++)
            {
                tbxs[i].Text = score.Substring(i, 1);
                updateTimers[i].Tag = score.Substring(i, 1);
                updateTimers[i].Start();
            }
        }

        private void updateScore(TextBlock tbx, object sender)
        {
            var timer = sender as System.Windows.Forms.Timer;
            var r = new Random(DateTime.Now.Millisecond + DateTime.Now.GetHashCode()).Next(0, 9);
            var initial = Convert.ToInt32(timer.Tag);
            if (initial < 9)
            {
                timer.Tag = Convert.ToInt32(timer.Tag) + 1;
                tbx.Text = timer.Tag.ToString();
            }
            else
            {
                tbx.Text = r.ToString();
                timer.Stop();
            }
        }

        private void ScoreControl_Loaded(object sender, RoutedEventArgs e)
        {
            Grid_Main.Children.Clear();
            for (int i = 0; i < tbxs.Length; i++)
            {
                tbxs[i] = new TextBlock()
                {
                    Text = "0",
                    FontSize = 85,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Vivaldi"),
                    Margin = new Thickness(i * 50, 0, 0, 0)
                };
                Grid_Main.Children.Add(tbxs[i]);
                updateTimers[i] = new System.Windows.Forms.Timer();
                updateTimers[i].Interval = 10;
            }
            updateTimers[4].Tick += (s, ev) => updateScore(tbxs[4], s);
            updateTimers[5].Tick += (s, ev) => updateScore(tbxs[5], s);
            updateTimers[6].Tick += (s, ev) => updateScore(tbxs[6], s);
        }
    }
}