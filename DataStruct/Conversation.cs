namespace MacroTalk.DataStruct
{
    public class Conversation
    {
        public string Name = "新建Macro Talk对话";

        public ImageInstance Avatar = ImageInstance.LogoInstance;

        public int PluginIndex = 0;

        public List<string> ImageNames = [];

        public List<Profile> Profiles = [];

        public List<Dialogue> Dialogues = [];

        public DateTime LatestUpdateTime = DateTime.Now;
    }
}
