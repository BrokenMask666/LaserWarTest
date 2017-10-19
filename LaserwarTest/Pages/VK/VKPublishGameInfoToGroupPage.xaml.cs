using LaserwarTest.Commons.Management.Navigation;
using LaserwarTest.Core.Networking.Social.VK;
using LaserwarTest.Core.Networking.Social.VK.Wall;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LaserwarTest.Pages.VK
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class VKPublishGameInfoToGroupPage : VKPopupPage
    {
        public override string Title => "Опубликовать в группе";

        VKPublishGameInfoToGroupNavigationParameters Parameters { set; get; }

        public VKPublishGameInfoToGroupPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is VKPublishGameInfoToGroupNavigationParameters parameters)
            {
                Parameters = parameters;
            }
            else
            {
                SendError(new VKError("Ошибка", "Параметры заданы неверно"));
            }
        }

        async Task Publish()
        {
            await SendLoading();

            VKApi vkApi = new VKApi();
            try
            {
                //var wallUploadServerResponse = (await vkApi.Photos.GetWallUploadServer(Parameters.VKGroupID)).Response;
                //Debug.WriteLine($"GetUploadServer ->\n\turl: {wallUploadServerResponse.UploadUrl},\n\tuser_id: {wallUploadServerResponse.UserId},\n\talbum_id: {wallUploadServerResponse.AlbumID}");

                //var uploadWallPhotoResponse = await vkApi.Photos.UploadWallPhoto(
                //    await ApplicationData.Current.LocalFolder.GetFileAsync(Parameters.ScreenshotFileName),
                //    wallUploadServerResponse.UploadUrl);

                //var savedPhoto = await vkApi.Photos.SaveWallPhoto(
                //    wallUploadServerResponse.AlbumID,
                //    /*wallUploadServerResponse.UserId*/0,
                //    Parameters.VKGroupID,
                //    //wallUploadServerResponse.UserId,
                //    ///*Parameters.VKGroupID*/0,
                //    uploadWallPhotoResponse.Photo,
                //    uploadWallPhotoResponse.Server,
                //    uploadWallPhotoResponse.Hash);

                var docsWallUploadServerResponse = (await vkApi.Docs.GetWallUploadServer(Parameters.VKGroupID)).Response;

                var docsUploadFileResponse = await vkApi.Docs.UploadWallFile(
                    await ApplicationData.Current.LocalFolder.GetFileAsync(Parameters.PdfFileName),
                    docsWallUploadServerResponse.UploadUrl);

                var doscSavedInfo = (await vkApi.Docs.SaveWallFile(
                    docsUploadFileResponse.File))
                    .Response[0];

                var wallPost = await vkApi.Wall.Post(-Parameters.VKGroupID, PublishingMessageTextBox.Text,
                    //new VKAttachement(VKAttachementType.Photo, savedPhoto.OwnerID, savedPhoto.ID),
                    new VKAttachement(VKAttachementType.Doc, doscSavedInfo.OwnerID, doscSavedInfo.ID));

                VisualStateManager.GoToState(this, nameof(SuccessState), false);
            }
            catch (VKApiException ex)
            {
                SendError(new VKError("Ошибка", ex.Message));
            }

            SendLoaded();
        }

        private void OnPublishButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Publish();
        }
    }

    public class VKPublishGameInfoToGroupNavigationParameters : NavigationParameters
    {
        public string ScreenshotFileName
        {
            set => SetParam(value);
            get => GetParam<string>();
        }

        public string PdfFileName
        {
            set => SetParam(value);
            get => GetParam<string>();
        }

        public long VKGroupID
        {
            set => SetParam(value);
            get => GetParam<long>();
        }
    }
}
