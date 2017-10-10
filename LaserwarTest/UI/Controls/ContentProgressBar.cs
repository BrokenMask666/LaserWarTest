using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace LaserwarTest.UI.Controls
{
    [ContentProperty(Name = nameof(Content))]
    public sealed class ContentProgressBar : Control
    {
        public ContentProgressBar()
        {
            DefaultStyleKey = typeof(ContentProgressBar);
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(ContentProgressBar),
                new PropertyMetadata(null));

        public object Content
        {
            set { SetValue(ContentProperty, value); }
            get { return GetValue(ContentProperty); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(ContentProgressBar),
                ProgressBar.ValueProperty.GetMetadata(typeof(ProgressBar)));

        public double Value
        {
            set { SetValue(ValueProperty, value); }
            get { return (double)GetValue(ValueProperty); }
        }

        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register(
                nameof(IsIndeterminate),
                typeof(bool),
                typeof(ContentProgressBar),
                ProgressBar.IsIndeterminateProperty.GetMetadata(typeof(ProgressBar)));

        public bool IsIndeterminate
        {
            set { SetValue(IsIndeterminateProperty, value); }
            get { return (bool)GetValue(IsIndeterminateProperty); }
        }
    }
}
