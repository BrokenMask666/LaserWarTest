using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LaserwarTest.UI.Controls.Extensions
{
    public class TextBoxExtensions
    {
        public static readonly DependencyProperty SelectTextOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectTextOnFocus",
                typeof(bool),
                typeof(TextBoxExtensions),
                new PropertyMetadata(false, OnSelectTextOnFocusChanged));

        private static void OnSelectTextOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as TextBox;
            if (obj == null) return;

            bool value = (bool)e.NewValue;
            if (value)
            {
                obj.GotFocus += OnGotFocus;
            }
            else
            {
                obj.GotFocus -= OnGotFocus;
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var obj = sender as TextBox;
            obj.SelectAll();
        }

        public static bool GetSelectTextOnFocus(TextBox element)
        {
            return (bool)element.GetValue(SelectTextOnFocusProperty);
        }

        public static void SetSelectTextOnFocus(TextBox element, bool value)
        {
            element.SetValue(SelectTextOnFocusProperty, value);
        }
    }
}
