using LaserwarTest.Presentation.Sounds;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace LaserwarTest.UI.Controls
{
    public sealed class SoundPlayerIconButton : IconButton
    {
        static BitmapImage ICON_PLAY = new BitmapImage(new Uri("ms-appx:///Assets/Icons/play.png"));
        static BitmapImage ICON_PLAY_DISABLED = new BitmapImage(new Uri("ms-appx:///Assets/Icons/play_disabled.png"));
        static BitmapImage ICON_STOP = new BitmapImage(new Uri("ms-appx:///Assets/Icons/stop.png"));

        public SoundPlayerIconButton()
        {
            Width = 14;
            Height = 14;

            ApplyState(State);
        }

        #region DependencyProperty

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                nameof(State),
                typeof(PlaySoundState),
                typeof(SoundPlayerIconButton),
                new PropertyMetadata(PlaySoundState.Stopped, OnStatePropertyChanged));

        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SoundPlayerIconButton obj = d as SoundPlayerIconButton;
            if (obj == null) return;

            obj.ApplyState((PlaySoundState)e.NewValue);
        }

        private void ApplyState(PlaySoundState state)
        {
            switch (state)
            {
                case PlaySoundState.Stopped:
                    Icon = ICON_PLAY;
                    break;

                case PlaySoundState.Playing:
                    Icon = ICON_STOP;
                    break;

                case PlaySoundState.Disabled:
                    Icon = ICON_PLAY_DISABLED;
                    break;
            }
        }

        public PlaySoundState State
        {
            set { SetValue(StateProperty, value); }
            get { return (PlaySoundState)GetValue(StateProperty); }
        }

        #endregion DependencyProperty
    }
}
