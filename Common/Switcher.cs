using Cycubeat.Pages;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Cycubeat
{
    public static class Switcher
    {
        public static PageSwitcher pageSwitcher;
        public static Dictionary<string, UserControl> PageDictionary = new Dictionary<string, UserControl>()
        {
            {"MainMenu", new MainMenu()}
        };

        public static void Switch(string switchPage)
        {
            pageSwitcher.Navigate(PageDictionary[switchPage]);
        }

        public static void Switch(UserControl newPage)
        {
            pageSwitcher.Navigate(newPage);
        }

        public static void Switch(UserControl newPage, object state)
        {
            pageSwitcher.Navigate(newPage, state);
        }
    }
}
