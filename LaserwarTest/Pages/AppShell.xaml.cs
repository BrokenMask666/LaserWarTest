using LaserwarTest.Core.UI.Popups;
using LaserwarTest.UI.SideMenu;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LaserwarTest.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class AppShell : Page
    {
        static AppShell _shell;
        public static AppShell GetCurrent() => _shell ?? throw new Exception("Application shell hasn't been initialized");

        uint _openedPopups;
        uint _loadRequests;

        AppMenu Menu { get; }

        public AppShell()
        {
            if (_shell != null) throw new Exception("Application shell has already been initialized");

            InitializeComponent();
            _shell = this;

            Menu = new AppMenu(InnerFrame);

            PopupContent.PopupOpening += OnPopupOpening;
            PopupContent.PopupClosed += OnPopupClosed;
        }

        private void ToState(string state) => VisualStateManager.GoToState(this, state, false);

        private void OnPopupOpening(object sender, EventArgs e)
        {
            _openedPopups++;
            if (_openedPopups == 1) ToState(ShowDialogState.Name);
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            _openedPopups--;
            if (_openedPopups == 0) ToState(NoDialogsState.Name);
        }

        public void SetLoading()
        {
            _loadRequests++;
            if (_loadRequests == 1) ToState(LoadingState.Name);
        }

        public void SetLoaded()
        {
            if (_loadRequests == 0) return;

            _loadRequests--;
            if (_loadRequests == 0) ToState(LoadedState.Name);
        }
    }

    public class AppMenuItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is AppMenuItem menuItem && targetType == typeof(AppMenuItem))
                return menuItem;

            return value;
        }
    }
}
