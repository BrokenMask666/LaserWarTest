using LaserwarTest.Commons.Observables;
using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace LaserwarTest.UI.SideMenu
{
    /// <summary>
    /// Представляет отдельный элемент меню приложения
    /// </summary>
    public class AppMenuItem : ObservableObject
    {
        ImageSource _icon;

        public Type TargetPageType { get; }
        public object NavigationParameter { get; }

        public ImageSource Icon
        {
            set { SetProperty(ref _icon, value); }
            get { return _icon; }
        }

        public AppMenuItem(Type targetPageType, object navigationParameter = null, Uri iconUri = null)
        {
            TargetPageType = targetPageType;
            NavigationParameter = navigationParameter;

            if (iconUri != null)
                _icon = new BitmapImage(iconUri);
        }
    }
}
