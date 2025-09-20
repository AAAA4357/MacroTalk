using Newtonsoft.Json;
using System.IO;
using System.Windows.Media.Imaging;

namespace MacroTalk.DataStruct
{
    [JsonConverter(typeof(ImageConverter))]
    public class ImageInstance
    {
        public ImageInstance() { }

        public ImageInstance(FileInfo file)
        {
            FilePath = file.FullName;
        }

        public ImageInstance(Uri uri)
        {
            FilePath = uri.LocalPath;
        }

        public string Name = "";

        public string FilePath = "";

        public BitmapImage Image
        {
            get
            {
                if (!File.Exists(FilePath)) FilePath = "pack://application:,,,/MacroTalk;component/Resources/Logo.png";
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(FilePath);
                bitmap.EndInit();
                return bitmap.Clone();
            }
        }

        public static readonly ImageInstance LogoInstance = new();
    }
}
