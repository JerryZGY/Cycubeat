using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Cycubeat.Controls;

namespace Cycubeat.Pages
{
    public partial class Page_Play : UserControl, ISwitchable
    {
        public event TouchEventHandler TouchEvent;

        public Page_Play()
        {
            InitializeComponent();
        }

        public void EnterStory()
        {
            StoryHandler.Begin(this, "Enter", () => initDifficulty());
        }

        public void ExitStory(Action callback)
        {
            StoryHandler.Begin(this, "Exit", () => callback());
        }

        public void InitializeProperty()
        {
            Grid_Left.Opacity = 0;
            Grid_Right.Opacity = 0;
            Cnv_Main.Children.Clear();
        }

        private void loaded(object sender, RoutedEventArgs e)
        {
            InitializeProperty();
            EnterStory();
        }

        private void Btn_Extra_Click(object sender, RoutedEventArgs e)
        {
            //Switcher.Switch(new Page_Start());
            TouchEvent(sender, new TouchEventArgs());
        }

        private void initDifficulty()
        {
            Ctrl_Touch[] touchers =
            {
                new Ctrl_Touch()
                {
                    Text = "Easy",
                    Foreground = Brushes.GreenYellow
                },
                new Ctrl_Touch()
                {
                    Text = "Normal",
                    Foreground = Brushes.Gold
                },
                new Ctrl_Touch()
                {
                    Text = "Hard",
                    Foreground = Brushes.Tomato
                }
            };

            for (int i = 0; i < touchers.Length; i++)
            {
                touchers[i].GroupName = "Difficulty";
                Canvas.SetLeft(touchers[i], 343 + i * 250);
                Canvas.SetTop(touchers[i], 294);
                touchers[i].TouchEvent += (s, e) =>
                {
                    Tbx_Difficulty.Text = touchers[i].Text;
                    Tbx_Difficulty.Foreground = touchers[i].Foreground;
                };
                touchers[i].ExitEvent += (s, e) => Cnv_Main.Children.Clear();
                Cnv_Main.Children.Add(touchers[i]);
            }

            TouchEvent += (s, e) =>
            {
                foreach (var x in touchers)
                    x.ExitStory();
            };
        }
    }
}