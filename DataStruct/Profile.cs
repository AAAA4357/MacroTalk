using System.Windows.Media;

namespace MacroTalk.DataStruct
{
    public class Profile
    {
        public string Name { get; set; } = "";

        public bool IsCustom { get; set; } = false;

        public ImageInstance[] Avatars { get; set; } = [];

        public ImageInstance[][] Faces { get; set; } = [];

        public ImageSource FirstImage
        {
            get
            {
                return Avatars[0].Image;
            }
        }
    }
}
