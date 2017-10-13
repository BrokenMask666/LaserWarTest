using LaserwarTest.Commons.Observables;
using LaserwarTest.Core.Networking.Downloading;
using LaserwarTest.Core.Networking.Downloading.Requests;
using LaserwarTest.Data.Server.Requests;
using LaserwarTest.UI.Controls;
using System;

namespace LaserwarTest.Presentation.Sounds
{
    /// <summary>
    /// Представляет возможности управления загрузкой звукового файла
    /// </summary>
    public sealed class SoundDownloader : ObservableObject
    {
        DownloadSoundState _state;
        string _stateMessage;
        int _progressPercentage;
        bool _isEnabled;

        /// <summary>
        /// Происходит, когда процесс загрузки завершен
        /// </summary>
        public event EventHandler Downloaded;

        /// <summary>
        /// Адрес загрузки файла
        /// </summary>
        string DownloadUrl { get; }
        /// <summary>
        /// Имя файла в локальном хранилище
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Запрос на загрузку файла
        /// </summary>
        DownloadRequest Request { set; get; }
        /// <summary>
        /// Идентификатор запроса
        /// </summary>
        DownloadRequestID RequestID { set; get; }

        /// <summary>
        /// Получает команду, отвечающую за управление процессом загрузки
        /// </summary>
        public IconButtonCommand Command { get; } = new IconButtonCommand();

        /// <summary>
        /// Получает состояние загрузки
        /// </summary>
        public DownloadSoundState State
        {
            private set => SetProperty(ref _state, value);
            get => _state;
        }

        /// <summary>
        /// Получает текст, сопровождающий текущее состояние загрузки
        /// </summary>
        public string StateMessage
        {
            private set => SetProperty(ref _stateMessage, value);
            get => _stateMessage;
        }

        /// <summary>
        /// Получает процент выполнения загрузки
        /// </summary>
        public int ProgressPercentage
        {
            private set => SetProperty(ref _progressPercentage, value);
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
        /// Получает, загружен ли файл
        /// </summary>
        public bool IsDownloaded { private set; get; }

        /// <summary>
        /// Создает новый загрузчик звукового файла
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="downloadUrl">Адрес файла для загрузки</param>
        /// <param name="fileName">Расположение файла в локальном хранилище</param>
        /// <param name="isDownloaded">Указывает, загружен ли уже файл</param>
        public SoundDownloader(int id, string downloadUrl, string fileName, bool isDownloaded)
        {
            DownloadUrl = downloadUrl;
            FileName = fileName;

            RequestID = DownloadRequestIDGenerator.Sound(id);

            Command.Invoked += Command_Invoked;

            if (isDownloaded)
                SetState(DownloadSoundState.Downloaded);
            else
            {
                Request = Downloader.GetCurrent().GetIfExist(RequestID);
                if (Request == null)
                    SetState(DownloadSoundState.Download);
                else
                {
                    switch (Request.State)
                    {
                        case RequestState.Canceled:
                            SetState(DownloadSoundState.Download);
                            break;

                        case RequestState.Completed:
                            SetState(DownloadSoundState.Downloaded);
                            break;

                        default:
                            SetState(DownloadSoundState.Downloading);

                            Request.StateChanged += DownloadStateChanged;
                            Request.ProgressChanged += DownloadProgressChanged;
                            Request.Completed += DownloadCompleted;
                            break;
                    }
                }
            }
        }

        private void Command_Invoked(object sender, IconButtonCommand e)
        {
            switch (State)
            {
                case DownloadSoundState.Download:
                    Request = new DownloadRequest(RequestID, DownloadUrl, FileName);

                    Request.StateChanged += DownloadStateChanged;
                    Request.ProgressChanged += DownloadProgressChanged;
                    Request.Completed += DownloadCompleted;

                    SetState(DownloadSoundState.Downloading);
                    Downloader.GetCurrent().ExecuteRequest(Request);
                    break;

                case DownloadSoundState.Downloading:
                    Request?.Cancel();
                    break;

                case DownloadSoundState.Downloaded:
                    break;
            }
        }

        private void SetState(DownloadSoundState state)
        {
            switch (state)
            {
                case DownloadSoundState.Download:
                    StateMessage = "";
                    ProgressPercentage = 0;

                    IsDownloaded = false;
                    IsEnabled = true;
                    break;

                case DownloadSoundState.Downloading:
                    if (Request == null) return;

                    SetProgress(Request.Progress);

                    IsDownloaded = false;
                    IsEnabled = true;
                    break;

                case DownloadSoundState.Downloaded:
                    StateMessage = "Файл загружен";
                    ProgressPercentage = 100;

                    IsDownloaded = true;
                    IsEnabled = false;
                    break;
            }

            State = state;
        }

        private void SetProgress(int value)
        {
            ProgressPercentage = value;
            StateMessage = $"{value} %";
        }

        private void DownloadStateChanged(object sender, DownloadRequestStateChangedEventArgs e)
        {
            switch (e.NewState)
            {
                case RequestState.Canceled:
                    SetState(DownloadSoundState.Download);
                    break;

                case RequestState.Completed:
                    SetState(DownloadSoundState.Downloaded);
                    break;

                default:
                    SetState(DownloadSoundState.Downloading);
                    break;
            }
        }

        private void DownloadProgressChanged(object sender, DownloadRequestProgressChangedEventArgs e)
        {
            SetProgress(e.Percentage);
        }

        private void DownloadCompleted(object sender, DownloadRequestCompletedEventArgs e)
        {
            Dispose();

            if (e.Result == RequestCompletionResult.Failed)
                SetState(DownloadSoundState.Download);
            else if (e.Result == RequestCompletionResult.Success)
                Downloaded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Производит освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            if (Request == null) return;

            Request.StateChanged += DownloadStateChanged;
            Request.ProgressChanged += DownloadProgressChanged;
            Request.Completed += DownloadCompleted;

            Request = null;
        }
    }

    /// <summary>
    /// Перечисление состояний загрузки файла
    /// </summary>
    public enum DownloadSoundState
    {
        /// <summary>
        /// Необходимо провести загрузку
        /// </summary>
        Download,
        /// <summary>
        /// Загрузка в процессе выполнения
        /// </summary>
        Downloading,
        /// <summary>
        /// Файл загружен
        /// </summary>
        Downloaded
    }
}
