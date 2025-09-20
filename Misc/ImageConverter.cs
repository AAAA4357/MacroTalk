using MacroTalk.DataStruct;
using Newtonsoft.Json;
using System.IO;

namespace MacroTalk
{
    public class ImageConverter : JsonConverter<ImageInstance>
    {
        public override ImageInstance? ReadJson(JsonReader reader, Type objectType, ImageInstance? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            ImageInstance image = null!;
            // 检查当前是否是对象开始
            if (reader.TokenType == JsonToken.Null)
                return null;
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException($"Expected object start, got {reader.TokenType}");
            // 读取对象内容
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                    break;
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = reader.Value.ToString();
                    reader.Read(); // 移动到属性值
                    if (propertyName != "Image" || reader.TokenType != JsonToken.String)
                        continue;
                    string imagePath = reader.Value.ToString()!;
                    image = new(new FileInfo(ConversationManager.CurrentConversationPath + imagePath));
                }
            }
            return image;
        }

        public override void WriteJson(JsonWriter writer, ImageInstance? value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Image");
            writer.WriteValue(value.FilePath.Replace(ConversationManager.CurrentConversationPath, null));
            writer.WriteEndObject();
        }
    }
}
