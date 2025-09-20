using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using MacroTalk.DataStruct;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace MacroTalk
{
    public partial class ConversationPageViewModel : ObservableObject
    {
        public ConversationPageViewModel()
        {
            foreach (Dialogue dialogue in ConversationManager.CurrentConversation.Dialogues)
            {
                DialogueList.Add(dialogue);
            };

            DialogueList.CollectionChanged += DialogueList_CollectionChanged;
            PropertyChanged += ConversationPageViewModel_PropertyChanged;

            DragHandler.Predicate = AddDialogueClickCheck;

            DragHandler.viewModel = this;
            ReplyDragHandler.viewModel = this;

            Utils.ConversationPageViewModel = this;
            ConversationManager.viewModel = this;
        }

        public static string Title = ConversationManager.CurrentConversation.Name;

        [ObservableProperty]
        private int selectedToolModeIndex = 0;

        [ObservableProperty]
        private int selectedDialogueTypeIndex = 0;

        [ObservableProperty]
        private int selectedDialogueIndentTypeIndex = 0;

        [ObservableProperty]
        private int dialogueIndentTarget = 0;

        [ObservableProperty]
        private int dialogueIndentCounter = 0;

        [ObservableProperty]
        private int selectedProfileTypeIndex = 0;

        [ObservableProperty]
        private int selectedProfileIndex = -1;

        [ObservableProperty]
        private int selectedProfileSkinIndex = -1;

        [ObservableProperty]
        private int selectedProfileFaceIndex = -1;

        [ObservableProperty]
        private int selectedContentTypeIndex = 1;

        [ObservableProperty]
        private int selectedInsertIndex = 0;

        [ObservableProperty]
        private int selectedInsertTypeIndex = 0;

        [ObservableProperty]
        private int selectedReplyIndex = -1;

        [ObservableProperty]
        private int selectedDialogueIndex = -1;

        [ObservableProperty]
        private int selectedCutModeIndex = 0;

        [ObservableProperty]
        private int selectedLengthModeIndex = 0;

        [ObservableProperty]
        private DialogueDragHandler dragHandler = new();

        [ObservableProperty]
        private DialogueDropHandler dropHandler = new();

        [ObservableProperty]
        private ReplyDragHandler replyDragHandler = new();

        [ObservableProperty]
        private ReplyDropHandler replyDropHandler = new();

        [ObservableProperty]
        private ObservableCollection<Profile> profileList = [];

        [ObservableProperty]
        private ObservableCollection<ImageInstance> profileSkinList = [];

        [ObservableProperty]
        private ImageInstance imageContent = null!;

        [ObservableProperty]
        private string mainContent = "";

        [ObservableProperty]
        private FlowDocument richContent = null!;

        [ObservableProperty]
        private string markNoteContent = "";

        [ObservableProperty]
        private ObservableCollection<Dialogue> dialogueList = [];

        [ObservableProperty]
        private ObservableCollection<string> replyList = [];

        [ObservableProperty]
        private string replyContent = "";

        [RelayCommand]
        private void ClearIndentCounterClick()
        {
            DialogueIndentCounter = 0;
        }

        private bool InsertDialogueClickCheck()
        {
            return AddDialogueClickCheck() && DialogueList.Count > 0;
        }

        [RelayCommand(CanExecute = nameof(InsertDialogueClickCheck))]
        private void InsertDialogueClick()
        {
            RefreshData();
            if (Data is null)
                return;
            DialogueList.Insert(SelectedInsertTypeIndex == 0 ? SelectedInsertIndex - 1 : SelectedInsertIndex, Data);
        }

        private bool AddDialogueClickCheck()
        {
            return SelectedDialogueTypeIndex switch
            {
                //普通对话
                0 => (SelectedProfileIndex != -1 && SelectedProfileSkinIndex != -1) &&
                     ((SelectedContentTypeIndex == 0 && ImageContent != null) ||
                      (SelectedContentTypeIndex == 1 && MainContent != "") ||
                      (SelectedContentTypeIndex == 2 && MainContent != "") ||
                      (SelectedContentTypeIndex == 3 && RichContent != null)),
                //旁白对话
                1 => (SelectedContentTypeIndex == 0 && ImageContent != null) ||
                     (SelectedContentTypeIndex == 1 && MainContent != "") ||
                     (SelectedContentTypeIndex == 2 && MainContent != "") ||
                     (SelectedContentTypeIndex == 3 && RichContent != null),
                //标记对话
                2 => true,
                //回复对话
                3 => ReplyList.Count != 0,
                //羁绊对话
                4 => SelectedProfileIndex != -1,
                //表情对话
                5 => (SelectedProfileIndex != -1 && SelectedProfileSkinIndex != -1) &&
                     SelectedProfileFaceIndex != -1,
                _ => false,
            };
        }

        [RelayCommand(CanExecute = nameof(AddDialogueClickCheck))]
        private void AddDialogueClick()
        {
            RefreshData();
            if (Data is null)
                return;
            DialogueList.Add(Data);
            InsertDialogueClickCommand.NotifyCanExecuteChanged();
        }

        private bool AddReplyClickCheck()
        {
            return ReplyContent != "";
        }

        [RelayCommand(CanExecute = nameof(AddReplyClickCheck))]
        private void AddReplyClick()
        {
            ReplyList.Add(ReplyContent);
        }

        private bool RemoveReplyClickCheck()
        {
            return ReplyList.Count != 0;
        }

        [RelayCommand(CanExecute = nameof(RemoveReplyClickCheck))]
        private void RemoveReplyClick()
        {
            if (SelectedReplyIndex == -1)
                ReplyList.RemoveAt(ReplyList.Count - 1);
            else
                ReplyList.RemoveAt(SelectedReplyIndex);
        }

        [RelayCommand]
        private void UndoDialogue()
        {
            if (undoList.Count == 0)
            {
                operationLock = true;
                DialogueList.Clear();
                operationLock = false;
                return;
            }
            operationLock = true;
            NotifyCollectionChangedEventArgs e = undoList.Pop();
            UndoCollection(e);
            redoList.Push(e);
            undoCheck = true;
            operationLock = false;
        }

        [RelayCommand]
        private void RedoDialogue()
        {
            if (!undoCheck)
                return;
            if (redoList.Count == 0)
            {
                undoCheck = false;
                return;
            }
            operationLock = true;
            NotifyCollectionChangedEventArgs e = redoList.Pop();
            RedoCollection(e);
            undoList.Push(e);
            operationLock = false;
            redoClear = true;
        }

        [RelayCommand]
        private void SaveConversation()
        {
            ConversationManager.SaveConversation();
        }

        [RelayCommand]
        private void LoadConversation()
        {
            ConversationManager.LoadConversation();
        }

        partial void OnSelectedProfileIndexChanged(int value)
        {
            ProfileSkinList.Clear();
            if (ConversationManager.CurrentConversation.Profiles.Count == 0 || value == -1)
                return;
            foreach (ImageInstance image in ConversationManager.CurrentConversation.Profiles[value].Avatars)
            {
                ProfileSkinList.Add(image);
            }
        }

        private void ConversationPageViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            AddDialogueClickCommand.NotifyCanExecuteChanged();
            InsertDialogueClickCommand.NotifyCanExecuteChanged();
            AddReplyClickCommand.NotifyCanExecuteChanged();
            removeReplyClickCommand.NotifyCanExecuteChanged();
        }

        private bool redoClear;

        private bool operationLock;

        private bool undoCheck;

        private FixedStack<NotifyCollectionChangedEventArgs> undoList = new(10);

        private Stack<NotifyCollectionChangedEventArgs> redoList = new();

        private Stack<List<Dialogue>> clearList = new();

        public Dialogue? Data = null;

        public void LoadConversationDialogues()
        {
            DialogueList.Clear();
            foreach (Dialogue dialogue in ConversationManager.CurrentConversation.Dialogues)
            {
                DialogueList.Add(dialogue);
            }
            undoList.Clear();
            redoList.Clear();
        }

        private void DialogueList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            int insertIndex = e.NewStartingIndex + i;
                            if (insertIndex <= ConversationManager.CurrentConversation.Dialogues.Count)
                                ConversationManager.CurrentConversation.Dialogues.Insert(insertIndex, (Dialogue)e.NewItems[i]!);
                            else
                                ConversationManager.CurrentConversation.Dialogues.Add((Dialogue)e.NewItems[i]!);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex >= 0 && e.OldItems != null)
                    {
                        for (int i = e.OldItems.Count - 1; i >= 0; i--)
                        {
                            if (e.OldStartingIndex + i < ConversationManager.CurrentConversation.Dialogues.Count)
                                ConversationManager.CurrentConversation.Dialogues.RemoveAt(e.OldStartingIndex + i);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewStartingIndex >= 0 && e.NewItems != null)
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            int replaceIndex = e.NewStartingIndex + i;
                            if (replaceIndex < ConversationManager.CurrentConversation.Dialogues.Count)
                                ConversationManager.CurrentConversation.Dialogues[replaceIndex] = (Dialogue)e.NewItems[i]!;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex >= 0 && e.NewStartingIndex >= 0 && e.NewItems != null)
                    {
                        var item = ConversationManager.CurrentConversation.Dialogues[e.OldStartingIndex];
                        ConversationManager.CurrentConversation.Dialogues.RemoveAt(e.OldStartingIndex);
                        ConversationManager.CurrentConversation.Dialogues.Insert(e.NewStartingIndex, item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Dialogue[] dialogues = new Dialogue[ConversationManager.CurrentConversation.Dialogues.Count];
                    ConversationManager.CurrentConversation.Dialogues.CopyTo(dialogues);
                    clearList.Push([.. dialogues]);
                    ConversationManager.CurrentConversation.Dialogues.Clear();
                    break;
            }
            if (!operationLock)
            {
                undoList.Push(e);
                if (redoClear)
                {
                    redoList.Clear();
                    redoClear = false;
                }
            }
            ConversationManager.isModified = true;
        }

        private void UndoCollection(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null && e.NewStartingIndex >= 0)
                    {
                        for (int i = e.NewItems.Count - 1; i >= 0; i--)
                        {
                            int removeIndex = e.NewStartingIndex + i;
                            if (removeIndex < DialogueList.Count)
                                DialogueList.RemoveAt(removeIndex);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null && e.OldStartingIndex >= 0)
                    {
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            if (e.OldStartingIndex + i <= DialogueList.Count)
                                DialogueList.Insert(e.OldStartingIndex + i, (Dialogue)e.OldItems[i]!);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null && e.OldItems != null && e.NewStartingIndex >= 0)
                    {
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            int replaceIndex = e.NewStartingIndex + i;
                            if (replaceIndex < DialogueList.Count)
                                DialogueList[replaceIndex] = (Dialogue)e.OldItems[i]!;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.NewStartingIndex >= 0 && e.OldStartingIndex >= 0 && e.NewItems != null)
                    {
                        var movedItem = DialogueList[e.NewStartingIndex];
                        DialogueList.RemoveAt(e.NewStartingIndex);
                        DialogueList.Insert(e.OldStartingIndex, movedItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (Dialogue dialogue in clearList.Pop())
                    {
                        DialogueList.Add(dialogue);
                    }
                    break;
            }
        }

        private void RedoCollection(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            int insertIndex = e.NewStartingIndex + i;
                            if (insertIndex <= DialogueList.Count)
                                DialogueList.Insert(insertIndex, (Dialogue)e.NewItems[i]!);
                            else
                                DialogueList.Add((Dialogue)e.NewItems[i]!);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex >= 0 && e.OldItems != null)
                    {
                        for (int i = e.OldItems.Count - 1; i >= 0; i--)
                        {
                            if (e.OldStartingIndex + i < DialogueList.Count)
                                DialogueList.RemoveAt(e.OldStartingIndex + i);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewStartingIndex >= 0 && e.NewItems != null)
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            int replaceIndex = e.NewStartingIndex + i;
                            if (replaceIndex < DialogueList.Count)
                                DialogueList[replaceIndex] = (Dialogue)e.NewItems[i]!;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex >= 0 && e.NewStartingIndex >= 0 && e.NewItems != null)
                    {
                        var item = DialogueList[e.OldStartingIndex];
                        DialogueList.RemoveAt(e.OldStartingIndex);
                        DialogueList.Insert(e.NewStartingIndex, item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    DialogueList.Clear();
                    break;
            }
        }

        public void RefreshData()
        {
            Dialogue dialogue = null!;
            switch (SelectedDialogueTypeIndex)
            {
                case 0:
                    //普通
                    switch (SelectedContentTypeIndex)
                    {
                        case 0:
                            //图片
                            switch (SelectedDialogueIndentTypeIndex)
                            {
                                case 0:
                                    //自动缩进
                                    dialogue = new()
                                    {
                                        DialogueType = DialogueIndentCounter == DialogueIndentTarget ? DialogueType.NoAvatar : DialogueType.Normal,
                                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                                        ContentType = ContentType.Image,
                                        ImageContent = ImageContent
                                    };
                                    break;
                                case 1:
                                    //强制缩进
                                    dialogue = new()
                                    {
                                        DialogueType = DialogueType.NoAvatar,
                                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                                        ContentType = ContentType.Image,
                                        ImageContent = ImageContent
                                    };
                                    break;
                                case 2:
                                    //强制不缩进
                                    dialogue = new()
                                    {
                                        DialogueType = DialogueType.Normal,
                                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                                        ContentType = ContentType.Image,
                                        ImageContent = ImageContent
                                    };
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 1:
                            //普通
                            switch (SelectedDialogueIndentTypeIndex)
                            {
                                case 0:
                                    //自动缩进
                                    dialogue = new()
                                    {
                                        DialogueType = DialogueIndentCounter == DialogueIndentTarget ? DialogueType.NoAvatar : DialogueType.Normal,
                                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                                        ContentType = ContentType.Normal,
                                        Content = MainContent
                                    };
                                    break;
                                case 1:
                                    //强制缩进
                                    dialogue = new()
                                    {
                                        DialogueType = DialogueType.NoAvatar,
                                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                                        ContentType = ContentType.Normal,
                                        Content = MainContent
                                    };
                                    break;
                                case 2:
                                    //强制不缩进
                                    dialogue = new()
                                    {
                                        DialogueType = DialogueType.Normal,
                                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                                        ContentType = ContentType.Normal,
                                        Content = MainContent
                                    };
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 2:
                            //Markdown
                            switch (SelectedDialogueIndentTypeIndex)
                            {
                                case 0:
                                    //自动缩进
                                    dialogue = new()
                                    {
                                        DialogueType = DialogueIndentCounter == DialogueIndentTarget ? DialogueType.NoAvatar : DialogueType.Normal,
                                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                                        ContentType = ContentType.Markdown,
                                        Content = MainContent
                                    };
                                    break;
                                case 1:
                                    //强制缩进
                                    dialogue = new()
                                    {
                                        DialogueType = DialogueType.NoAvatar,
                                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                                        ContentType = ContentType.Markdown,
                                        Content = MainContent
                                    };
                                    break;
                                case 2:
                                    //强制不缩进
                                    dialogue = new()
                                    {
                                        DialogueType = DialogueType.Normal,
                                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                                        ContentType = ContentType.Markdown,
                                        Content = MainContent
                                    };
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 3:
                            //富文本
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                    //旁白
                    switch (SelectedContentTypeIndex)
                    {
                        case 0:
                            //图片
                            dialogue = new()
                            {
                                DialogueType = DialogueType.Middle,
                                ContentType = ContentType.Image,
                                ImageContent = ImageContent
                            };
                            break;
                        case 1:
                            //普通
                            dialogue = new()
                            {
                                DialogueType = DialogueType.Middle,
                                ContentType = ContentType.Normal,
                                Content = MainContent
                            };
                            break;
                        case 2:
                            //Markdown
                            dialogue = new()
                            {
                                DialogueType = DialogueType.Middle,
                                ContentType = ContentType.Markdown,
                                Content = MainContent
                            };
                            break;
                        case 3:
                            //富文本
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    //标记
                    dialogue = new()
                    {
                        DialogueType = DialogueType.Mark,
                        Content = MarkNoteContent
                    };
                    break;
                case 3:
                    //回复对话
                    dialogue = new()
                    {
                        DialogueType = DialogueType.Custom,
                        Content = JsonConvert.SerializeObject(ReplyList.ToList()),
                        CustomTypeID = "Momotalk-回复对话"
                    };
                    break;
                case 4:
                    //羁绊对话
                    dialogue = new()
                    {
                        DialogueType = DialogueType.Custom,
                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                        CustomTypeID = "Momotalk-羁绊对话"
                    };
                    break;
                case 5:
                    //表情对话
                    dialogue = new()
                    {
                        DialogueType = DialogueType.Custom,
                        Avatar = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex]!.Avatars[SelectedProfileSkinIndex],
                        Name = ConversationManager.CurrentConversation.Profiles[SelectedProfileIndex].Name,
                        Content = JsonConvert.SerializeObject(ImageContent),
                        CustomTypeID = "Momotalk-表情对话"
                    };
                    break;
                default:
                    break;
            }
            Data = dialogue;
        }
    }

    public class DialogueDragHandler() : DefaultDragHandler
    {
        public ConversationPageViewModel? viewModel;

        public Func<bool> Predicate = () => false;

        public override void StartDrag(IDragInfo dragInfo)
        {
            if (!Predicate.Invoke())
                return;
            base.StartDrag(dragInfo);
            viewModel.RefreshData();
            dragInfo.Data = viewModel.Data;
        }
    }

    public class ReplyDragHandler : DefaultDragHandler
    {
        public ConversationPageViewModel? viewModel;

        public override void StartDrag(IDragInfo dragInfo)
        {
            base.StartDrag(dragInfo);
            dragInfo.Data = viewModel.ReplyContent;
        }
    }

    public class DialogueDropHandler : DefaultDropHandler
    {
        public override void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is string)
            {
                dropInfo.Effects = DragDropEffects.None;
                dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
                dropInfo.DropTargetHintState = DropHintState.Error;
                return;
            }
            base.DragOver(dropInfo);
        }
    }

    public class ReplyDropHandler : DefaultDropHandler
    {
        public ConversationPageViewModel? viewModel;

        public override void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is not string)
            {
                dropInfo.Effects = DragDropEffects.None;
                dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
                dropInfo.DropTargetHintState = DropHintState.Error;
                return;
            }
            base.DragOver(dropInfo);
        }
    }

    public class Uri2ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null!;
            else
                return new BitmapImage((Uri)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }
    }
}
