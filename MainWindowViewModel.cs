using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MacroTalk
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            Utils.MainWindowViewModel = this;
        }

        [ObservableProperty]
        private string menuTitle = "Macro Talk v6.0 - 主页";

        [ObservableProperty]
        private bool menuExtended;

        [ObservableProperty]
        private int menuSelectedIndex;

        [RelayCommand]
        private void MenuButtonClick()
        {
            MenuExtended = !MenuExtended;
        }

        [RelayCommand]
        private void MenuButtonHomeClick()
        {
            MenuSelectedIndex = 0;
        }

        [RelayCommand]
        private void MenuButton1Click()
        {
            MenuSelectedIndex = 1;
        }

        [RelayCommand]
        private void MenuButton2Click()
        {
            MenuSelectedIndex = 2;
        }

        [RelayCommand]
        private void MenuButton3Click()
        {
            MenuSelectedIndex = 3;
        }

        [RelayCommand]
        private void MenuButtonSettingClick()
        {
            MenuSelectedIndex = 4;
        }

        [RelayCommand]
        private void MenuBackClick()
        {
            MenuExtended = false;
        }
    }

    public class Double2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((double)value == 0)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }
    }
}
