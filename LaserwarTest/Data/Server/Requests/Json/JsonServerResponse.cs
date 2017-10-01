using LaserwarTest.Data.DB.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LaserwarTest.Data.Server.Requests.Json
{
    /// <summary>
    /// Представляет данные, возвращаемые сервером, представленные в виде Json
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class JsonServerResponse
    {
        [JsonProperty("error")]
        public string Error { set; get; }

        [JsonProperty("games")]
        public List<JsonGameDownloadData> Games { set; get; }

        [JsonProperty("sounds")]
        public List<SoundEntity> Sounds { set; get; }
    }

    /// <summary>
    /// Представляет данные о расположении загружаемых файлов
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class JsonGameDownloadData
    {
        [JsonProperty("url")]
        public string Url { set; get; }
    }
}
