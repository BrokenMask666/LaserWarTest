using System;
using LaserwarTest.Core.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LaserwarTest.UI.Dialogs
{
    public sealed partial class ErrorDialog : UserControl, IPopupUIElement
    {
        public event EventHandler CanClose;

        public ErrorDialog()
        {
            InitializeComponent();
        }

        #region DependencyProperty

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(ErrorDialog),
                new PropertyMetadata(""));

        public string Title
        {
            set { SetValue(TitleProperty, value); }
            get { return (string)GetValue(TitleProperty); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                nameof(Message),
                typeof(string),
                typeof(ErrorDialog),
                new PropertyMetadata(""));

        public string Message
        {
            set { SetValue(MessageProperty, value); }
            get { return (string)GetValue(MessageProperty); }
        }



        #endregion DependencyProperty

        private void CloseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CanClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
