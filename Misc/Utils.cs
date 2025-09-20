using MacroTalk.DataStruct;

namespace MacroTalk
{
    public class Utils
    {
        public static MainWindowViewModel MainWindowViewModel = null!;

        public static MainPageViewModel MainPageViewModel = null!;

        public static FilePageViewModel FilePageViewModel = null!;

        public static ProfilePageViewModel ProfilePageViewModel = null!;

        public static ConversationPageViewModel ConversationPageViewModel = null!;

        public static ApplicationSettings SettingData = new();

        public static void ChangeMainWindowTitle(string title)
        {
            MainWindowViewModel.MenuTitle = "Macro Talk v6.0 - " + title;
        }
    }
}
