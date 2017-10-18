using LaserwarTest.Core.Networking.Social.VK;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LaserwarTest.Pages.VK
{
    public sealed partial class VKAuthorizationPage : VKPopupPage
    {
        /// <summary>
        /// Происходит при успешной авторизации
        /// </summary>
        public event EventHandler AuthorizationCompleted;

        public override string Title => "Авторизация";

        public VKAuthorizationPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await SendLoading();

            WebView.NavigationStarting += (s, args) =>  SendLoading(0);
            WebView.NavigationCompleted += (s, args) => SendLoaded();

            VKApi vkApi = new VKApi();
            vkApi.AuthorizationCompleted += (s, args) => { SendLoaded(); AuthorizationCompleted?.Invoke(this, args); };
            vkApi.AuthorizationFailed += (s, args) => { SendLoaded(); SendError(args); };

            vkApi.Authorize(WebView, 
                VKUserPermissions.Groups | 
                VKUserPermissions.Docs | 
                VKUserPermissions.Wall);
        }
    }
}
