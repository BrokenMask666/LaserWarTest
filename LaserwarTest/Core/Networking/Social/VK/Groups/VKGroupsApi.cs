using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaserwarTest.Core.Networking.Social.VK.Groups
{
    /// <summary>
    /// Предоставляет единый доступ к API-функциям групп пользователя
    /// </summary>
    public sealed class VKGroupsApi : VKApiBase
    {
        public async Task<VKGroupsGetResponse> Get()
        {
            var response = await new VKApiRequest($"https://api.vk.com/method/groups.get?" +
                    $"access_token={ApiInfo.AccessToken}" +
                    $"&extended=1" +
                    $"&fields=status,can_post" +
                    $"&v={ApiInfo.APIVersion}")
                    .Execute<VKGroupsGetResponse>();

            if (response.Error != null)
            {
                switch (response.Error.Code)
                {
                    case 260:
                        throw new VKApiException("Доступ к запрошенному списку групп ограничен настройками приватности пользователя");

                    default:
                        throw new VKApiException($"Code: {response.Error.Code}\nMessage: {response.Error.Message}");
                }
            }

            return response;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKGroupsGetResponse : VKApiResponse
    {
        [JsonProperty("response")]
        public VKGroupsGetResponseDetails Response { set; get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKGroupsGetResponseDetails
    {
        [JsonProperty("count")]
        public int Count { set; get; }
        [JsonProperty("items")]
        public List<VKGroupInfo> Groups { set; get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKGroupInfo
    {
        [JsonProperty("id")]
        public long ID { set; get; }

        [JsonProperty("name")]
        public string Name { set; get; }

        [JsonProperty("type")]
        public string Type { set; get; }
        [JsonProperty("is_closed")]
        public bool IsClosed { set; get; }

        [JsonProperty("photo_50")]
        public string Photo_50 { set; get; }
        [JsonProperty("photo_100")]
        public string Photo_100 { set; get; }
        [JsonProperty("photo_200")]
        public string Photo_200 { set; get; }

        [JsonProperty("status")]
        public string Status { set; get; }
        [JsonProperty("can_post")]
        public bool CanPost { set; get; }

    }
}
