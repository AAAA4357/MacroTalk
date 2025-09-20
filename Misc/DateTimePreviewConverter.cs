using System.Globalization;
using System.Windows.Data;

namespace MacroTalk
{
    internal class DateTimePreviewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime now = DateTime.Now;
            DateTime latest = (DateTime)value;
            TimeSpan delta = latest - now;
            if (delta.TotalMinutes < 1)
            {
                return "刚刚";
            }
            else if (delta.TotalDays < 1)
            {
                return " " + (int)delta.TotalMinutes + "分钟 之前";
            }
            else if (delta.TotalDays < 7)
            {
                return "一周内";
            }
            else
            {
                if (latest.Year == now.Year)
                {
                    if (latest.Month == now.Month)
                    {
                        return "本月内";
                    }
                    else
                    {
                        return "今年内";
                    }
                }
                else
                {
                    return "很久以前";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }
    }
}
