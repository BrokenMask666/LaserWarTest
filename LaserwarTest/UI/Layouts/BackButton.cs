using LaserwarTest.Commons.Helpers;
using LaserwarTest.UI.Commands.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace LaserwarTest.UI.Layouts
{
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    public sealed class BackButton : Control
    {
        Button _button;

        BackCommand BackCommand { set; get; }

        public BackButton()
        {
            DefaultStyleKey = typeof(BackButton);
            Visibility = Visibility.Collapsed;

            Loaded += (s, e) =>
            {
                Frame frame = VisualTreeExplorer.FindParent<Frame>(this);
                if (frame != null) SetFrame(frame);
            };
        }

        protected override void OnApplyTemplate()
        {
            _button = (Button)GetTemplateChild("PART_Button");
            _button.Command = BackCommand;

            base.OnApplyTemplate();
        }

        private void SetFrame(Frame frame)
        {
            if (BackCommand != null)
                BackCommand.CanExecuteChanged -= OnCanExecuteChanged;

            BackCommand = new BackCommand(frame);
            BackCommand.CanExecuteChanged += OnCanExecuteChanged;

            BackCommand.RaiseCanExecuteChanged();
        }

        private void OnCanExecuteChanged(object sender, System.EventArgs e)
        {
            Visibility = ((BackCommand.CanExecute(null)) ? Visibility.Visible : Visibility.Collapsed);
        }


    }
}
