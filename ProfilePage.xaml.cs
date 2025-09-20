using System.Windows.Controls;

namespace MacroTalk
{
    /// <summary>
    /// ProfilePage.xaml 的交互逻辑
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();

            DataContext = new ProfilePageViewModel();
        }
    }
}
