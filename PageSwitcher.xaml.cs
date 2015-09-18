using System;
using System.Windows;
using Cycubeat.Pages;

namespace Cycubeat
{
    public partial class PageSwitcher : Window
    {
        public PageSwitcher()
        {
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch(new Page_Start());
        }

        public void Navigate(System.Windows.Controls.UserControl nextPage)
        {
            Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;
            if (s != null)
                s.Initialize();
            else
                throw new ArgumentException("NextPage is not ISwitchable! " + nextPage.Name.ToString());
        }
    }
}