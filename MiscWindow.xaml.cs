using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace MacroTalk
{
    /// <summary>
    /// MiscWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MiscWindow : Window, INotifyPropertyChanged
    {
        public MiscWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string WindowTitle { get; set; } = "";

        public object SerializableObject { get; set; } = null!;

        [Obsolete("使用ShowDialog(object)", true)]
        public new void Show()
        {

        }

        [Obsolete("使用ShowDialog(object)", true)]
        public new void ShowDialog()
        {

        }

        public object ShowDialog(string title, object serializableObject)
        {
            WindowTitle = title;
            PropertyChanged.Invoke(this, new(nameof(WindowTitle)));
            SerializableObject = serializableObject;
            PropertyChanged.Invoke(this, new(nameof(SerializableObject)));
            base.ShowDialog();
            return SerializableObject;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
