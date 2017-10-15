using LaserwarTest.Presentation.Games;
using LaserwarTest.Presentation.Games.Comparers;
using LaserwarTest.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LaserwarTest.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class GameDetailsPage : Page
    {
        VMGameDetails VMGameDetails { get; } = new VMGameDetails();

        SortButton ActiveSortButton { set; get; }

        public GameDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Game game)
            {
                PageLayout.Title = game.Name;
                VMGameDetails.Load(game.ID);
            }
        }

        private void SortByPlayerRequested(object sender, bool byDesc)
        {
            Sort(sender as SortButton, new PlayerComparer(desc: byDesc));
        }

        private void SortByRatingRequested(object sender, bool byDesc)
        {
            Sort(sender as SortButton, new PlayerByRatingComparer(desc: byDesc));

        }

        private void SortByAccuracyRequested(object sender, bool byDesc)
        {
            Sort(sender as SortButton, new PlayerByAccuracComparer(desc: byDesc));

        }

        private void SortByShotsRequested(object sender, bool byDesc)
        {
            Sort(sender as SortButton, new PlayerByShotsComparer(desc: byDesc));
        }

        void Sort(SortButton sender, PlayerComparer comparer)
        {
            if (ActiveSortButton == null)
                ActiveSortButton = sender;
            else if (ActiveSortButton != sender)
            {
                ActiveSortButton.ResetSortRequest();
                ActiveSortButton = sender;
            }

            VMGameDetails.Sort(comparer);
        }


        void ResetFocus()
        {
            object focusedElement = FocusManager.GetFocusedElement();
            if (focusedElement is Control control && control.FocusState != FocusState.Unfocused)
                Focus(FocusState.Programmatic);
        }
        
        private void OnListViewItemClick_LostFocus(object sender, ItemClickEventArgs e)
        {
            ResetFocus();
        }

        private void OnLostFocus_SavePlayerData(object sender, RoutedEventArgs e)
        {
            if (sender is Control control && control.DataContext is Player player)
            {
                if (control is TextBox tb)
                {
                    BindingExpression expr = tb.GetBindingExpression(TextBox.TextProperty);
                    expr?.UpdateSource();
                }

                player.Save();
            }
        }

        private void OnEnterUp_SavePlayerData(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != Windows.System.VirtualKey.Enter) return;
            ResetFocus();
        }

        private void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement tappedElement)
            {
                if (tappedElement.DataContext is Player player)
                {
                    VMGameDetails.EditPlayer(player);
                }
            }
        }
    }
}
