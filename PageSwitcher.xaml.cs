using System;
using System.Windows;
using System.Windows.Controls;

namespace Cycubeat
{
    public partial class PageSwitcher : Window
    {
        public PageSwitcher()
        {
            InitializeComponent();
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
