using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cycubeat.Controls
{
    public partial class Ctrl_Touch : UserControl
    {
        public event TouchDelegate TouchEvent;

        public event ExitDelegate ExitEvent;

        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(Ctrl_Touch), new PropertyMetadata(""));
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Ctrl_Touch), new PropertyMetadata(""));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public void EnterStory(double beginTime)
        {
            StoryHandler.Begin(this, "Enter", beginTime, () => IsHitTestVisible = true);
        }

        public void EnterStory(double beginTime, Action callback)
        {
            StoryHandler.Begin(this, "Enter", beginTime, () =>
            {
                IsHitTestVisible = true;
                callback();
            });
        }

        public void ExitStory()
        {
            IsHitTestVisible = false;
            if (ExitEvent != null)
                StoryHandler.Begin(this, "Exit", () => ExitEvent());
            else
                StoryHandler.Begin(this, "Exit");
        }

        private bool isChecked = false;

        public Ctrl_Touch()
        {
            InitializeComponent();
            DataContext = this;
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Grid_Main.Opacity = 0;
                RdBtn.Opacity = 0;
                Eps_Effect.Opacity = 0;
                Eps_Effect.RenderTransform = new ScaleTransform(0, 0);
            }
        }

        private void mouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isChecked)
                StoryHandler.Begin(this, "MouseEnter");
        }

        private void mouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isChecked)
                StoryHandler.Begin(this, "MouseExit");
        }

        private void check(object sender, RoutedEventArgs e)
        {
            TouchEvent();
            isChecked = true;
        }

        private void uncheck(object sender, RoutedEventArgs e)
        {
            StoryHandler.Begin(this, "MouseExit");
            isChecked = false;
        }
    }
}