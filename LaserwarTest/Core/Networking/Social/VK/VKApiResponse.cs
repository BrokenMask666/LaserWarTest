using Newtonsoft.Json;

namespace LaserwarTest.Core.Networking.Social.VK
{
    /// <summary>
    /// Базовый объект для представления ответа от сервера при использовании API ВКонтакте
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class VKApiResponse
    {
        [JsonProperty("error")]
        public VKResponseError Error { set; get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKResponseError
    {
        [JsonProperty("error_code")]
        public int Code { set; get; }

        [JsonProperty("error_msg")]
        public string Message { set; get; }
    }
}
