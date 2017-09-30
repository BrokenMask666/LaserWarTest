using LaserwarTest.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LaserwarTest.UI.Layouts
{
    [ContentProperty(Name = nameof(InnerContent))]
    public sealed partial class PageLayout : UserControl
    {
        public BackButton BackButton { get; } = new BackButton();

        public PageLayout()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Frame frame = VisualTreeExplorer.FindParent<Frame>(this);
                if (frame != null) BackButton.SetFrame(frame);
            };
        }

        #region DependencyProperty

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(PageLayout),
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
                typeof(PageLayout),
                null);

        public object InnerContent
        {
            set { SetValue(InnerContentProperty, value); }
            get { return GetValue(InnerContentProperty); }
        }

        public static readonly DependencyProperty TitleRightContentProperty =
            DependencyProperty.Register(
                nameof(TitleRightContent),
                typeof(object),
                typeof(PageLayout),
                null);


        public object TitleRightContent
        {
            set { SetValue(TitleRightContentProperty, value); }
            get { return GetValue(TitleRightContentProperty); }
        }

        #endregion DependencyProperty
    }
}
