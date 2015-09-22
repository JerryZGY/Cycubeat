﻿using System.Windows;
using Cycubeat.Pages;

namespace Cycubeat
{
    public delegate void DifficultyDelegate();

    public delegate void BeatDelegate(int score);

    public delegate void PeekDelegate(int id);

    public delegate void NumpadDelegate(int num);

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
            var prevPage = Content as ISwitchable;
            if (prevPage != null)
                prevPage.ExitStory(() => Content = nextPage);
            else
                Content = nextPage;
        }
    }
}