using System;
using System.Windows;
using System.Windows.Controls;

namespace Cycubeat.Controls
{
    public partial class Ctrl_Touch : UserControl
    {
        public event TouchEventHandler TouchEvent;

        public event ExitEventHandler ExitEvent;

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

        public void ExitStory()
        {
            IsHitTestVisible = false;
            VisualStateManager.GoToState(RdBtn, "Exit", false);
        }

        private void ExitStoryCompleted(object sender, EventArgs e)
        {
            ExitEvent(null, null);
        }

        public Ctrl_Touch()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Ctrl_Touch_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(RdBtn, "Enter", false);
        }

        private void RdBtn_Click(object sender, RoutedEventArgs e)
        {
            TouchEvent(sender, new TouchEventArgs(RdBtn.Foreground));
        }
    }
}