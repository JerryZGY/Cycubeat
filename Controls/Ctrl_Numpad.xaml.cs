using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cycubeat.Controls
{
    public partial class Ctrl_Numpad : UserControl, ITouchable
    {
        public event NumpadDelegate NumEvent;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Ctrl_Numpad), new PropertyMetadata(""));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Ctrl_Numpad()
        {
            InitializeComponent();
        }

        public Ctrl_Numpad(string text)
        {
            InitializeComponent();
            DataContext = this;
            Text = text;
            Grid_Main.Opacity = 0;
            Tbx_Numpad.Opacity = 0;
            Eps_Effect.Opacity = 0;
            Eps_Effect.RenderTransform = new ScaleTransform(0, 0);
        }

        public void EnterStory()
        {
            EnterStory(0);
        }

        public void EnterStory(double beginTime)
        {
            StoryHandler.Begin(this, "Enter", beginTime, () => IsHitTestVisible = true);
        }

        public void ExitStory(Action callback)
        {
            IsHitTestVisible = false;
            StoryHandler.Begin(this, "Exit", callback);
        }

        public void RemoveSelf()
        {
            var parent = Parent as Grid;
            if (parent != null)
                parent.Children.Remove(this);
            else
                ((Canvas)Parent).Children.Remove(this);
        }

        private void mouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StoryHandler.Begin(this, "MouseEnter");
        }

        private void mouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StoryHandler.Begin(this, "MouseExit");
        }

        private void click(object sender, RoutedEventArgs e)
        {
            NumEvent(Convert.ToInt32(Text));
        }
    }
}
