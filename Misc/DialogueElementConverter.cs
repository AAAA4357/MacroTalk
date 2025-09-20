using MacroTalk.DataStruct;
using MdXaml;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace MacroTalk
{
    public class DialogueElementConverter : IValueConverter
    {
        const string PrefixObject = ">";
        const string PrefixList = "-";
        const string SuffixProperty = ":";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Dialogue)
                return null!;
            return GetElement((Dialogue)value, bool.Parse(parameter.ToString()!));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }

        public FrameworkElement GetElement(Dialogue dialogue, bool RemoveMargin = false)
        {
            Grid element;
            Image avatarImage;
            Border contentBorder;
            TextBlock nameTextBlock;
            TextBlock contentTextBlock;
            MarkdownScrollViewer contentMarkdownTextBlock;
            RichTextBox contentRichTextBox;
            Image contentImage;
            TextBlock titleTextBlock;
            if (dialogue.DialogueType != DialogueType.Custom)
            {
                switch (dialogue.DialogueType)
                {
                    case DialogueType.Normal:
                        element = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Normal"]);
                        avatarImage = (Image)((Border)(element.Children[0])).Child;
                        nameTextBlock = (TextBlock)element.Children[1];
                        avatarImage.Source = dialogue.Avatar.Image;
                        nameTextBlock.Text = dialogue.Name;
                        contentBorder = (Border)element.Children[2];
                        break;
                    case DialogueType.NoAvatar:
                        element = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_NoAvatar"]);
                        contentBorder = (Border)element.Children[0];
                        if (RemoveMargin)
                            ((Border)element.Children[0]).Margin = new();
                        break;
                    case DialogueType.Sender:
                        element = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Sender"]);
                        contentBorder = (Border)element.Children[0];
                        if (RemoveMargin)
                            ((Border)element.Children[0]).Margin = new();
                        break;
                    case DialogueType.Middle:
                        element = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Middle"]);
                        contentBorder = (Border)element.Children[0];
                        if (RemoveMargin)
                            ((Border)element.Children[0]).Margin = new();
                        break;
                    case DialogueType.Mark:
                        element = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Mark"]);
                        contentTextBlock = (TextBlock)((Grid)((Border)element.Children[0]).Child).Children[2];
                        contentTextBlock.Text = dialogue.Content;
                        if (RemoveMargin)
                            ((Border)element.Children[0]).Margin = new();
                        return element;
                    default:
                        return null!;
                }
                switch (dialogue.ContentType)
                {
                    case ContentType.Normal:
                        contentTextBlock = (TextBlock)DeepCopy((TextBlock)Application.Current.Resources["Element_ContentNormal"]);
                        contentTextBlock.Text = dialogue.Content;
                        contentBorder.Child = contentTextBlock;
                        if (dialogue.DialogueType == DialogueType.Middle)
                            contentTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(252, 150, 95));
                        break;
                    case ContentType.Image:
                        contentImage = (Image)DeepCopy((Image)Application.Current.Resources["Element_ContentImage"]);
                        contentImage.Source = dialogue.ImageContent.Image;
                        contentBorder.Child = contentImage;
                        break;
                    case ContentType.Markdown:
                        contentMarkdownTextBlock = (MarkdownScrollViewer)DeepCopy((MarkdownScrollViewer)Application.Current.Resources["Element_ContentMarkdown"]);
                        contentMarkdownTextBlock.Markdown = dialogue.Content;
                        contentBorder.Child = contentMarkdownTextBlock;
                        if (dialogue.DialogueType == DialogueType.Middle)
                            contentMarkdownTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(252, 150, 95));
                        break;
                    case ContentType.RichText:
                        contentRichTextBox = (RichTextBox)DeepCopy((RichTextBox)Application.Current.Resources["Element_ContentRichText"]);
                        contentRichTextBox.Document = (FlowDocument)XamlReader.Parse(dialogue.Content);
                        contentBorder.Child = contentRichTextBox;
                        if (dialogue.DialogueType == DialogueType.Middle)
                            contentRichTextBox.Foreground = new SolidColorBrush(Color.FromRgb(252, 150, 95));
                        break;
                    default:
                        return null!;
                }
                return element;
            }
            element = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Custom"]);
            StackPanel panel = (StackPanel)((Border)element.Children[0]).Child;
            Stack<StackPanel> panelStack = new();
            panelStack.Push(panel);
            if (dialogue.Name is not null || dialogue.Avatar is not null)
            {
                Grid titleGrid = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Custom_Title"]);
                titleTextBlock = (TextBlock)titleGrid.Children[0];
                titleTextBlock.Text = dialogue.CustomTypeID + "-档案";
                panelStack.Peek().Children.Add(titleGrid);
                Grid profileGrid = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Custom_Profile"]);
                avatarImage = (Image)((Grid)element.Children[0]).Children[0];
                nameTextBlock = (TextBlock)((Grid)element.Children[0]).Children[1];
                avatarImage.Source = dialogue.Avatar?.Image;
                nameTextBlock.Text = dialogue.Name ?? "暂无名称";
                panelStack.Peek().Children.Add(profileGrid);
            }
            if (dialogue.Content is not null)
            {
                Grid titleGrid = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Custom_Title"]);
                titleTextBlock = (TextBlock)titleGrid.Children[0];
                titleTextBlock.Text = dialogue.CustomTypeID + "-自定义内容";
                panelStack.Peek().Children.Add(titleGrid);
                byte[] buffer = Encoding.UTF8.GetBytes(dialogue.Content);
                using MemoryStream stream = new();
                stream.Seek(0, SeekOrigin.Begin);
                stream.Write(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);
                using var streamReader = new StreamReader(stream);
                using var jsonReader = new JsonTextReader(streamReader);
                string currentProperty = "";
                bool isObject = false;
                bool isList = false;
                Grid indentGrid;
                Grid keyValueGrid;
                while (jsonReader.Read())
                {
                    switch (jsonReader.TokenType)
                    {
                        case JsonToken.None:
                            break;
                        case JsonToken.PropertyName:
                            //"property":
                            currentProperty = jsonReader.Value.ToString()!;
                            if (isObject)
                                currentProperty = PrefixObject + currentProperty;
                            if (isList)
                                currentProperty = PrefixList + currentProperty;
                            break;
                        case JsonToken.String:
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.Boolean:
                        case JsonToken.Date:
                        case JsonToken.Null:
                            //"value"
                            //123
                            //3.14
                            //true
                            //"2023-01-01T00:00:00Z"
                            //null
                            currentProperty += SuffixProperty + jsonReader.Value.ToString();
                            keyValueGrid = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Custom_KeyValue"]);
                            contentTextBlock = (TextBlock)keyValueGrid.Children[0];
                            panelStack.Peek().Children.Add(contentTextBlock);
                            break;
                        case JsonToken.StartObject:
                            //{
                            isObject = true;
                            indentGrid = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Custom_Indent"]);
                            panelStack.Push((StackPanel)indentGrid.Children[0]);
                            break;
                        case JsonToken.StartArray:
                            //[
                            isList = true;
                            indentGrid = (Grid)DeepCopy((Grid)Application.Current.Resources["Element_Custom_Indent"]);
                            panelStack.Push((StackPanel)indentGrid.Children[0]);
                            break;
                        case JsonToken.EndObject:
                            //}
                            isObject = false;
                            panelStack.Pop();
                            break;
                        case JsonToken.EndArray:
                            //]
                            isList = false;
                            panelStack.Pop();
                            break;
                        default:
                            continue;
                    }
                }
            }
            if (RemoveMargin)
                ((Border)element.Children[0]).Margin = new();
            return element;
        }

        public UIElement DeepCopy(UIElement element)
        {
            string xaml = XamlWriter.Save(element);
            return (UIElement)XamlReader.Parse(xaml);
        }
    }
}
