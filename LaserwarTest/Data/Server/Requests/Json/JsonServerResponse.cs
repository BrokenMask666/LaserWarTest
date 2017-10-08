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
        public List<GameDataUrlEntity> Games { set; get; }

        [JsonProperty("sounds")]
        public List<SoundEntity> Sounds { set; get; }

        public static JsonServerResponse FromString(string jsonContent)
        {
            return JsonConvert.DeserializeObject<JsonServerResponse>(jsonContent);
        }
    }
}
