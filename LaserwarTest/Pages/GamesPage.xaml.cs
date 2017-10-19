using LaserwarTest.Presentation.Games;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LaserwarTest.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class GamesPage : Page
    {
        public VMGames VMGames { get; } = new VMGames();

        public GamesPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await VMGames.Load();

            if (VMGames.Items.Count == 0)
                VisualStateManager.GoToState(this, nameof(NoDataState), false);
        }

        private void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement tappedElement)
            {
                if (tappedElement.DataContext is Game game)
                {
                    Frame.Navigate(typeof(GameDetailsPage), game);
                }
            }
        }
    }
}
