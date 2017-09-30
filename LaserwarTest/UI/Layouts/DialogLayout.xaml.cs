using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LaserwarTest.UI.Layouts
{
    [ContentProperty(Name = nameof(InnerContent))]
    public sealed partial class DialogLayout : UserControl
    {
        public DialogLayout()
        {
            InitializeComponent();
        }

        #region DependencyProperty

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(DialogLayout),
                new PropertyMetadata(""));

        public string Title
        {
            set { SetValue(TitleProperty, value.ToUpper()); }
            get { return (string)GetValue(TitleProperty); }
        }

        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register(
                nameof(InnerContent),
                typeof(object),
                typeof(DialogLayout),
                null);

        public object InnerContent
        {
            set { SetValue(InnerContentProperty, value); }
            get { return GetValue(InnerContentProperty); }
        }

        public static readonly DependencyProperty CommandsBarProperty =
            DependencyProperty.Register(
                nameof(CommandsBar),
                typeof(object),
                typeof(DialogLayout),
                null);


        public object CommandsBar
        {
            set { SetValue(CommandsBarProperty, value); }
            get { return GetValue(CommandsBarProperty); }
        }

        #endregion DependencyProperty
    }
}
