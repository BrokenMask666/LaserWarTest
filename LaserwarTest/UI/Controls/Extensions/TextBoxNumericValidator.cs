using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LaserwarTest.UI.Controls.Extensions
{
    public class TextBoxNumericValidator
    {
        /// <summary>
        /// Класс для хранения служебных свойств
        /// </summary>
        public class Services
        {
            #region LastSelectionChangedParams

            public static readonly DependencyProperty LastSelectionChangedParamsProperty =
                DependencyProperty.RegisterAttached(
                    "LastSelectionChangedParams",
                    typeof(SelectionChangedParams),
                    typeof(TextBoxNumericValidator),
                    new PropertyMetadata(new SelectionChangedParams(0,0)));

            public static SelectionChangedParams GetLastSelectionChangedParams(TextBox element)
            {
                return (SelectionChangedParams)element.GetValue(LastSelectionChangedParamsProperty);
            }

            public static void SetLastSelectionChangedParams(TextBox element, SelectionChangedParams value)
            {
                element.SetValue(LastSelectionChangedParamsProperty, value);
            }


            #endregion LastSelectionChangedParams

            #region IgnoreSelectionChanged

            public static readonly DependencyProperty IgnoreSelectionChangedProperty =
                DependencyProperty.RegisterAttached(
                    "IgnoreSelectionChanged",
                    typeof(bool),
                    typeof(TextBoxNumericValidator),
                    new PropertyMetadata(false));

            public static bool GetIgnoreSelectionChanged(TextBox element)
            {
                return (bool)element.GetValue(IgnoreSelectionChangedProperty);
            }

            public static void SetIgnoreSelectionChanged(TextBox element, bool value)
            {
                element.SetValue(IgnoreSelectionChangedProperty, value);
            }

            #endregion IgnoreSelectionChanged
        }

        #region CorrectText

        public static readonly DependencyProperty CorrectTextProperty =
            DependencyProperty.RegisterAttached(
                "CorrectText",
                typeof(string),
                typeof(TextBoxNumericValidator),
                new PropertyMetadata(null));

        public static string GetCorrectText(TextBox element)
        {
            return (string)element.GetValue(CorrectTextProperty);
        }

        public static void SetCorrectText(TextBox element, string value)
        {
            element.SetValue(CorrectTextProperty, value);
        }

        #endregion CorrectText

        #region MinValue

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.RegisterAttached(
                "MinValue",
                typeof(string),
                typeof(TextBoxNumericValidator),
                new PropertyMetadata(null));

        public static string GetMinValue(TextBox element)
        {
            return (string)element.GetValue(MinValueProperty);
        }

        public static void SetMinValue(TextBox element, string value)
        {
            element.SetValue(MinValueProperty, value);
        }

        #endregion MinValue

        #region MaxValue

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.RegisterAttached(
                "MaxValue",
                typeof(string),
                typeof(TextBoxNumericValidator),
                new PropertyMetadata(null));

        public static string GetMaxValue(TextBox element)
        {
            return (string)element.GetValue(MaxValueProperty);
        }

        public static void SetMaxValue(TextBox element, string value)
        {
            element.SetValue(MaxValueProperty, value);
        }

        #endregion MaxValue

        #region IgnoredSymbol

        public static readonly DependencyProperty IgnoredSymbolProperty =
            DependencyProperty.RegisterAttached(
                "IgnoredSymbol",
                typeof(string),
                typeof(TextBoxNumericValidator),
                new PropertyMetadata(null));

        public static string GetIgnoredSymbol(TextBox element)
        {
            return (string)element.GetValue(IgnoredSymbolProperty);
        }

        public static void SetIgnoredSymbol(TextBox element, string value)
        {
            element.SetValue(IgnoredSymbolProperty, value);
        }

        #endregion IgnoredSymbol

        #region InputType

        public static readonly DependencyProperty InputTypeProperty =
            DependencyProperty.RegisterAttached(
                "InputType",
                typeof(NumericInputType),
                typeof(TextBoxNumericValidator),
                new PropertyMetadata(NumericInputType.None, OnInputTypeChanged));

        private static void OnInputTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as TextBox;
            if (obj == null) return;

            NumericInputType oldValue = (NumericInputType)e.OldValue;
            if (oldValue == NumericInputType.None)
            {
                obj.TextChanging += OnTextChanging;
                obj.TextChanged += OnTextChanged;
                obj.SelectionChanged += OnSelectionChanged;
                obj.GotFocus += OnGotFocus;
                obj.LostFocus += OnLostFocus;
            }

            NumericInputType newValue = (NumericInputType)e.NewValue;
            if (newValue == NumericInputType.None)
            {
                obj.TextChanging -= OnTextChanging;
                obj.TextChanged -= OnTextChanged;
                obj.SelectionChanged -= OnSelectionChanged;
                obj.GotFocus -= OnGotFocus;
                obj.LostFocus -= OnLostFocus;
            }
        }

        public static NumericInputType GetInputType(TextBox element)
        {
            return (NumericInputType)element.GetValue(InputTypeProperty);
        }

        public static void SetInputType(TextBox element, NumericInputType value)
        {
            element.SetValue(InputTypeProperty, value);
        }

        #endregion InputType

        #region Validation

        private static void OnTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            Services.SetIgnoreSelectionChanged(sender, true);
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var obj = sender as TextBox;

            string text = obj.Text;
            string parsingText = text;
            string correctText = GetCorrectText(obj);

            int? minInt = null;
            int? maxInt = null;
            double? minDbl = null;
            double? maxDbl = null;

            NumericInputType inputType = GetInputType(obj);
            CultureInfo currentCulture = CultureInfo.CurrentUICulture;

            string ignoredSymbol = GetIgnoredSymbol(obj);
            bool hasIgnoredSymbol = false;
            if (!string.IsNullOrWhiteSpace(ignoredSymbol))
            {
                int indx = text.LastIndexOf(ignoredSymbol);
                if (indx != -1)
                {
                    hasIgnoredSymbol = true;
                    parsingText = text.Substring(0, indx);
                }
                else
                {
                    indx = correctText.LastIndexOf(ignoredSymbol);
                    hasIgnoredSymbol = (indx != -1);
                }
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                switch (inputType)
                {
                    case NumericInputType.Integer:
                        minInt = ParseInt(GetMinValue(obj), NumberStyles.None, currentCulture);

                        parsingText = (minInt ?? default(int)).ToString();
                        break;

                    case NumericInputType.Double:
                        minDbl = double.Parse(GetMinValue(obj), NumberStyles.AllowDecimalPoint, currentCulture);

                        parsingText = (minDbl ?? default(double)).ToString();
                        break;

                    default:
                        hasIgnoredSymbol = false;
                        parsingText = correctText;
                        break;
                }

                text = $"{parsingText}{((hasIgnoredSymbol) ? ignoredSymbol : "")}";

                obj.Text = text;
                obj.SelectAll();
                return;
            }

            if (text == correctText) return;

            bool isCorrect = true;
            bool valueChanged = false;
            switch (inputType)
            {
                case NumericInputType.Integer:
                    isCorrect = int.TryParse(parsingText, NumberStyles.None, currentCulture, out int intResult);

                    minInt = ParseInt(GetMinValue(obj), NumberStyles.None, currentCulture);
                    maxInt = ParseInt(GetMaxValue(obj), NumberStyles.None, currentCulture);

                    if (minInt != null && intResult < minInt)
                    {
                        intResult = minInt.Value;
                        valueChanged = true;
                    }

                    if (maxInt != null && intResult > maxInt)
                    {
                        intResult = maxInt.Value;
                        valueChanged = true;
                    }

                    if (valueChanged)
                    {
                        parsingText = intResult.ToString();
                    }

                    break;

                case NumericInputType.Double:
                    parsingText = ValidateDecimalPoint(parsingText, currentCulture);
                    isCorrect = double.TryParse(parsingText, NumberStyles.AllowDecimalPoint, currentCulture, out double dblResult);

                    minDbl = double.Parse(GetMinValue(obj), NumberStyles.AllowDecimalPoint, currentCulture);
                    maxDbl = double.Parse(GetMaxValue(obj), NumberStyles.AllowDecimalPoint, currentCulture);

                    if (minDbl != null && dblResult < minDbl)
                    {
                        dblResult = minDbl.Value;
                        valueChanged = true;
                    }

                    if (maxDbl != null && dblResult > maxDbl)
                    {
                        dblResult = maxDbl.Value;
                        valueChanged = true;
                    }

                    if (valueChanged)
                    {
                        parsingText = dblResult.ToString();
                    }

                    break;

                default:
                    break;
            }

            if (isCorrect)
            {
                text = $"{parsingText}{((hasIgnoredSymbol) ? ignoredSymbol : "")}";
                SetCorrectText(obj, text);

                if (valueChanged)
                {
                    obj.SelectionChanged -= OnSelectionChanged;
                    obj.TextChanged -= OnTextChanged;

                    obj.Text = text;
                    obj.SelectionStart = parsingText.Length;

                    obj.SelectionChanged += OnSelectionChanged;
                    obj.TextChanged += OnTextChanged;
                }
            }
            else
            {
                SelectionChangedParams selectionParams = Services.GetLastSelectionChangedParams(obj);
                bool oldTextEmpty = string.IsNullOrWhiteSpace(correctText);

                obj.SelectionChanged -= OnSelectionChanged;
                obj.TextChanged -= OnTextChanged;

                obj.Text = (oldTextEmpty) ? "" : correctText;
                obj.SelectionStart = selectionParams.Start;
                obj.SelectionLength = selectionParams.Length;

                obj.TextChanged += OnTextChanged;
                obj.SelectionChanged += OnSelectionChanged;
            }

            Services.SetLastSelectionChangedParams(obj, new SelectionChangedParams(obj.SelectionStart, obj.SelectionLength));
            Services.SetIgnoreSelectionChanged(obj, false);
        }

        public static string ValidateDecimalPoint(string text, CultureInfo culture)
        {
            string ret = "";
            if (text.IndexOfAny(new[] { ',', '.' }) != -1)
            {
                var numberDecimalSeparator = culture.NumberFormat.NumberDecimalSeparator;

                if (numberDecimalSeparator == ",")
                    ret = text.Replace(".", ",");
                else if (numberDecimalSeparator == ".")
                    ret = text.Replace(",", ".");
            }

            return ret;
        }

        private static int? ParseInt(string text, NumberStyles styles, CultureInfo culture)
        {
            int? ret = null;
            if (!string.IsNullOrWhiteSpace(text))
                ret = int.Parse(text, styles, culture);

            return ret;
        }

        private static double? ParseDouble(string text, NumberStyles styles, CultureInfo culture)
        {
            double? ret = null;

            text = ValidateDecimalPoint(text, culture);
            if (!string.IsNullOrWhiteSpace(text))
                ret = double.Parse(text, styles, culture);

            return ret;
        }

        private static void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var obj = sender as TextBox;

            if (Services.GetIgnoreSelectionChanged(obj)) return;

            Services.SetLastSelectionChangedParams(obj, new SelectionChangedParams(obj.SelectionStart, obj.SelectionLength));
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var obj = sender as TextBox;
            string correctText = GetCorrectText(obj);

            if (correctText == null) SetCorrectText(obj, obj.Text);
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var obj = sender as TextBox;

            CultureInfo currentCulture = CultureInfo.CurrentUICulture;
            NumericInputType inputType = GetInputType(obj);
            if (inputType == NumericInputType.Double)
            {
                double? val = ParseDouble(GetCorrectText(obj), NumberStyles.AllowDecimalPoint, currentCulture);
                if (val != null) SetCorrectText(obj, val.Value.ToString("G", currentCulture));
            }

            obj.Text = GetCorrectText(obj);
        }
        
        #endregion Validation
    }

    /// <summary>
    /// Перечисление доступных числовых типов ввода в текстовом поле
    /// </summary>
    public enum NumericInputType
    {
        /// <summary>
        /// Тип не установлен
        /// </summary>
        None = 0,
        /// <summary>
        /// Целое число
        /// </summary>
        Integer,
        /// <summary>
        /// Значение с плавающей точкой
        /// </summary>
        Double
    }

    public struct SelectionChangedParams
    {
        public int Start { get; }
        public int Length { get; }

        public SelectionChangedParams(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
}
