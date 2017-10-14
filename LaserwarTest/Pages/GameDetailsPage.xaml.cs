using LaserwarTest.Presentation.Games;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            VMGameDetails.Sort(new PlayerComparer());
        }

        private void TextBlock_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            VMGameDetails.Sort(new PlayerByRatingComparer());

        }

        private void TextBlock_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            VMGameDetails.Sort(new PlayerByAccuracComparer());

        }

        private void TextBlock_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            VMGameDetails.Sort(new PlayerByShotsComparer());
        }
        
        private void OnListViewItemClick_LostFocus(object sender, ItemClickEventArgs e)
        {
            object focusedElement = FocusManager.GetFocusedElement();
            if (focusedElement is Control control && control.FocusState != FocusState.Unfocused)
                Focus(FocusState.Programmatic);
        }
    }
}
