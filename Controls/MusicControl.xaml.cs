using System;
using System.Windows.Controls;

namespace Cycubeat.Controls
{
    public partial class MusicControl : UserControl
    {
        public MusicControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Music.Source = new Uri("Music/BGM.mp3", UriKind.Relative);
            Music.Volume = 0;
            Music.Play();
        }
    }
}