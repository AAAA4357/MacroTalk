using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MacroTalk.DataStruct;

namespace MacroTalk
{
    public partial class FilePageViewModel : ObservableObject
    {
        public static string Title = "项目管理";

        [ObservableProperty]
        private int selectedPreviewIndex = -1;

        [RelayCommand]
        private void CreateFile()
        {

        }

        [RelayCommand]
        private void LoadFile()
        {

        }

        public List<ConversationPreview> ConversationPreviewList => Utils.SettingData.ConversationPreviews;

        public void UpdatePreview()
        {
            OnPropertyChanged(nameof(ConversationPreviewList));
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
