using MacroTalk.DataStruct;
using System.IO;

namespace MacroTalk
{
    public static class ProfileManager
    {
        public static List<Profile> ProfileList = [];

        public static List<string> LoadPathList = ["Prefab_Macro Talk"];

        public static void LoadProfile()
        {
            foreach (string path in LoadPathList)
            {
                DirectoryInfo info = new("Plugins\\" + path);
                foreach (DirectoryInfo studentDirectory in info.EnumerateDirectories())
                {
                    List<ImageInstance> images = [];
                    List<string> names = [];
                    foreach (FileInfo profileImage in studentDirectory.EnumerateFiles())
                    {
                        images.Add(new(profileImage));
                        names.Add(profileImage.Name.Replace(profileImage.Extension, null));
                    }

                    List<ImageInstance>[] faces = new List<ImageInstance>[names.Count];

                    foreach (DirectoryInfo faceDirectory in studentDirectory.EnumerateDirectories())
                    {
                        int index = names.IndexOf(faceDirectory.Name);
                        foreach (FileInfo face in faceDirectory.EnumerateFiles())
                        {
                            faces[index].Add(new(face));
                        }
                    }

                    Profile profile = new()
                    {
                        Name = studentDirectory.Name,
                        Avatars = images.ToArray(),
                        Faces = faces.Select(list => list is null ? [new()] : list.ToArray()).ToArray()
                    };
                    ProfileList.Add(profile);
                }
            }
        }
    }
}
