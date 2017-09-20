using LaserwarTest.Helpers;
using LaserwarTest.UI.Commands.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LaserwarTest.UI.Layouts
{
    [ContentProperty(Name = nameof(InnerContent))]
    public sealed partial class CommonLayout : UserControl, INotifyPropertyChanged
    {
        public BackButton BackButton { get; } = new BackButton();

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T valueHolder, T value, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(valueHolder, value))
                return false;

            valueHolder = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            return true;
        }

        

        public CommonLayout()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Frame frame = VisualTreeExplorer.FindParent<Frame>(this);
                if (frame != null) BackButton.SetFrame(frame);
            };
        }

        #region DependencyProperty

        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register(
                nameof(InnerContent),
                typeof(object),
                typeof(CommonLayout),
                null);


        public object InnerContent
        {
            set { SetValue(InnerContentProperty, value); }
            get { return GetValue(InnerContentProperty); }
        }

        #endregion DependencyProperty
    }
}
