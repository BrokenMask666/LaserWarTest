using LaserwarTest.Presentation.Sounds;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace LaserwarTest.UI.Controls
{
    public sealed class DownloadIconButton : IconButton
    {
        static BitmapImage ICON_DOWNLOAD = new BitmapImage(new Uri("ms-appx:///Assets/Icons/download_sound.png"));
        static BitmapImage ICON_DOWNLOADING = new BitmapImage(new Uri("ms-appx:///Assets/Icons/downloading_sound.png"));
        static BitmapImage ICON_DOWNLOADED = new BitmapImage(new Uri("ms-appx:///Assets/Icons/downloaded_sound.png"));

        public DownloadIconButton()
        {
            Width = 20;
            Height = 16;

            ApplyState(State);
        }

        #region DependencyProperty

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                nameof(State),
                typeof(DownloadSoundState),
                typeof(DownloadIconButton),
                new PropertyMetadata(DownloadSoundState.Download, OnStatePropertyChanged));

        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DownloadIconButton obj = d as DownloadIconButton;
            if (obj == null) return;

            obj.ApplyState((DownloadSoundState)e.NewValue);
        }

        private void ApplyState(DownloadSoundState state)
        {
            switch (state)
            {
                case DownloadSoundState.Download:
                    Icon = ICON_DOWNLOAD;
                    break;

                case DownloadSoundState.Downloading:
                    Icon = ICON_DOWNLOADING;
                    break;

                case DownloadSoundState.Downloaded:
                    Icon = ICON_DOWNLOADED;
                    break;
            }
        }

        public DownloadSoundState State
        {
            set { SetValue(StateProperty, value); }
            get { return (DownloadSoundState)GetValue(StateProperty); }
        }

        #endregion DependencyProperty
    }
}
