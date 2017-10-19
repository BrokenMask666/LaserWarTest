using LaserwarTest.Core.Networking.Social.VK;
using LaserwarTest.Core.Networking.Social.VK.Groups;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LaserwarTest.Pages.VK
{
    public sealed partial class VKPublishToGroupSelectorPage : VKPopupPage
    {
        public override string Title => "Опубликовать в группе";

        VKPublishGameInfoToGroupNavigationParameters VKPublishGameInfoToGroupNavigationParameters { set; get; }

        public VKPublishToGroupSelectorPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is VKPublishGameInfoToGroupNavigationParameters)
            {
                VKPublishGameInfoToGroupNavigationParameters = e.Parameter as VKPublishGameInfoToGroupNavigationParameters;
            }

            await SendLoading();

            VKApi vkApi = new VKApi();
            try
            {
                var response = await vkApi.Groups.Get();
                var items = response.Response.Groups.Where(x => x.CanPost);
                if (items.Count() == 0)
                    VisualStateManager.GoToState(this, nameof(NoDataState), false);
                else
                    Groups.ItemsSource = items;
            }
            catch (VKApiException ex)
            {
                SendError(new VKError("Ошибка", ex.Message));
            }

            SendLoaded();
        }

        private void Groups_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if (e.ClickedItem is VKGroupInfo vkGroup)
            {
                if (VKPublishGameInfoToGroupNavigationParameters != null)
                {
                    VKPublishGameInfoToGroupNavigationParameters.VKGroupID = vkGroup.ID;
                    Frame.Navigate(typeof(VKPublishGameInfoToGroupPage), VKPublishGameInfoToGroupNavigationParameters);
                }
            }

            
        }
    }
}
