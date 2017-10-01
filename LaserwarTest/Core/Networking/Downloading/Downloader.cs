using LaserwarTest.Core.Networking.Downloading.Requests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace LaserwarTest.Core.Networking.Downloading
{
    /// <summary>
    /// Обеспечивает общий доступ к механизмам загрузки данных
    /// </summary>
    public sealed class Downloader
    {
        static Lazy<Downloader> _instance = new Lazy<Downloader>(() => new Downloader(), true);

        DownloaderState _state;

        /// <summary>
        /// Получает набор выполняющихся в данный момент запросов
        /// </summary>
        Dictionary<string, DownloadRequest> Requests { get; } = new Dictionary<string, DownloadRequest>();

        /// <summary>
        /// Получает состояние загрузчика
        /// </summary>
        public DownloaderState State
        {
            private set
            {
                if (_state == value) return;

                _state = value;
            }
            get { return _state; }
        }

        Downloader() { }

        /// <summary>
        /// Получает загрузчик
        /// </summary>
        /// <returns></returns>
        public static Downloader GetCurrent() => _instance.Value;

        /// <summary>
        /// Запускает выполнение указанного запроса.
        /// Если загрузчик приостановлен, то просто добавляет в коллекцию запросов
        /// до тех пор, пока загрузчик не будет возобновлен.
        /// 
        /// Выбрасывает <see cref="RequestAlreadyExistsDownloaderException"/>,
        /// если запрос с таким же идентификатором уже выполняется.
        /// 
        /// Выбрасывает <see cref="NoNetworkDownloaderException"/>
        /// при отсутствии подключения к интернету
        /// </summary>
        /// <param name="request"></param>
        public void ExecuteRequest(DownloadRequest request)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new NoNetworkDownloaderException();

            if (ContainsRequest(request.ID))
                throw new RequestAlreadyExistsDownloaderException($"Запрос с идентификатором '{request.ID.GetID()}' уже выполняется");

            request.Completed += OnRequestCompleted;

            Requests.Add(request.ID, request);

            if (State == DownloaderState.Sleep) State = DownloaderState.Active;
            if (State == DownloaderState.Active) request.Execute();
        }

        private void OnRequestCompleted(object sender, DownloadRequestCompletedEventArgs e)
        {
            Requests.Remove(e.RequestID);
            if (Requests.Count == 0) State = DownloaderState.Sleep;
        }

        /// <summary>
        /// Приостанавливает работу загрузчика и всех связанных с ним запросов
        /// </summary>
        public void Suspend()
        {
            if (State != DownloaderState.Active) return;

            foreach (var request in Requests.Values)
                request.Pause();
        }

        /// <summary>
        /// Возобновляет работу загрузчика, если он был приостановлен.
        /// 
        /// Если на момент возобновления отсутствует интернет подключение,
        /// будет выброшено <see cref="NoNetworkDownloaderException"/>,
        /// а все выполняющиеся загрузки будут отменены
        /// </summary>
        public void Resume()
        {
            if (State != DownloaderState.Suspended) return;

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                foreach (var request in Requests.Values)
                    request.Cancel();

                Requests.Clear();

                throw new NoNetworkDownloaderException("При возобновлении загрузчика не обнаружился интернет! Все загрузки отменены!");
            }

            foreach (var request in Requests.Values)
            {
                if (request.State == RequestState.Default)
                    request.Execute();
                else
                    request.Resume();
            }
        }

        /// <summary>
        /// Получает, находится ли запрос с указанным идентификатором в процессе обработки
        /// </summary>
        /// <param name="requestID">Идентифкатор запроса</param>
        /// <returns></returns>
        public bool ContainsRequest(DownloadRequestID requestID)
        {
            return Requests.ContainsKey(requestID);
        }

        /// <summary>
        /// Получает запрос, если таковой обрабатывается, иначе - null
        /// </summary>
        /// <param name="requestID">Идентифкатор запроса</param>
        /// <returns></returns>
        public DownloadRequest GetIfExist(DownloadRequestID requestID)
        {
            DownloadRequest request = null;
            Requests.TryGetValue(requestID, out request);

            return request;
        }
    }

    /// <summary>
    /// Перечисление состояний загрузчика
    /// </summary>
    public enum DownloaderState
    {
        /// <summary>
        /// Загрузчик не выполняет никакой работы
        /// </summary>
        Sleep = 0,
        /// <summary>
        /// Загрузчик находится в процессе выполнения запросов
        /// </summary>
        Active,
        /// <summary>
        /// Работа загрузчика была приостановлена
        /// </summary>
        Suspended,
    }
}
