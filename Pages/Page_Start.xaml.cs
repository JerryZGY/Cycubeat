using System.Windows.Controls;

namespace Cycubeat.Pages
{
    public partial class Page_Start : UserControl, ISwitchable
    {
        public Page_Start()
        {
            InitializeComponent();
        }

        void ISwitchable.Initialize()
        {
        }

        void ISwitchable.Utilize()
        {
        }
    }
}