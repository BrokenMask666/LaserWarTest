using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Core.Networking.Downloading
{
    /// <summary>
    /// Выбрасывается, если загрузчик обнаружил, что отсутствует подключение к интернету
    /// </summary>
    public sealed class NoNetworkDownloaderException : Exception
    {
        public NoNetworkDownloaderException() { }
        public NoNetworkDownloaderException(string message) : base(message) { }
        public NoNetworkDownloaderException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Выбрасывается, если была попытка добавить в загрузчик запрос, который уже выполняется
    /// </summary>
    public sealed class RequestAlreadyExistsDownloaderException : Exception
    {
        public RequestAlreadyExistsDownloaderException() { }
        public RequestAlreadyExistsDownloaderException(string message) : base(message) { }
        public RequestAlreadyExistsDownloaderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
