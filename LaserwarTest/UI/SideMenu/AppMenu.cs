using LaserwarTest.Commons.Observables;
using LaserwarTest.Pages;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace LaserwarTest.UI.SideMenu
{
    /// <summary>
    /// Представляет меню приложения,
    /// обеспечивает навигацию между различными ключевыми частями приложения
    /// </summary>
    public sealed class AppMenu : ObservableObject
    {
        AppMenuItem _selectedItem;

        /// <summary>
        /// Используется для навигации между различными частями приложения
        /// </summary>
        private Frame Frame { get; }

        /// <summary>
        /// Получает набор доступных элементов меню
        /// </summary>
        public List<AppMenuItem> Items { get; }

        /// <summary>
        /// Получает или задает активный элемент меню
        /// </summary>
        public AppMenuItem SelectedItem
        {
            set
            {
                if (value == null)
                {
                    if (SetProperty(ref _selectedItem, value))
                    {
                        Frame.Content = null;
                        Frame.BackStack.Clear();
                    }

                    return;
                }

                if (!Items.Contains(value) || !SetProperty(ref _selectedItem, value))
                    return;

                Frame.Navigate(value.TargetPageType, value.NavigationParameter);
                Frame.BackStack.Clear();
            }
            get { return _selectedItem; }
        }

        /// <summary>
        /// Создает меню приложения
        /// </summary>
        /// <param name="navigationFrame">Фрейм, в пределах которого будет производиться навигация</param>
        public AppMenu(Frame navigationFrame)
        {
            Frame = navigationFrame;

            AppMenuItem download = new AppMenuItem(
                typeof(DownloadPage),
                iconUri: new Uri("ms-appx:///Assets/Icons/download.png"));

            AppMenuItem sounds = new AppMenuItem(
                typeof(SoundsPage),
                iconUri: new Uri("ms-appx:///Assets/Icons/sounds.png"));

            AppMenuItem games = new AppMenuItem(
                typeof(GamesPage),
                iconUri: new Uri("ms-appx:///Assets/Icons/games.png"));

            Items = new List<AppMenuItem>
            {
                download,
                sounds,
                games,
            };

            SelectedItem = download;
        }
    }
}
