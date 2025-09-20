using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MacroTalk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

            ((MainWindowViewModel)DataContext).PropertyChanging += MainWindow_PropertyChanging;
            ((MainWindowViewModel)DataContext).PropertyChanged += MainWindow_PropertyChanged;

            RichTextBoxEditWindow window = new();
            window.Show();
        }

        private int _previousIndex;
        private int _currentIndex;

        private void MainWindow_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            if (e.PropertyName != nameof(MainWindowViewModel.MenuSelectedIndex))
                return;
            int index = (int)typeof(MainWindowViewModel).GetProperty(e.PropertyName!)!.GetValue(DataContext)!;
            _previousIndex = index;
        }

        private void MainWindow_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(MainWindowViewModel.MenuSelectedIndex))
                return;
            int index = (int)typeof(MainWindowViewModel).GetProperty(e.PropertyName!)!.GetValue(DataContext)!;
            if (_previousIndex == 4)
            {
                ((TranslateTransform)MenuBar.RenderTransform).Y = 50 * index;
            }
            float intensity = Math.Abs(index - _previousIndex);
            float result = -intensity / 4 + 1.25f;
            Storyboard storyboard = new();
            DoubleAnimation doubleAnimation = new()
            {
                To = _previousIndex == 4 ? 0 : (index == 4 ? -4 : 50 * index),
                Duration = new TimeSpan(0, 0, 0, 0, 400),
                EasingFunction = new BackEase()
                {
                    Amplitude =  (_previousIndex == 4 || index == 4) ? 1 : result,
                    EasingMode = EasingMode.EaseInOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, MenuBar);
            Storyboard.SetTargetProperty(doubleAnimation, (_previousIndex == 4 || index == 4) ? new("(Border.RenderTransform).(TranslateTransform.X)") : new("(Border.RenderTransform).(TranslateTransform.Y)"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.FillBehavior = FillBehavior.Stop;
            storyboard.Completed += (o, e) =>
            {
                if (index == 4)
                {
                    ((TranslateTransform)MenuBar.RenderTransform).X = -4;
                    return;
                }
                if (_previousIndex == 4)
                {
                    ((TranslateTransform)MenuBar.RenderTransform).X = 0;
                    return;
                }
                ((TranslateTransform)MenuBar.RenderTransform).Y = 50 * index;
            };
            storyboard.Begin();
            storyboard = new();
            doubleAnimation = new()
            {
                To = index == 4 ? 0 : -4,
                Duration = new TimeSpan(0, 0, 0, 0, 400),
                EasingFunction = new BackEase()
                {
                    Amplitude = 1,
                    EasingMode = EasingMode.EaseInOut
                }
            };
            Storyboard.SetTarget(doubleAnimation, MenuSettingBar);
            Storyboard.SetTargetProperty(doubleAnimation, new("(Border.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(doubleAnimation);
            storyboard.FillBehavior = FillBehavior.Stop;
            storyboard.Completed += (o, e) =>
            {
                ((TranslateTransform)MenuSettingBar.RenderTransform).X = _currentIndex == 4 ? 0 : -4;
            };
            storyboard.Begin();
            storyboard = new();
            ThicknessAnimation thicknessAnimation = new()
            {
                To = new(0, -PagePanel.ActualHeight / 5 * index, 0, 0),
                Duration = new TimeSpan(0, 0, 0, 0, 400),
                EasingFunction = new BackEase()
                {
                    Amplitude = 0.2,
                    EasingMode = EasingMode.EaseInOut
                }
            };
            Storyboard.SetTarget(thicknessAnimation, PagePanel);
            Storyboard.SetTargetProperty(thicknessAnimation, new("Margin"));
            storyboard.Children.Add(thicknessAnimation);
            storyboard.FillBehavior = FillBehavior.Stop;
            storyboard.Completed += (o, e) =>
            {
                PagePanel.Margin = new(0, -PagePanel.ActualHeight / 5 * _currentIndex, 0, 0);
            };
            storyboard.Begin();
            _currentIndex = index;
            switch (index)
            {
                case 0:
                    Utils.ChangeMainWindowTitle(MainPageViewModel.Title);
                    break;
                case 1:
                    Utils.ChangeMainWindowTitle(FilePageViewModel.Title);
                    break;
                case 2:
                    Utils.ChangeMainWindowTitle(ProfilePageViewModel.Title);
                    break;
                case 3:
                    Utils.ChangeMainWindowTitle(ConversationPageViewModel.Title);
                    break;
                case 4:
                    Utils.ChangeMainWindowTitle(SettingPageViewModel.Title);
                    break;
                default:
                    break;
            }
        }

        private void PagePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PagePanel.Margin = new(0, -PagePanel.ActualHeight / 5 * _currentIndex, 0, 0);
        }
    }
}