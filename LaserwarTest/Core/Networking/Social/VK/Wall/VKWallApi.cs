using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Core.Networking.Social.VK.Wall
{
    /// <summary>
    /// Предоставляет единый доступ к API-функциям стены пользователя или сообщества
    /// </summary>
    public sealed class VKWallApi : VKApiBase
    {
        public async Task<VKApiResponse> Post(long ownerID, string message, params VKAttachement[] attachements)
        {
            var response = await new VKApiRequest($"https://api.vk.com/method/wall.post?" +
                    $"access_token={ApiInfo.AccessToken}" +
                    $"&owner_id={ownerID}" +
                    $"&message={message}" +
                    ((attachements.Count() > 0) ? $"&attachments={string.Join(",", attachements.Select(x => x.VKString))}" : "") +
                    //$"&guid={new Guid()}" +
                    $"&v={ApiInfo.APIVersion}")
                    .Execute<VKApiResponse>();

            if (response.Error != null)
            {
                switch (response.Error.Code)
                {
                    case 214:
                        throw new VKApiException($"Публикация запрещена. Превышен лимит на число публикаций в сутки, либо на указанное время уже запланирована другая запись, либо для текущего пользователя недоступно размещение записи на этой стене");

                    case 219:
                        throw new VKApiException($"Рекламный пост уже недавно публиковался");

                    case 220:
                        throw new VKApiException($"Слишком много получателей");

                    case 222:
                        throw new VKApiException($"Запрещено размещать ссылки");

                    default:
                        throw new VKApiException($"Code: {response.Error.Code}\nMessage: {response.Error.Message}");
                }
            }

            return response;
        }
    }

    public class VKAttachement
    {
        public long MediaID { get; }
        public long OwnerID { get; }

        public VKAttachementType Type { get; }

        public string VKString { get; }

        public VKAttachement(VKAttachementType type, long ownerID, long mediaID)
        {
            Type = type;

            MediaID = mediaID;
            OwnerID = ownerID;

            VKString = $"{type.ToString().ToLowerInvariant()}{ownerID}_{mediaID}";
        }
    }

    public enum VKAttachementType
    {
        Photo,
        Doc,
    }
}
