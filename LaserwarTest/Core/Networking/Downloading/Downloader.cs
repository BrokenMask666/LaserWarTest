using LaserwarTest.Core.Networking.Downloading.Requests;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
        /// Выполняет Get-запрос по указанному адресу и возвращает результат в виде строки
        /// </summary>
        /// <param name="requestUri">Адрес ресурса</param>
        /// <returns></returns>
        public async Task<string> GetStringRequest(string requestUri)
        {
            HttpClient client = new HttpClient();

            return await client.GetStringAsync(requestUri);
        }

        /// <summary>
        /// Запускает выполнение указанного запроса.
        /// Если загрузчик приостановлен, то просто добавляет в коллекцию запросов
        /// до тех пор, пока загрузчик не будет возобновлен
        /// </summary>
        /// <param name="request"></param>
        public void ExecuteRequest(DownloadRequest request)
        {
            if (ContainsRequest(request.ID))
            {
                /// TODO: выбросить исключение
                throw new NotImplementedException("Если запрос уже в очереди, выбросить исключение");
            }

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
        /// Возобновляет работу загрузчика, если он был приостановлен
        /// </summary>
        public void Resume()
        {
            if (State != DownloaderState.Suspended) return;

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
