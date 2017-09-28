using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace LaserwarTest.Core.Networking.Downloading.Requests
{
    /// <summary>
    /// Представляет метод, выполняющийся по завершении процесса выполнения запроса загрузки
    /// </summary>
    public delegate void DownloadRequestCompletedHandler();

    /// <summary>
    /// Представляет идентификатор отдельного запроса на загрузку данных
    /// </summary>
    public class DownloadRequestID
    {
        string _id;

        public string GetID() => _id;

        protected DownloadRequestID(string id)
        {
            _id = id;
        }


        public static implicit operator string(DownloadRequestID requestID) => requestID.GetID();
    }

    /// <summary>
    /// Базовый класс, обеспечивающий загрузку файлов из удаленного источника
    /// </summary>
    public abstract class DownloadRequest
    {
        RequestState _state;

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
        public string RequestUri { get; }
        /// <summary>
        /// Получает целевой файл
        /// </summary>
        public StorageFile DestinationFile { get; }

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
        /// Получает метод, выполняющийся по завершении выполнения запроса
        /// </summary>
        DownloadRequestCompletedHandler OnCompletedHandler { get; }

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

        public DownloadRequest(DownloadRequestID id, string requestUri, StorageFile destinationFile, DownloadRequestCompletedHandler onCompleted = null)
        {
            ID = id;
            RequestUri = requestUri;
            DestinationFile = destinationFile;
            OnCompletedHandler = onCompleted;
        }

        /// <summary>
        /// Вызывает выполнение запроса
        /// </summary>
        /// <returns></returns>
        public async Task Execute()
        {
            if (State != RequestState.Default) return;

            State = RequestState.Processing;
            ReportProgress(0);

            Progress<DownloadOperation> progress = new Progress<DownloadOperation>(
                (DownloadOperation operation) =>
                {
                    int progressPercentage = (int)(100 * ((double)operation.Progress.BytesReceived / operation.Progress.TotalBytesToReceive));
                    Debug.WriteLine($"Persantage = {progressPercentage}");

                    ReportProgress(progressPercentage);
                });


            DownloadClient = new BackgroundDownloader();
            CancellationToken = new CancellationTokenSource();

            DownloadOperation = DownloadClient.CreateDownload(new Uri(RequestUri), DestinationFile);
            try
            {
                await DownloadOperation.StartAsync().AsTask(CancellationToken.Token, progress);
                OnCompletedHandler?.Invoke();
                Complete(RequestCompletionResult.Success);
            }
            catch (TaskCanceledException)
            {
                await DestinationFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                Complete(RequestCompletionResult.Canceled);
            }
            catch (Exception)
            {
                await DestinationFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                Complete(RequestCompletionResult.Failed);
            }
            finally
            {
                CancellationToken.Dispose();
                CancellationToken = null;
                DownloadClient = null;
            }
        }

        private void ReportProgress(int progressPercentage)
        {
            ProgressChanged?.Invoke(this, new DownloadRequestProgressChangedEventArgs(ID, progressPercentage));
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
