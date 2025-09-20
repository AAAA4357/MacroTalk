using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MacroTalk.DataStruct;

namespace MacroTalk
{
    public partial class MainPageViewModel : ObservableObject
    {
        public static string Title = "主页";

        [ObservableProperty]
        private int selectedPreviewIndex = -1;

        public List<ConversationPreview> ConversationPreviewList => Utils.SettingData.ConversationPreviews.Take(3).ToList();

        public void UpdatePreview()
        {
            OnPropertyChanged(nameof(ConversationPreviewList));
        }

        [RelayCommand]
        private void EditProfileClick()
        {
            Utils.MainWindowViewModel.MenuSelectedIndex = 2;
        }

        [RelayCommand]
        private void NewConversationClick()
        {
            Utils.MainWindowViewModel.MenuSelectedIndex = 3;
        }

        [RelayCommand]
        private void SettingClick()
        {
            Utils.MainWindowViewModel.MenuSelectedIndex = 4;
        }

        partial void OnSelectedPreviewIndexChanged(int value)
        {
            if (value == -1)
                return;
            SelectedPreviewIndex = -1;
            ConversationManager.LoadConversation(ConversationPreviewList[value].Path);
        }
    }
}
