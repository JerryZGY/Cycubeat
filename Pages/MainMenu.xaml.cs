using System.Windows.Controls;
using Cycubeat.Controls;

namespace Cycubeat.Pages
{
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void StartGame()
        {
            Grid_Main.Children.Remove(Ctrl_Start);
            Ctrl_Start = new StartControl();
            Grid_Main.Children.Insert(0, Ctrl_Start);
            Btn_Start.IsHitTestVisible = true;
        }

        private void Btn_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Btn_Start.IsHitTestVisible = false;
            Ctrl_Start.Start();
            Ctrl_Start.NotifyEvent += new NotifyDelegate(StartGame);
        }
    }
}
