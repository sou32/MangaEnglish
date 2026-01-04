using System.Globalization;

namespace MangaEnglish.Converters;

public class Percent90Converter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double w) return w * 0.9;
        return 300;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}