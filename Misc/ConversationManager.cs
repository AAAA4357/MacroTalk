using HandyControl.Controls;
using MacroTalk.DataStruct;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MacroTalk
{
    public static class ConversationManager
    {
        public static Conversation CurrentConversation { get; private set; } = new();

        public static ConversationPageViewModel viewModel = null!;

        static bool _IsModified = false;
        public static bool isModified 
        {
            get
            {
                return _IsModified;
            }
            set
            {
                _IsModified = value;
                Utils.ChangeMainWindowTitle(CurrentConversation.Name + (value ? "*" : ""));
            }        
        }

        public static string CurrentConversationPath => CurrentConversationFolderPath + CurrentConversation.Name + ProjectFileExtension;

        public static string CurrentConversationFolderPath = "";

        public const string ProjectFileExtension = ".mtproj";

        public static void LoadConversation(string path)
        {
            FileInfo file = new(path);
            if (!file.Exists)
                throw new FileNotFoundException("指定工程文件不存在");
            try
            {
                string json = File.ReadAllText(file.FullName);
                CurrentConversation = JsonConvert.DeserializeObject<Conversation>(json);
                Utils.ConversationPageViewModel.LoadConversationDialogues();
            }
            catch
            {
                MessageBox.Warning("工程文件已损坏或不支持的格式！");
                return;
            }
            CloseCurrentConversation();
            if (isModified)
            {
                if (MessageBox.Ask("是否保存？") == System.Windows.MessageBoxResult.Yes)
                {
                    SaveConversation();
                }
            }
            CurrentConversationFolderPath = file.DirectoryName + "\\";
        }

        public static void LoadConversation()
        {
            OpenFileDialog ofd = new()
            {
                Title = "请选择工程文件",
                Filter = "Macro Talk工程文件|*.mtproj"
            };
            if (ofd.ShowDialog() != true)
                return;
            LoadConversation(ofd.FileName);
        }

        public static void SaveConversation(string path)
        {
            if (!File.Exists(path))
                File.Create(path).Dispose();
            string json = JsonConvert.SerializeObject(CurrentConversation);
            File.WriteAllText(path, json);
            isModified = false;
            if (Utils.SettingData.ConversationPreviews.Find(preview => preview.Name == CurrentConversation.Name) is ConversationPreview conversation)
            {
                Utils.SettingData.ConversationPreviews.Remove(conversation);
            }
            ConversationPreview preview = new(CurrentConversation, CurrentConversationPath);
            Utils.SettingData.ConversationPreviews.Add(preview);
            Utils.MainPageViewModel.UpdatePreview();
            Utils.FilePageViewModel.UpdatePreview();
        }

        public static void SaveConversation()
        {
            if (CurrentConversationFolderPath == "")
                SaveConversationNoPath();
            else
                SaveConversation(CurrentConversationPath);
        }

        public static bool SaveConversationNoPath()
        {
            OpenFolderDialog ofd = new()
            {
                Title = "请选择保存位置"
            };
            if (ofd.ShowDialog() != true)
                return false;
            var conversationInfo = new ConversationInfo();
            MiscWindow window = new();
            conversationInfo = (ConversationInfo)window.ShowDialog("设置工程属性", conversationInfo);
            CurrentConversation.Name = conversationInfo.Name;
            CurrentConversation.Avatar = new(new FileInfo(((BitmapImage)conversationInfo.Avatar).BaseUri.LocalPath));
            CurrentConversationFolderPath = ofd.FolderName + "\\";
            SaveConversation(CurrentConversationPath);
            return true;
        }

        public static void CloseCurrentConversation()
        {
            CurrentConversation = new();
            CurrentConversationFolderPath = "";
            CurrentConversationFolderPath = "";
        }

        private class ConversationInfo
        {
            public string Name = "";
            public ImageSource Avatar = ImageInstance.LogoInstance.Image;
        }
    }
}
