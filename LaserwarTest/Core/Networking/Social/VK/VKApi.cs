using LaserwarTest.Core.Networking.Social.VK.Groups;
using LaserwarTest.Management.Settings;
using LaserwarTest.Management.SettingsStorage.Storages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace LaserwarTest.Core.Networking.Social.VK
{
    /// <summary>
    /// Предоставляет основную информацию для работы API ВКонтакте
    /// </summary>
    public sealed class VKApiInfo
    {
        SettingsStorage _settings = new SettingsStorage();

        VKSettings VKSettings => _settings.VK;

        public long AppID { get; } = 6223975L;
        public string APIVersion { get; } = "5.68";

        public string UserID => VKSettings.UserID;

        /// <summary>
        /// Возвращает токен доступа, если не истекло его время
        /// </summary>
        public string AccessToken
        {
            get
            {
                string value = VKSettings.AT;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    DateTime expirationTime = VKSettings.ExpirationTime;
                    if (expirationTime < DateTime.Now.AddMinutes(-30))
                    {
                        VKSettings.AT = null;
                        return null;
                    }
                }

                return value;
            }
        }
    }

    public class VKApiBase
    {
        protected VKApiInfo ApiInfo { get; } = new VKApiInfo();
    }

    /// <summary>
    /// Предоставляет доступ к API социальной сети ВКонтакте
    /// </summary>
    public sealed class VKApi : VKApiBase
    {
        const string REDIRECT_URI = "https://oauth.vk.com/blank.html";

        /// <summary>
        /// Происходит при успешной авторизации
        /// </summary>
        public event EventHandler AuthorizationCompleted;
        /// <summary>
        /// Происходит, если во время авторизации произошли ошибки
        /// </summary>
        public event EventHandler<VKAuthorizationError> AuthorizationFailed;

        Lazy<VKGroupsApi> _groups = new Lazy<VKGroupsApi>(() => new VKGroupsApi());

        public VKGroupsApi Groups => _groups.Value;


        public void Authorize(WebView web, VKUserPermissions userPermissions)
        {
            web.NavigationCompleted += Web_NavigationCompleted;
            web.NavigationFailed += Web_NavigationFailed;

            web.Navigate(new Uri($"https://oauth.vk.com/authorize?" +
                $"client_id={ApiInfo.AppID}" +
                $"&display=page" +
                $"&redirect_uri={REDIRECT_URI}" +
                $"&scope={userPermissions.ToString().ToLowerInvariant()}" +
                $"&response_type=token" +
                $"&v={ApiInfo.APIVersion}"));
        }

        private void Web_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            Debug.WriteLine(nameof(Web_NavigationFailed));

            RaiseAuthorizationFailed(e.WebErrorStatus.ToString());
        }

        private void Web_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs e)
        {
            Debug.WriteLine($"{nameof(Web_NavigationCompleted)}");

            if (!e.IsSuccess)
            {
                RaiseAuthorizationFailed(e.WebErrorStatus.ToString());
                return;
            }

            string uri = e.Uri.OriginalString;
            if (!uri.StartsWith(REDIRECT_URI)) return;

            string response = uri.Substring(uri.IndexOf('#') + 1);

            string[] responseParamsPairs = response.Split(new char[] { '&' });
            Dictionary<string, string> responseParams = responseParamsPairs.ToDictionary(
                keySelector: (x) => x.Substring(0, x.IndexOf('=')),
                elementSelector: (x) => x.Substring(x.IndexOf('=') + 1));

            if (responseParams.ContainsKey("error"))
            {
                RaiseAuthorizationFailed(responseParams["error"], responseParams["error_description"]);
            }
            else if (responseParams.ContainsKey("access_token"))
            {
                VKSettings VKSettings = new SettingsStorage().VK;

                VKSettings.UserID = responseParams["user_id"];
                VKSettings.ExpirationTime = DateTime.Now.AddSeconds(int.Parse(responseParams["expires_in"]));
                VKSettings.AT = responseParams["access_token"];
                
                AuthorizationCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }

            RaiseAuthorizationFailed("error_unknown");
        }

        void RaiseAuthorizationFailed(string error, string description = "")
        {
            AuthorizationFailed?.Invoke(this, new VKAuthorizationError(error, description));
        }
    }

    /// <summary>
    /// Набор, представляющий права доступа пользователя
    /// </summary>
    [Flags]
    public enum VKUserPermissions
    {
        Friens = 0,
        Groups = 1 << 0,
        Docs = 1 << 1,
        Wall = 1 << 2,
        Friends = 1 << 3
    }
}
