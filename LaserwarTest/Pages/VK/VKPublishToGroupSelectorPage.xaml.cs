using LaserwarTest.Core.Networking.Social.VK;
using System.Linq;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LaserwarTest.Pages.VK
{
    public sealed partial class VKPublishToGroupSelectorPage : VKPopupPage
    {
        public override string Title => "Опубликовать в группе";

        public VKPublishToGroupSelectorPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await SendLoading();

            VKApi vkApi = new VKApi();
            try
            {
                var response = await vkApi.Groups.Get();
                Groups.ItemsSource = response.Response.Groups.Where(x => x.CanPost);
            }
            catch (VKApiException ex)
            {
                SendError(new VKError("Ошибка", ex.Message));
            }

            SendLoaded();
        }
    }
}
