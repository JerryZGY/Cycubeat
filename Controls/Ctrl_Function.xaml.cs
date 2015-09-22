using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cycubeat.Controls
{
    public partial class Ctrl_Function : UserControl, ITouchable
    {
        public event FunctionDelegate FuncEvent;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Ctrl_Function), new PropertyMetadata(""));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Ctrl_Function()
        {
            InitializeComponent();
        }

        public Ctrl_Function(string text, SolidColorBrush brush)
        {
            InitializeComponent();
            DataContext = this;
            Text = text;
            Eps_Effect.Fill = brush;
            Grid_Main.Opacity = 0;
            Tbx_Function.Opacity = 0;
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
            FuncEvent();
        }
    }
}
