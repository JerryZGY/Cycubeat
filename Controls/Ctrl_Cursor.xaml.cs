using System.Windows.Controls;

namespace Cycubeat.Controls
{
    public partial class Ctrl_Cursor : UserControl
    {
        public Ctrl_Cursor()
        {
            InitializeComponent();
        }

        public void Down()
        {
            StoryHandler.Begin(this, "Down_Cursor");
        }

        public void Up()
        {
            StoryHandler.Begin(this, "Up_Cursor");
        }
    }
}
