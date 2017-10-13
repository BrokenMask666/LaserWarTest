using System;
using Windows.ApplicationModel.Core;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LaserwarTest.Core.Media
{
    /// <summary>
    /// Позволяет проигрывать аудио-файлы
    /// </summary>
    public class AudioPlayer
    {
        MediaElement _player = new MediaElement();
        MediaSource _playerSource;
        DispatcherTimer _audioProgressTimer = new DispatcherTimer();

        bool _waitBeforeStop;

        public event EventHandler MediaStopped;
        public event EventHandler<AudioPlayerProgressChangedEventArgs> ProgressChanged;

        public bool IsPlaying { get { return (_playerSource != null); } }

        public AudioPlayer()
        {
            _audioProgressTimer.Interval = TimeSpan.FromSeconds(0.05);
            _audioProgressTimer.Tick += AudioCompletedTimer_Tick;
        }

        public void Play(Uri soundFileUri)
        {
            if (IsPlaying) Stop();

            _playerSource = MediaSource.CreateFromUri(soundFileUri);
            _playerSource.StateChanged += PlayerSource_StateChanged;

            _player.SetPlaybackSource(_playerSource);
            _player.Play();
        }

        public void Stop()
        {
            if (!IsPlaying) return;

            _waitBeforeStop = false;

            _player.Stop();
            _audioProgressTimer.Stop();

            _playerSource.Dispose();
            _playerSource = null;

            MediaStopped?.Invoke(this, EventArgs.Empty);
        }

        private async void PlayerSource_StateChanged(MediaSource sender, MediaSourceStateChangedEventArgs e)
        {
            switch (e.NewState)
            {
                case MediaSourceState.Opened:
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            _audioProgressTimer.Start();
                        });

                        break;
                    }
            }
        }

        private void AudioCompletedTimer_Tick(object sender, object e)
        {
            if (!_playerSource.IsOpen) return;

            TimeSpan duration = _player.NaturalDuration.TimeSpan;
            TimeSpan position = _player.Position;

            ProgressChanged?.Invoke(this, new AudioPlayerProgressChangedEventArgs(duration, position));

            if (position.TotalSeconds >= duration.TotalSeconds)
            {
                _waitBeforeStop = !_waitBeforeStop;
                if (!_waitBeforeStop) Stop();
            }
        }
    }

    /// <summary>
    /// Аргументы изменения состояния процесса проигрывания аудио-файла
    /// </summary>
    public class AudioPlayerProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Получает длительность файла
        /// </summary>
        public TimeSpan Duration { get; }
        /// <summary>
        /// Получает текущую позицию прогресса
        /// </summary>
        public TimeSpan Position { get; }

        public AudioPlayerProgressChangedEventArgs(TimeSpan duration, TimeSpan position)
        {
            Duration = duration;
            Position = position;
        }
    }
}
