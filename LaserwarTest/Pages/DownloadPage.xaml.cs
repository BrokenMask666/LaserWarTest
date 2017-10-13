using LaserwarTest.Presentation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LaserwarTest.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class DownloadPage : Page
    {
        VMDownload VMDownload { get; } = new VMDownload();

        public DownloadPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            VMDownload.CheckDataDownloadedStatus();
        }

        private void DownloadFileButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            VMDownload.DownloadFile();
        }
    }
}
