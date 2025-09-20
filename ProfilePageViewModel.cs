using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using MacroTalk.DataStruct;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace MacroTalk
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        public ProfilePageViewModel()
        {
            ProfileManager.LoadProfile();
            LoadProfiles();

            SelectedProfileList.CollectionChanged += SelectedProfileList_CollectionChanged;

            //TODO:加载已选择档案
        }

        public static string Title = "档案管理";

        [ObservableProperty]
        private string searchStr = "";

        [ObservableProperty]
        private ObservableCollection<Profile> profileList = [];

        [ObservableProperty]
        private ObservableCollection<Profile> customProfileList = [];

        [ObservableProperty]
        private ObservableCollection<Profile> selectedProfileList = [];

        [ObservableProperty]
        private DropProfileHandler dropProfileHandler = new();

        [ObservableProperty]
        private DropCustomProfileHandler dropCustomProfileHandler = new();

        [ObservableProperty]
        private DropSelectedProfileHandler dropSelectedProfileHandler = new();

        [RelayCommand]
        private void CreateProfile()
        {
            //TODO:创建自定义档案
        }

        [RelayCommand]
        private void LoadProfile()
        {
            //TODO:加载外部档案
        }

        partial void OnSearchStrChanged(string value)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(ProfileList);
            cv.Filter = element =>
            {
                return ((Profile)element).Name.Contains(value);
            };
            cv = CollectionViewSource.GetDefaultView(CustomProfileList);
            cv.Filter = element =>
            {
                return ((Profile)element).Name.Contains(value);
            };
            cv = CollectionViewSource.GetDefaultView(SelectedProfileList);
            cv.Filter = element =>
            {
                return ((Profile)element).Name.Contains(value);
            };
        }

        private void SelectedProfileList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Utils.ConversationPageViewModel.ProfileList.Clear();
            ConversationManager.CurrentConversation.Profiles.Clear();
            foreach (Profile profile in SelectedProfileList)
            {
                Utils.ConversationPageViewModel.ProfileList.Add(profile);
                ConversationManager.CurrentConversation.Profiles.Add(profile);
            }
        }

        public void LoadProfiles()
        {
            foreach (var profile in ProfileManager.ProfileList)
            {
                ProfileList.Add(profile);
            }
        }

        public void MoveProfiles()
        {
            List<Profile> moveProfiles = [];
            foreach (var profile in ConversationManager.CurrentConversation.Profiles)
            {
                foreach (var test in ProfileList)
                {
                    if (test.Name == profile.Name)
                    {
                        moveProfiles.Add(profile);
                    }
                }
            }
            moveProfiles.ForEach(profile =>
            {
                ProfileList.Remove(profile);
                SelectedProfileList.Add(profile);
            });
        }
    }

    public class DropSelectedProfileHandler : DefaultDropHandler
    {
        public override void Drop(IDropInfo dropInfo)
        {
            base.Drop(dropInfo);
            //TODO:档案头像文件处理
        }
    }

    public class DropProfileHandler : DefaultDropHandler
    {
        public override void DragOver(IDropInfo dropInfo)
        {
            if (((Profile)dropInfo.Data).IsCustom)
            {
                dropInfo.Effects = DragDropEffects.None;
                dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
                dropInfo.DropTargetHintState = DropHintState.Error;
                return;
            }
            base.DragOver(dropInfo);
        }
    }

    public class DropCustomProfileHandler : DefaultDropHandler
    {
        public override void DragOver(IDropInfo dropInfo)
        {
            if (!((Profile)dropInfo.Data).IsCustom)
            {
                dropInfo.Effects = DragDropEffects.None;
                dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
                dropInfo.DropTargetHintState = DropHintState.Error;
                return;
            }
            base.DragOver(dropInfo);
        }
    }
}
