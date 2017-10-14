using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LaserwarTest.UI.Controls.Extensions
{
    public class TextBoxNumericValidator
    {
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

        #endregion CorrectText

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

        #region InputType (and validation)

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

        private static void OnTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            Debug.WriteLine("OnTextChanging");
            SetIgnoreSelectionChanged(sender, true);
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Debug.WriteLine("OnTextChanged");
            var obj = sender as TextBox;

            string text = obj.Text;
            string parsingText = text;
            string correctText = GetCorrectText(obj);

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

            NumericInputType inputType = GetInputType(obj);
            if (string.IsNullOrWhiteSpace(text))
            {
                switch (inputType)
                {
                    case NumericInputType.Integer:
                        parsingText = default(int).ToString();
                        break;

                    case NumericInputType.Double:
                        parsingText = default(double).ToString();
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

            CultureInfo currentCulture = CultureInfo.CurrentUICulture;

            bool isCorrect = true;
            switch (inputType)
            {
                case NumericInputType.Double:

                    if (parsingText.IndexOfAny(new[] { ',', '.' }) != -1)
                    {
                        var numberDecimalSeparator = currentCulture.NumberFormat.NumberDecimalSeparator;

                        if (numberDecimalSeparator == ",")
                            parsingText = parsingText.Replace(".", ",");
                        else if (numberDecimalSeparator == ".")
                            parsingText = parsingText.Replace(",", ".");
                    }

                    isCorrect = double.TryParse(parsingText, NumberStyles.AllowDecimalPoint, currentCulture, out double dblResult);
                    break;

                case NumericInputType.Integer:
                    isCorrect = int.TryParse(parsingText, NumberStyles.None, currentCulture, out int intResult);
                    break;

                default:
                    break;
            }

            if (isCorrect)
            {
                text = $"{parsingText}{((hasIgnoredSymbol) ? ignoredSymbol : "")}";
                SetCorrectText(obj, text);
            }
            else
            {
                SelectionChangedParams selectionParams = GetLastSelectionChangedParams(obj);
                bool oldTextEmpty = string.IsNullOrWhiteSpace(correctText);

                obj.SelectionChanged -= OnSelectionChanged;
                obj.TextChanged -= OnTextChanged;

                obj.Text = (oldTextEmpty) ? "" : correctText;
                obj.SelectionStart = selectionParams.Start;
                obj.SelectionLength = selectionParams.Length;

                obj.TextChanged += OnTextChanged;
                obj.SelectionChanged += OnSelectionChanged;
            }

            SetLastSelectionChangedParams(obj, new SelectionChangedParams(obj.SelectionStart, obj.SelectionLength));
            SetIgnoreSelectionChanged(obj, false);
        }

        private static void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnSelectionChanged");
            var obj = sender as TextBox;

            if (GetIgnoreSelectionChanged(obj)) return;

            SetLastSelectionChangedParams(obj, new SelectionChangedParams(obj.SelectionStart, obj.SelectionLength));
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

            if (GetInputType(obj) == NumericInputType.Double)
            {
                CultureInfo currentCulture = CultureInfo.CurrentUICulture;
                SetCorrectText(obj, double
                    .Parse(GetCorrectText(obj), NumberStyles.AllowDecimalPoint, currentCulture)
                    .ToString("G", currentCulture));
            }

            obj.Text = GetCorrectText(obj);
        }

        public static NumericInputType GetInputType(TextBox element)
        {
            return (NumericInputType)element.GetValue(InputTypeProperty);
        }

        public static void SetInputType(TextBox element, NumericInputType value)
        {
            element.SetValue(InputTypeProperty, value);
        }

        #endregion InputType (and validation)
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
