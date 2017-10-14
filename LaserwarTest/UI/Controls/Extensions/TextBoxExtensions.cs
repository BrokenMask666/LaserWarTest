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










        public static readonly DependencyProperty OnSelectionChangedIgnoreSymbolProperty =
            DependencyProperty.RegisterAttached(
                "OnSelectionChangedIgnoreSymbol",
                typeof(string),
                typeof(TextBoxExtensions),
                new PropertyMetadata("", OnOnSelectionChangedIgnoreSymbolChanged));

        private static void OnOnSelectionChangedIgnoreSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as TextBox;
            if (obj == null) return;

            string value = (string)e.NewValue;
            if (!string.IsNullOrWhiteSpace(value))
            {
                obj.SelectionChanged += OnSelectionChanged;
            }
            else
            {
                obj.SelectionChanged -= OnSelectionChanged;
            }
        }

        private static void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var obj = sender as TextBox;
            string symbol = GetOnSelectionChangedIgnoreSymbol(obj);

            int indx = obj.Text.LastIndexOf(symbol);
            if (indx == -1) return;

            obj.SelectionChanged -= OnSelectionChanged;

            int start = obj.SelectionStart;
            int length = obj.SelectionLength;

            if (start > indx)
            {
                obj.SelectionStart = indx;
                obj.SelectionLength = 0;
            }
            else if (start + length > indx)
            {
                obj.SelectionLength = indx - start;
            }

            obj.SelectionChanged += OnSelectionChanged;
        }

        public static string GetOnSelectionChangedIgnoreSymbol(TextBox element)
        {
            return (string)element.GetValue(OnSelectionChangedIgnoreSymbolProperty);
        }

        public static void SetOnSelectionChangedIgnoreSymbol(TextBox element, string value)
        {
            element.SetValue(OnSelectionChangedIgnoreSymbolProperty, value);
        }
    }
}
