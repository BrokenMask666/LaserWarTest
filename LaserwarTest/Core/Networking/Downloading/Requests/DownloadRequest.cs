using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace LaserwarTest.Core.Networking.Downloading.Requests
{
    /// <summary>
    /// Представляет идентификатор отдельного запроса на загрузку данных
    /// </summary>
    public class DownloadRequestID
    {
        string _id;

        public string GetID() => _id;

        public DownloadRequestID(string id) { _id = id; }

        public static implicit operator string(DownloadRequestID requestID) => requestID.GetID();
    }

    /// <summary>
    /// Базовый класс, обеспечивающий загрузку файлов из удаленного источника
    /// </summary>
    public class DownloadRequest
    {
        RequestState _state;
        int _progress;

        /// <summary>
        /// Происходит при каждом изменении состояния запроса
        /// </summary>
        public event EventHandler<DownloadRequestStateChangedEventArgs> StateChanged;
        /// <summary>
        /// Происходит при изменении прогресса выполнения запроса
        /// </summary>
        public event EventHandler<DownloadRequestProgressChangedEventArgs> ProgressChanged;
        /// <summary>
        /// Происходит при завершении запроса
        /// </summary>
        public event EventHandler<DownloadRequestCompletedEventArgs> Completed;

        /// <summary>
        /// Получает идентификатор запроса
        /// </summary>
        public DownloadRequestID ID { get; }

        /// <summary>
        /// Получает адрес запрашиваемого файла
        /// </summary>
        public string RequestUrl { get; }
        /// <summary>
        /// Получает имя целевого файла в локальном хранилище
        /// </summary>
        public string DestinationFileName { get; }

        /// <summary>
        /// Получает процент завершения запроса
        /// </summary>
        public int Progress
        {
            private set
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;

                _progress = value;
                ProgressChanged?.Invoke(this, new DownloadRequestProgressChangedEventArgs(ID, value));
            }
            get { return _progress; }
        }

        /// <summary>
        /// Предоставляет токен отмены выполнения запроса
        /// </summary>
        CancellationTokenSource CancellationToken { set; get; }
        /// <summary>
        /// Получает объект, отвечающий за выполнение загрузки
        /// </summary>
        BackgroundDownloader DownloadClient { set; get; }
        /// <summary>
        /// Получает текущую операцию загрузки
        /// </summary>
        DownloadOperation DownloadOperation { set; get; }

        /// <summary>
        /// Получает состояние запроса
        /// </summary>
        public RequestState State
        {
            private set
            {
                if (_state == value) return;

                _state = value;
                StateChanged?.Invoke(this, new DownloadRequestStateChangedEventArgs(ID, value));
            }
            get { return _state; }
        }

        /// <summary>
        /// Получает, может ли запрос быть отменен
        /// </summary>
        public bool CanBeCanceled { get { return _state == RequestState.Processing || _state == RequestState.Paused; } }

        public DownloadRequest(DownloadRequestID id, string requestUrl, string destinationFileName)
        {
            ID = id;
            RequestUrl = requestUrl;
            DestinationFileName = destinationFileName.Replace('/', '\\');
        }

        /// <summary>
        /// Вызывает выполнение запроса
        /// </summary>
        /// <returns></returns>
        public async Task Execute()
        {
            if (State != RequestState.Default) return;

            State = RequestState.Processing;
            Progress = 0;

            Progress<DownloadOperation> progress = new Progress<DownloadOperation>(
                (DownloadOperation operation) =>
                {
                    int progressPercentage = (int)(100 * ((double)operation.Progress.BytesReceived / operation.Progress.TotalBytesToReceive));
                    Debug.WriteLine($"Persantage = {progressPercentage}");

                    Progress = progressPercentage;
                });

            StorageFile destinationFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(DestinationFileName, CreationCollisionOption.ReplaceExisting);

            DownloadClient = new BackgroundDownloader();
            CancellationToken = new CancellationTokenSource();

            DownloadOperation = DownloadClient.CreateDownload(new Uri(RequestUrl), destinationFile);

            try
            {
                await DownloadOperation.StartAsync().AsTask(CancellationToken.Token, progress);
                Complete(RequestCompletionResult.Success);
            }
            catch (TaskCanceledException)
            {
                await destinationFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                Complete(RequestCompletionResult.Canceled);
            }
            catch (Exception)
            {
                await destinationFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                Complete(RequestCompletionResult.Failed);
            }
            finally
            {
                CancellationToken.Dispose();
                CancellationToken = null;
                DownloadClient = null;
            }
        }

        /// <summary>
        /// Отменяет запрос, если это возможно
        /// </summary>
        public void Cancel()
        {
            if (!CanBeCanceled) return;

            if (CancellationToken != null)
                CancellationToken.Cancel();
            else
                Complete(RequestCompletionResult.Canceled);
        }

        /// <summary>
        /// Приостанавливает выполнение запроса
        /// </summary>
        public void Pause()
        {
            if (State != RequestState.Processing) return;

            DownloadOperation.Pause();
            State = RequestState.Paused;
        }

        /// <summary>
        /// Возобновляет выполнение приостановленного запроса
        /// </summary>
        public void Resume()
        {
            if (State != RequestState.Paused) return;

            DownloadOperation.Resume();
            State = RequestState.Processing;
        }

        /// <summary>
        /// Вызывает завершение запроса
        /// </summary>
        /// <param name="result">Результат запроса</param>
        void Complete(RequestCompletionResult result)
        {
            State = (result == RequestCompletionResult.Canceled) 
                ? RequestState.Canceled : RequestState.Completed;

            Completed?.Invoke(this, new DownloadRequestCompletedEventArgs(ID, result));
        }
    }

    /// <summary>
    /// Определяет состояние запроса
    /// </summary>
    public enum RequestState
    {
        /// <summary>
        /// В этом состоянии запрос пребывает изначально
        /// </summary>
        Default = 0,
        /// <summary>
        /// Запрос находится в состоянии обработки
        /// </summary>
        Processing,
        /// <summary>
        /// Запрос находится в процессе ожидания обработки
        /// </summary>
        Paused,
        /// <summary>
        /// Запрос был отменен
        /// </summary>
        Canceled,
        /// <summary>
        /// Запрос был завершен
        /// </summary>
        Completed
    }
}
