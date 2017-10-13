using LaserwarTest.Core.Networking.Downloading.Requests;

namespace LaserwarTest.Data.Server.Requests
{
    /// <summary>
    /// Позволяет генерировать различные идентификаторы для загрузки разных групп файлов
    /// </summary>
    public static class DownloadRequestIDGenerator
    {
        /// <summary>
        /// Получает идентифкатор загрузки звукового файла
        /// </summary>
        /// <param name="id">Идентификатор звукового файла</param>
        /// <returns></returns>
        public static DownloadRequestID Sound(int id) => new DownloadRequestID($"sound_{id}");
    }
}
