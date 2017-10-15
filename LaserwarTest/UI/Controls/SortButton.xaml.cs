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
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LaserwarTest.UI.Controls
{
    [ContentProperty(Name = nameof(InnerContent))]
    public sealed partial class SortButton : UserControl
    {
        public event EventHandler<bool> SortRequested;

        public SortButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register(
                nameof(InnerContent),
                typeof(object),
                typeof(SortButton),
                new PropertyMetadata(null, OnInnerContentChanged));

        private static void OnInnerContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SortButton obj = d as SortButton;
            if (obj == null) return;

            obj.ButtonContent.Content = e.NewValue;
        }

        public object InnerContent
        {
            set { SetValue(InnerContentProperty, value); }
            get { return GetValue(InnerContentProperty); }
        }

        /// <summary>
        /// Определяет, в каком состоянии находится объект.
        /// True - активна сортировка по убыванию.
        /// False - активна сортировка по возрастанию.
        /// Null - сортировка не активна
        /// </summary>
        public bool? SortByDesc { private set; get; }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            /// ms-appx:///Assets/Icons/sort.png
            if (SortByDesc == null || SortByDesc == false)
            {
                SortByDesc = true;
                IconImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/sort.png"));
                IconImageProjection.RotationX = 0;

                SortRequested?.Invoke(this, true);
            }
            else
            {
                SortByDesc = false;
                IconImageProjection.RotationX = 180;

                SortRequested?.Invoke(this, false);
            }
        }

        /// <summary>
        /// Сбрасывает указатель активности сортировка
        /// </summary>
        public void ResetSortRequest()
        {
            IconImage.Source = null;
            SortByDesc = null;
        }
    }
}
