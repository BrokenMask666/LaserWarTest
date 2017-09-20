using LaserwarTest.Commons.Observables;
using LaserwarTest.UI.Commands.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LaserwarTest.UI.Layouts
{
    public sealed class BackButton : ObservableObject
    {
        private BackCommand _backCommand;
        private Visibility _visibility = Visibility.Collapsed;

        public BackCommand BackCommand
        {
            private set { SetProperty(ref _backCommand, value); }
            get { return _backCommand; }
        }

        public Visibility Visibility
        {
            private set { SetProperty(ref _visibility, value); }
            get { return _visibility; }
        }

        public void SetFrame(Frame frame)
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
