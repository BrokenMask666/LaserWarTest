using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LaserwarTest.Commons.UI.Xaml.Converters
{
    /// <summary>
    /// Преобразует текстовое значение перечисления в значение видимости.
    /// Возвращает <see cref="Visibility.Visible"/>, если значение перечисления соответствует заданному параметру (иначе <see cref="Visibility.Collapsed"/>).
    /// Работает только в одну сторону
    /// </summary>
    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value.GetType().GetTypeInfo().IsEnum && targetType == typeof(Visibility) && parameter is string enumStr)
                return (value.ToString().Equals(enumStr)) ? Visibility.Visible : Visibility.Collapsed;

            throw new InvalidOperationException("EnumToBoolConverter -> target type is not of type bool OR value is not of enum type OR invalid parameter");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
