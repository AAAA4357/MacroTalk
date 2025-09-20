using System.Windows.Controls;

namespace MacroTalk
{
    /// <summary>
    /// FilePage.xaml 的交互逻辑
    /// </summary>
    public partial class FilePage : Page
    {
        public FilePage()
        {
            InitializeComponent();

            DataContext = new FilePageViewModel();
        }
    }
}
