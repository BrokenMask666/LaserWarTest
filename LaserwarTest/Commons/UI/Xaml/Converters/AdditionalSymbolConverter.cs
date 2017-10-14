using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace LaserwarTest.Commons.UI.Xaml.Converters
{
    public sealed class AdditionalSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string)) return value;

            string ret = (string)XamlBindingHelper.ConvertValue(targetType, value);
            if (parameter is string str && !string.IsNullOrWhiteSpace(str))
                return $"{ret}{str}";

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string strValue && parameter is string str && !string.IsNullOrWhiteSpace(str))
            {
                int indx = strValue.LastIndexOf(str);
                if (indx != -1) value = strValue.Remove(indx);
            }

            return XamlBindingHelper.ConvertValue(targetType, value);
        }
    }
}
