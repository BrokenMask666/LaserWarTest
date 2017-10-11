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
        string _originalJson;

        [JsonProperty("error")]
        public string Error { set; get; }

        [JsonProperty("games")]
        public List<GameDataUrlEntity> Games { set; get; }

        [JsonProperty("sounds")]
        public List<SoundEntity> Sounds { set; get; }

        public static JsonServerResponse FromString(string jsonContent)
        {
            JsonServerResponse ret = JsonConvert.DeserializeObject<JsonServerResponse>(jsonContent);
            ret._originalJson = jsonContent;

            return ret;
        }

        public string GetOriginalJson()
        {
            /// Приходится извращаться, поскольку десериализованный объект он не хочет сериализовать,
            /// NullReferenceException видите ли, а разбираться лень
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(_originalJson), Formatting.Indented);
        }
    }
}
