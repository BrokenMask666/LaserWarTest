using LaserwarTest.Commons.Observables;
using LaserwarTest.Core.Media;
using LaserwarTest.UI.Controls;
using System;

namespace LaserwarTest.Presentation.Sounds
{
    /// <summary>
    /// Предоставляет возможности управления проигрыванием звукового файла
    /// </summary>
    public sealed class SoundPlayer : ObservableObject
    {
        PlaySoundState _state;
        string _stateMessage;
        double _progressPercentage;
        bool _isEnabled;

        /// <summary>
        /// Получает плеер для проигрывания звука
        /// </summary>
        AudioPlayer AudioPlayer { get; }

        /// <summary>
        /// Имя файла в локальном хранилище
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// Загружен ли файл для вопроизведения
        /// </summary>
        bool IsDownloaded { set; get; }

        /// <summary>
        /// Получает состояние проигрывания
        /// </summary>
        public PlaySoundState State
        {
            private set => SetProperty(ref _state, value);
            get => _state;
        }

        /// <summary>
        /// Получает текст, сопровождающий текущее состояние проигрывания
        /// </summary>
        public string StateMessage
        {
            private set => SetProperty(ref _stateMessage, value);
            get => _stateMessage;
        }

        /// <summary>
        /// Получает процент выполнения проигрывания
        /// </summary>
        public double ProgressPercentage
        {
            private set => SetProperty(ref _progressPercentage, Math.Min(100, value));
            get => _progressPercentage;
        }

        /// <summary>
        /// Получает, доступно ли вопроизведение
        /// </summary>
        public bool IsEnabled
        {
            private set => SetProperty(ref _isEnabled, value);
            get => _isEnabled;
        }

        /// <summary>
        /// Получает команду, отвечающую за управление процессом воспроизведения звука
        /// </summary>
        public IconButtonCommand Command { get; } = new IconButtonCommand();

        public SoundPlayer(AudioPlayer audioPlayer, string fileName, bool isDownloaded)
        {
            FileName = fileName;
            AudioPlayer = audioPlayer;

            Command.Invoked += Command_Invoked;

            SetState((isDownloaded) ? PlaySoundState.Stopped : PlaySoundState.Disabled);
        }

        private void Command_Invoked(object sender, IconButtonCommand e)
        {
            if (State == PlaySoundState.Disabled) return;

            if (State == PlaySoundState.Stopped)
            {
                SetState(PlaySoundState.Playing);

                if (AudioPlayer.IsPlaying) AudioPlayer.Stop();

                AudioPlayer.ProgressChanged += AudioProgressChanged;
                AudioPlayer.MediaStopped += AudioStopped;

                AudioPlayer.Play(new Uri($"ms-appdata:///local/{FileName}"));
            }
            else
            {
                AudioPlayer.Stop();
            }
        }

        /// <summary>
        /// Указывает плееру, что файл загружен
        /// </summary>
        /// <returns></returns>
        public void SetIsDownloaded()
        {
            if (State != PlaySoundState.Disabled || IsDownloaded)
                return;

            SetState(PlaySoundState.Stopped);
        }

        private void SetState(PlaySoundState state)
        {
            switch (state)
            {
                case PlaySoundState.Disabled:
                    StateMessage = "";
                    ProgressPercentage = 0;

                    IsDownloaded = false;
                    IsEnabled = false;
                    break;

                case PlaySoundState.Stopped:
                    StateMessage = "";
                    ProgressPercentage = 0;

                    IsDownloaded = true;
                    IsEnabled = true;
                    break;

                case PlaySoundState.Playing:
                    StateMessage = "0:00";
                    ProgressPercentage = 0;

                    IsDownloaded = true;
                    IsEnabled = true;
                    break;
            }

            State = state;
        }

        private void AudioStopped(object sender, EventArgs e)
        {
            SetState(PlaySoundState.Stopped);
            Dispose();
        }

        private void AudioProgressChanged(object sender, AudioPlayerProgressChangedEventArgs e)
        {
            ProgressPercentage = (100 * e.Position.TotalSeconds / e.Duration.TotalSeconds);

            double positionInSeconds = Math.Round(e.Position.TotalSeconds);
            TimeSpan position = TimeSpan.FromSeconds(positionInSeconds);

            string hoursFormat = (position.Hours > 0) ? @"h\:" : "";
            StateMessage = position.ToString($@"{hoursFormat}m\:ss");
        }

        /// <summary>
        /// Освобождает используемые ресурсы
        /// </summary>
        public void Dispose()
        {
            AudioPlayer.ProgressChanged -= AudioProgressChanged;
            AudioPlayer.MediaStopped -= AudioStopped;
        }
    }

    /// <summary>
    /// перечисление состояний проигрывания звука
    /// </summary>
    public enum PlaySoundState
    {
        /// <summary>
        /// Файл не воспроизводится
        /// </summary>
        Stopped,
        /// <summary>
        /// Файл воспроизводится
        /// </summary>
        Playing,
        /// <summary>
        /// Файл недоступен для проигрывания
        /// </summary>
        Disabled,
    }
}
