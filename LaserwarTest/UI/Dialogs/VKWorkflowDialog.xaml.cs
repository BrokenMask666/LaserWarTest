using LaserwarTest.Core.Networking.Social.VK;
using LaserwarTest.Core.UI.Popups;
using LaserwarTest.Pages.VK;
using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LaserwarTest.UI.Dialogs
{
    /// <summary>
    /// Диалоговое окно, предназначенное для навигации по страницам,
    /// служащим для отображения содержимого из социальной сети ВКонтакте
    /// </summary>
    public sealed partial class VKWorkflowDialog : UserControl, IPopupUIElement
    {
        public event EventHandler CanClose;

        Type RedirectPage { get; }
        object RedirectPageParameter { get; }

        VKError LastError { set; get; }
        object LastParameter { set; get; }

        /// <summary>
        /// Создает новое окно, которое ведет на страницу страницу авторизации ВКонтакте,
        /// а затем перенавигирует на указанную страницу
        /// </summary>
        /// <param name="redirectPageType">Страница, открывающаяся после успешной авторизации</param>
        /// <param name="redirectPageParameter">Параметры навигации для этой редирект-страницы</param>
        public VKWorkflowDialog(Type redirectPageType, object redirectPageParameter)
        {
            InitializeComponent();

            if (!redirectPageType.GetTypeInfo().IsSubclassOf(typeof(VKPopupPage)))
                throw new ArgumentException($"Page must be a subclass of {nameof(VKPopupPage)}");

            RedirectPage = redirectPageType;
            RedirectPageParameter = redirectPageParameter;

            Frame.Navigating += Frame_Navigating;
            Frame.Navigated += Frame_Navigated;

            VKApiInfo vkApiInfo = new VKApiInfo();
            if (vkApiInfo.AccessToken == null)
                Frame.Navigate(typeof(VKAuthorizationPage), null, new SuppressNavigationTransitionInfo());
            else
                Frame.Navigate(RedirectPage, RedirectPageParameter);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is VKPopupPage vkPage)
            {
                LastParameter = e.Parameter;

                Layout.Title = vkPage.Title;

                vkPage.LoadingSent += OnLoadingSent;
                vkPage.LoadedSent += OnLoadedSent;
                vkPage.ErrorOccured += OnErrorOccured;

                if (vkPage is VKAuthorizationPage authorizationPage)
                    authorizationPage.AuthorizationCompleted += OnAuthorizationCompleted;
            }
        }

        private void OnAuthorizationCompleted(object sender, EventArgs e)
        {
            Frame.Navigate(RedirectPage, RedirectPageParameter);
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if ((sender as Frame).Content is VKPopupPage vkPage)
            {
                vkPage.LoadingSent -= OnLoadingSent;
                vkPage.LoadedSent -= OnLoadedSent;

                if (vkPage is VKAuthorizationPage authorizationPage)
                    authorizationPage.AuthorizationCompleted -= OnAuthorizationCompleted;
            }
        }

        private void OnLoadingSent(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, nameof(LoadingState), false);
        }

        private void OnLoadedSent(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, nameof(LoadedState), false);
        }

        private void OnErrorOccured(object sender, VKError e)
        {
            VisualStateManager.GoToState(this, nameof(HasErrorState), false);

            LastError = e;

            Error.Text = e.Error;
            ErrorDescription.Text = e.Descriprion;
        }

        private void RetryButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (LastError == null) return;

            VisualStateManager.GoToState(this, nameof(NoErrorState), false);

            if (LastError is VKAuthorizationError)
                Frame.Navigate(typeof(VKAuthorizationPage), null, new SuppressNavigationTransitionInfo());
            else
            {
                if (LastError.Renavigate && Frame.Content is VKPopupPage)
                    Frame.Navigate(Frame.Content.GetType(), LastParameter, new SuppressNavigationTransitionInfo());
            }
        }

        private void CloseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CanClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
