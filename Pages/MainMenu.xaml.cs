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

        private void MainMenu_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            enterMainMenu();
        }

        private void enterPlaying()
        {
            Ctrl.Content = new PlayControl();
            var Ctrl_Start = Ctrl.Content as PlayControl;
            Ctrl_Start.NotifyEvent += new NotifyDelegate(enterMainMenu);
        }

        private void enterMainMenu()
        {
            Ctrl.Content = new StartControl();
            Btn_Start.IsHitTestVisible = true;
        }

        private void Btn_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Btn_Start.IsHitTestVisible = false;
            var Ctrl_Start = Ctrl.Content as StartControl;
            if (Ctrl_Start != null)
            {
                Ctrl_Start.Start();
                Ctrl_Start.NotifyEvent += new NotifyDelegate(enterPlaying);
            }
        }
    }
}
