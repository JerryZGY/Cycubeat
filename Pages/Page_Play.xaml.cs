using System;
using System.Windows;
using System.Windows.Controls;

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
            StoryHandler.Begin(this, "Enter");
        }

        public void ExitStory(Action callback)
        {
            StoryHandler.Begin(this, "Exit", () => callback());
        }

        public void InitializeProperty()
        {
            Grid_Left.Opacity = 0;
            Grid_Right.Opacity = 0;
        }

        private void loaded(object sender, RoutedEventArgs e)
        {
            InitializeProperty();
            EnterStory();
        }

        private void Btn_Extra_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Page_Start());
        }
    }
}
