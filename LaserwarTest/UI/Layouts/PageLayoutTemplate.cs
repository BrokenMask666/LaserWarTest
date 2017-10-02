using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace LaserwarTest.UI.Layouts
{
    [ContentProperty(Name = nameof(Content))]
    public sealed class PageLayoutTemplate : Control
    {
        public PageLayoutTemplate()
        {
            DefaultStyleKey = typeof(PageLayoutTemplate);
        }


        #region DependencyProperty

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(PageLayoutTemplate),
                new PropertyMetadata(""));

        public string Title
        {
            set { SetValue(TitleProperty, value.ToUpper()); }
            get { return (string)GetValue(TitleProperty); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(PageLayoutTemplate),
                new PropertyMetadata(null));

        public object Content
        {
            set { SetValue(ContentProperty, value); }
            get { return GetValue(ContentProperty); }
        }

        public static readonly DependencyProperty TitleRightContentProperty =
            DependencyProperty.Register(
                nameof(TitleRightContent),
                typeof(object),
                typeof(PageLayoutTemplate),
                null);


        public object TitleRightContent
        {
            set { SetValue(TitleRightContentProperty, value); }
            get { return GetValue(TitleRightContentProperty); }
        }

        #endregion DependencyProperty
    }
}
