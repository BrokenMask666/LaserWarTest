using LaserwarTest.UI.SideMenu;
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
    public sealed partial class AppShell : Page
    {
        static AppShell _shell;
        public static AppShell GetCurrent() => _shell ?? throw new Exception("Application shell hasn't been initialized");

        AppMenu Menu { get; }

        public AppShell()
        {
            if (_shell != null) throw new Exception("Application shell has already been initialized");

            InitializeComponent();
            _shell = this;

            Menu = new AppMenu(InnerFrame);
        }
    }

    public class AppMenuItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is AppMenuItem menuItem && targetType == typeof(AppMenuItem))
                return menuItem;

            return value;
        }
    }
}
