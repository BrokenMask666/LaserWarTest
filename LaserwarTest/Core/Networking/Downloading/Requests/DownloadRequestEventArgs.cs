using System;

namespace LaserwarTest.Core.Networking.Downloading.Requests
{
    /// <summary>
    /// Аргументы событий запроса загрузки
    /// </summary>
    public class DownloadRequestEventArgs : EventArgs
    {
        public DownloadRequestID RequestID { get; }

        public DownloadRequestEventArgs(DownloadRequestID requestID)
        {
            RequestID = requestID;

        }
    }

    /// <summary>
    /// Аргументы события изменения прогресса выполнения запроса
    /// </summary>
    public class DownloadRequestProgressChangedEventArgs : DownloadRequestEventArgs
    {
        /// <summary>
        /// Получает количество процентов завершения операции загрузки
        /// </summary>
        public int Percentage { get; }

        public DownloadRequestProgressChangedEventArgs(DownloadRequestID requestID, int percentage) : base(requestID)
        {
            if (percentage < 0) percentage = 0;
            if (percentage > 100) percentage = 100;

            Percentage = percentage;
        }
    }

    /// <summary>
    /// Аргументы события изменения состояния запроса загрузки
    /// </summary>
    public class DownloadRequestStateChangedEventArgs : DownloadRequestEventArgs
    {
        /// <summary>
        /// Получает новое состояние запроса
        /// </summary>
        public RequestState NewState { get; }

        public DownloadRequestStateChangedEventArgs(DownloadRequestID requestID, RequestState requestState) : base(requestID)
        {
            NewState = requestState;
        }
    }

    /// <summary>
    /// Аргументы собsтия завершения запроса загрузки
    /// </summary>
    public class DownloadRequestCompletedEventArgs : DownloadRequestEventArgs
    {
        /// <summary>
        /// Получает результат завершения запроса
        /// </summary>
        public RequestCompletionResult Result { get; }

        public DownloadRequestCompletedEventArgs(DownloadRequestID requestID, RequestCompletionResult result) : base(requestID)
        {
            Result = result;
        }
    }

    /// <summary>
    /// Определяет, каким образом был завершен запрос на загрузку файла
    /// </summary>
    public enum RequestCompletionResult
    {
        /// <summary>
        /// Запрос выполнен успешно
        /// </summary>
        Success = 0,
        /// <summary>
        /// Запрос был отменен
        /// </summary>
        Canceled,
        /// <summary>
        /// Загрузка завершилась с ошибкой
        /// </summary>
        Failed
    }



    
}
