using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LaserwarTest.UI.Controls.Extensions
{
    public class ContentControlExtensions
    {
        static Dictionary<ContentControl, long> _attachedControls = new Dictionary<ContentControl, long>();

        public static readonly DependencyProperty UseUpperCaseProperty = 
            DependencyProperty.RegisterAttached(
                "UseUpperCase",
                typeof(bool),
                typeof(ContentControlExtensions),
                new PropertyMetadata(false, OnUseUpperCasePropertyChanged));

        private static void OnUseUpperCasePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ContentControl;
            if (obj == null) return;

            bool value = (bool)e.NewValue;
            if (value)
            {
                if (RegisterContentChangedCallback(obj))
                {
                    obj.Loaded += OnControlLoaded;
                    obj.Unloaded += OnControlUnloaded;
                }
            }
            else
            {
                if (UnregisterContentChangedCallback(obj))
                    obj.Unloaded -= OnControlUnloaded;
            }
        }

        private static void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            var obj = sender as ContentControl;
            obj.Content = obj.Content;

            obj.Loaded -= OnControlLoaded;
        }

        private static void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            var obj = sender as ContentControl;

            if (UnregisterContentChangedCallback(obj))
                obj.Unloaded -= OnControlUnloaded;
        }

        private static bool RegisterContentChangedCallback(ContentControl contentControl)
        {
            if (_attachedControls.ContainsKey(contentControl))
                return false;

            _attachedControls[contentControl] =
                    contentControl.RegisterPropertyChangedCallback(ContentControl.ContentProperty, OnAttachedControlContentChanged);

            return true;
        }

        private static bool UnregisterContentChangedCallback(ContentControl contentControl)
        {
            if (!_attachedControls.ContainsKey(contentControl))
                return false;

            contentControl.UnregisterPropertyChangedCallback(ContentControl.ContentProperty, _attachedControls[contentControl]);
            _attachedControls.Remove(contentControl);

            return true;
        }

        private static void OnAttachedControlContentChanged(DependencyObject sender, DependencyProperty dp)
        {
            var obj = sender as ContentControl;
            if (obj.Content is string str && str != null)
            {
                string upperStr = str.ToUpper();
                if (str == upperStr) return;

                obj.Content = upperStr;
            }
        }

        public static bool GetUseUpperCase(ContentControl element)
        {
            return (bool)element.GetValue(UseUpperCaseProperty);
        }

        public static void SetUseUpperCase(ContentControl element, bool value)
        {
            element.SetValue(UseUpperCaseProperty, value);
        }
    }
}
