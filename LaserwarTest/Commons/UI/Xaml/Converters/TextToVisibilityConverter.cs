using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LaserwarTest.Commons.UI.Xaml.Converters
{
    public class TextToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string str && targetType == typeof(Visibility))
                return (string.IsNullOrWhiteSpace(str)) ? Visibility.Collapsed : Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
