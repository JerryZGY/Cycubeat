using System;
using System.Windows;
using System.Windows.Controls;

namespace Cycubeat
{
    public delegate void NotifyDelegate();

    public delegate void ScoreEventHandler(int score);

    public partial class PageSwitcher : Window
    {
        public PageSwitcher()
        {
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch("MainMenu");
        }

        public void Navigate(UserControl nextPage)
        {
            Content = nextPage;
        }

        public void Navigate(UserControl nextPage, object state)
        {
            Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;
            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable! " + nextPage.Name.ToString());
        }
    }
}
