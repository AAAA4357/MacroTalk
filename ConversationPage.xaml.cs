using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MacroTalk
{
    /// <summary>
    /// ConversationPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConversationPage : Page
    {
        public ConversationPage()
        {
            InitializeComponent();

            DataContext = new ConversationPageViewModel();

            ((ConversationPageViewModel)DataContext).PropertyChanged += ConversationPage_PropertyChanged;
            ((ConversationPageViewModel)DataContext).DialogueList.CollectionChanged += DialogueList_CollectionChanged;

            //TODO:预览渲染
            //TODO:图片导出，视频导出
        }

        private bool _openGuide = false;

        private void ConversationPage_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ToolboxUpdate(e);
            ProfileUpdate(e);
            ContentTypeUpdate(e);
            CardUpdate(e);
            CutModeUpdate(e);
            LengthModeUpdate(e);
        }

        private void DialogueList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void OpenCutGuide_Click(object sender, RoutedEventArgs e)
        {
            CutGuideFadeIn();
            _openGuide = true;
        }

        private void CancelCutGuide_Click(object sender, RoutedEventArgs e)
        {
            CutGuideFadeOut(ClearCutGuide);
            LengthModeFadeOut();
            LengthSettingFadeOut();
            _openGuide = false;
        }

        private void ConfirmCutGuide_Click(object sender, RoutedEventArgs e)
        {
            CutGuideFadeOut(ClearCutGuide);
            LengthModeFadeOut();
            LengthSettingFadeOut();
            _openGuide = false;
        }

        private void ImageSelector_ImageSelected(object sender, RoutedEventArgs e)
        {
            if (ConversationManager.CurrentConversationFolderPath == "")
            {
                if (!ConversationManager.SaveConversationNoPath())
                {
                    HandyControl.Controls.MessageBox.Show("请先选择保存路径再添加图片");
                    ImageSelector.SetValue(HandyControl.Controls.ImageSelector.UriPropertyKey, default(Uri));
                    ImageSelector.SetValue(HandyControl.Controls.ImageSelector.PreviewBrushPropertyKey, default(Brush));
                    ImageSelector.SetValue(HandyControl.Controls.ImageSelector.HasValuePropertyKey, false);
                    ImageSelector.SetCurrentValue(ToolTipProperty, default);
                    ImageSelector.RaiseEvent(new RoutedEventArgs(HandyControl.Controls.ImageSelector.ImageUnselectedEvent, ImageSelector));
                    return;
                }
            }
            FileInfo info = new(ImageSelector.Uri.LocalPath);
            File.Copy(info.FullName, ConversationManager.CurrentConversationFolderPath + info.Name);
            ((ConversationPageViewModel)DataContext).ImageContent = new(ImageSelector.Uri);
        }

        private void ImageSelector_ImageUnselected(object sender, RoutedEventArgs e)
        {
            ((ConversationPageViewModel)DataContext).ImageContent = null!;
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
            Focus();
        }

        private void ClearCutGuide()
        {
            ((ConversationPageViewModel)DataContext).SelectedCutModeIndex = 0;
            ((ConversationPageViewModel)DataContext).SelectedLengthModeIndex = 0;
        }

        private void ProfileUpdate(PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ConversationPageViewModel.SelectedProfileTypeIndex))
                return;
            if (((ConversationPageViewModel)DataContext).SelectedDialogueTypeIndex == 4)
                return;
            if (((ConversationPageViewModel)DataContext).SelectedProfileTypeIndex != 0)
            {
                ProfileSelector.IsEnabled = true;
                SkinSelector.IsEnabled = true;
                Card3FadeIn();
            }
            else
            {
                ProfileSelector.IsEnabled = false;
                SkinSelector.IsEnabled = false;
                Card3FadeOut();
            }
        }

        private void ToolboxUpdate(PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ConversationPageViewModel.SelectedToolModeIndex))
                return;
            switch (((ConversationPageViewModel)DataContext).SelectedToolModeIndex)
            {
                case 0:
                    PanelFadeOut(() =>
                    {
                        EditPanel.Visibility = Visibility.Visible;
                        ExportPanel.Visibility = Visibility.Collapsed;
                        PanelFadeIn();
                    });
                    break;
                case 1:
                    PanelFadeOut(() =>
                    {
                        EditPanel.Visibility = Visibility.Collapsed;
                        ExportPanel.Visibility = Visibility.Collapsed;
                    });
                    break;
                case 2:
                    PanelFadeOut(() =>
                    {
                        EditPanel.Visibility = Visibility.Collapsed;
                        ExportPanel.Visibility = Visibility.Visible;
                        PanelFadeIn();
                    });
                    break;
                default:
                    break;
            }
        }

        private void ContentTypeUpdate(PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ConversationPageViewModel.SelectedContentTypeIndex))
                return;
            switch (((ConversationPageViewModel)DataContext).SelectedContentTypeIndex)
            {
                case 0:
                    ImageSelector.IsEnabled = true;
                    PlainTextbox.IsEnabled = false;
                    RichButton.IsEnabled = false;
                    Card6FadeOut();
                    Card7FadeOut();
                    Card5FadeIn();
                    break;
                case 1:
                    ImageSelector.IsEnabled = false;
                    PlainTextbox.IsEnabled = true;
                    RichButton.IsEnabled = false;
                    Card5FadeOut();
                    Card6FadeOut();
                    Card7FadeOut();
                    break;
                case 2:
                    ImageSelector.IsEnabled = false;
                    PlainTextbox.IsEnabled = true;
                    RichButton.IsEnabled = false;
                    Card5FadeOut();
                    Card7FadeOut();
                    Card6FadeIn();
                    break;
                case 3:
                    ImageSelector.IsEnabled = false;
                    PlainTextbox.IsEnabled = false;
                    RichButton.IsEnabled = true;
                    Card5FadeOut();
                    Card6FadeOut();
                    Card7FadeIn();
                    break;
                default:
                    break;
            }
        }

        private void CardUpdate(PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ConversationPageViewModel.SelectedDialogueTypeIndex))
                return;
            switch (((ConversationPageViewModel)DataContext).SelectedDialogueTypeIndex)
            {
                case 0:
                    if (((ConversationPageViewModel)DataContext).SelectedProfileTypeIndex == 0)
                    {
                        ProfileSelector.IsEnabled = true;
                        SkinSelector.IsEnabled = true;
                    }
                    Card3FadeOut();
                    Card8FadeOut();
                    Card9FadeOut();
                    Card1FadeIn();
                    Card2FadeIn();
                    Card4FadeIn();
                    if (((ConversationPageViewModel)DataContext).SelectedProfileTypeIndex != 0)
                        Card3FadeIn();
                    switch (((ConversationPageViewModel)DataContext).SelectedContentTypeIndex)
                    {
                        case 0:
                            Card5FadeIn();
                            break;
                        case 2:
                            Card6FadeIn();
                            break;
                        case 3:
                            Card7FadeIn();
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                    if (((ConversationPageViewModel)DataContext).SelectedProfileTypeIndex == 0)
                    {
                        ProfileSelector.IsEnabled = true;
                        SkinSelector.IsEnabled = true;
                    }
                    Card2FadeOut();
                    Card3FadeOut();
                    Card8FadeOut();
                    Card9FadeOut();
                    Card1FadeIn();
                    Card4FadeIn();
                    if (((ConversationPageViewModel)DataContext).SelectedProfileTypeIndex != 0)
                        Card3FadeIn();
                    switch (((ConversationPageViewModel)DataContext).SelectedContentTypeIndex)
                    {
                        case 0:
                            Card5FadeIn();
                            break;
                        case 2:
                            Card6FadeIn();
                            break;
                        case 3:
                            Card7FadeIn();
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    ProfileSelector.IsEnabled = false;
                    SkinSelector.IsEnabled = false;
                    Card1FadeOut();
                    Card2FadeOut();
                    Card3FadeOut();
                    Card8FadeOut();
                    Card9FadeOut();
                    Card4FadeIn();
                    switch (((ConversationPageViewModel)DataContext).SelectedContentTypeIndex)
                    {
                        case 0:
                            Card5FadeIn();
                            break;
                        case 2:
                            Card6FadeIn();
                            break;
                        case 3:
                            Card7FadeIn();
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    ProfileSelector.IsEnabled = false;
                    SkinSelector.IsEnabled = false;
                    Card1FadeOut();
                    Card2FadeOut();
                    Card3FadeOut();
                    Card4FadeOut();
                    Card5FadeOut();
                    Card6FadeOut();
                    Card7FadeOut();
                    Card8FadeOut();
                    Card9FadeIn();
                    break;
                case 4:
                    ProfileSelector.IsEnabled = false;
                    SkinSelector.IsEnabled = false;
                    Card1FadeOut();
                    Card2FadeOut();
                    Card3FadeOut();
                    Card4FadeOut();
                    Card5FadeOut();
                    Card6FadeOut();
                    Card7FadeOut();
                    Card9FadeOut();
                    Card8FadeIn();
                    break;
                case 5:
                    SkinSelector.IsEnabled = false;
                    ProfileSelector.IsEnabled = true;
                    Card1FadeOut();
                    Card3FadeOut();
                    Card4FadeOut();
                    Card5FadeOut();
                    Card6FadeOut();
                    Card7FadeOut();
                    Card8FadeOut();
                    Card9FadeOut();
                    Card2FadeIn();
                    break;
                case 6:
                    ProfileSelector.IsEnabled = true;
                    SkinSelector.IsEnabled = true;
                    Card1FadeOut();
                    Card4FadeOut();
                    Card5FadeOut();
                    Card6FadeOut();
                    Card7FadeOut();
                    Card8FadeOut();
                    Card9FadeOut();
                    Card2FadeIn();
                    Card3FadeIn();
                    break;
                default:
                    break;
            }
        }

        private void CutModeUpdate(PropertyChangedEventArgs e)
        {
            if (!_openGuide)
                return;
            if (e.PropertyName != nameof(ConversationPageViewModel.SelectedCutModeIndex))
                return;
            if (((ConversationPageViewModel)DataContext).SelectedCutModeIndex == 2)
            {
                CutGuideFadeIn();
                LengthModeFadeIn();
                if (((ConversationPageViewModel)DataContext).SelectedLengthModeIndex == 0)
                    LengthSettingFadeIn();
            }
            else
            {
                CutGuideFadeIn();
                LengthModeFadeOut();
                LengthSettingFadeOut();
            }
        }

        private void LengthModeUpdate(PropertyChangedEventArgs e)
        {
            if (!_openGuide)
                return;
            if (e.PropertyName != nameof(ConversationPageViewModel.SelectedLengthModeIndex))
                return;
            if (((ConversationPageViewModel)DataContext).SelectedLengthModeIndex == 0)
            {
                CutGuideFadeIn();
                LengthSettingFadeIn();
            }
            else
            {
                CutGuideFadeIn();
                LengthSettingFadeOut();
            }
        }

        private void PanelFadeOut(Action callback)
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = ToolBorder.ActualWidth + 20,
                Duration = new TimeSpan(0, 0, 0, 0, 400),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, ToolBorder);
            Storyboard.SetTargetProperty(doubleAnimation, new("(Border.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Completed += (o, e) => callback();
            storyboard.Begin();
        }

        private void PanelFadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 400),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseIn
                }
            };
            Storyboard.SetTarget(doubleAnimation, ToolBorder);
            Storyboard.SetTargetProperty(doubleAnimation, new("(Border.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card1FadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 170,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card1);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card1FadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card1);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card2FadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 360,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card2);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card2FadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card2);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card3FadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 180,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card3);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card3FadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card3);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card4FadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 360,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card4);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card4FadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card4);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card5FadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 220,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card5);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card5FadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card5);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card6FadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 220,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card6);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card6FadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card6);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card7FadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 220,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card7);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card7FadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card7);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card8FadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 250,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card8);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card8FadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card8);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card9FadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 150,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card9);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void Card9FadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, Card9);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void CutGuideFadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 160 +
                    (((ConversationPageViewModel)DataContext).SelectedCutModeIndex == 2 ? 60 +
                    (((ConversationPageViewModel)DataContext).SelectedLengthModeIndex == 0 ? 60 : 0) : 0),
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, CutGuide);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void CutGuideFadeOut(Action callback)
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, CutGuide);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Completed += (o, e) => callback();
            storyboard.Begin();
        }

        private void LengthModeFadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 60,
                Duration = new TimeSpan(0, 0, 0, 0, 400),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, LengthMode);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void LengthModeFadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 400),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, LengthMode);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void LengthSettingFadeIn()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 60,
                Duration = new TimeSpan(0, 0, 0, 0, 400),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, LengthSetting);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        private void LengthSettingFadeOut()
        {
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 400),
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, LengthSetting);
            Storyboard.SetTargetProperty(doubleAnimation, new("Height"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }
    }
}
