namespace MacroTalk.DataStruct
{
    public class ConversationPreview(Conversation conversation, string path)
    {
        public string Name = conversation.Name;

        public ImageInstance Avatar = conversation.Avatar;

        public DateTime Latest = conversation.LatestUpdateTime;

        public string Path = path;
    }
}
