using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LaserwarTest.Core.Networking.Social.VK.Photos
{
    /// <summary>
    /// Предоставляет единый доступ к API-функциям фотографий пользователя
    /// </summary>
    public sealed class VKPhotosApi : VKApiBase
    {
        public async Task<VKPhotosGetUploadServerResponse> GetWallUploadServer(long groupID)
        {
            var response = await new VKApiRequest($"https://api.vk.com/method/photos.getWallUploadServer?" +
                    $"access_token={ApiInfo.AccessToken}" +
                    $"&group_id={groupID}" +
                    $"&v={ApiInfo.APIVersion}")
                    .Execute<VKPhotosGetUploadServerResponse>();

            if (response.Error != null)
            {
                switch (response.Error.Code)
                {
                    default:
                        throw new VKApiException($"Code: {response.Error.Code}\nMessage: {response.Error.Message}");
                }
            }

            return response;
        }

        public async Task<VKPhotosGetUploadServerResponse> GetUploadServer(long albumID, long groupID = 0)
        {
            var response = await new VKApiRequest($"https://api.vk.com/method/photos.getUploadServer?" +
                    $"access_token={ApiInfo.AccessToken}" +
                    $"need_system=1" +
                    $"&album_id={albumID}" +
                    ((groupID != 0) ? $"&group_id={groupID}" : "") +
                    $"&v={ApiInfo.APIVersion}")
                    .Execute<VKPhotosGetUploadServerResponse>();

            if (response.Error != null)
            {
                switch (response.Error.Code)
                {
                    default:
                        throw new VKApiException($"Code: {response.Error.Code}\nMessage: {response.Error.Message}");
                }
            }

            return response;
        }

        public async Task<VKPhotosUploadResponseDetails> UploadWallPhoto(StorageFile file, string uploadUrl)
        {
            var request = new VKApiRequest($"{uploadUrl}");

            byte[] fileBytes = null;
            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (var dataReader = new DataReader(stream))
                {
                    await dataReader.LoadAsync((uint)stream.Size);
                    dataReader.ReadBytes(fileBytes);
                }
            }

            var response = await request.ExecuteUpload<VKPhotosUploadResponseDetails>(fileBytes, "photo", file.Name);
            if (response.Error != null)
            {
                switch (response.Error.Code)
                {
                    default:
                        throw new VKApiException($"Code: {response.Error.Code}\nMessage: {response.Error.Message}");
                }
            }

            return response;
        }

        public async Task<VKPhotoInfo> SaveWallPhoto(long albumID, long userID, long groupID, string photo, long server, string hash)
        {
            var response = await new VKApiRequest($"https://api.vk.com/method/photos.saveWallPhoto?" +
                    $"access_token={ApiInfo.AccessToken}" +
                    $"&album_id={albumID}" +
                    ((userID != 0) ? $"&user_id={userID}" : "") +
                    ((groupID != 0) ? $"&group_id={groupID}" : "") +
                    //$"&photo={photo.Substring(1, photo.Length - 2)}" +
                    $"&photo={photo.Substring(1, photo.Length - 2)}" +
                    $"&server={server}" +
                    $"&hash={hash}" +
                    $"&v={ApiInfo.APIVersion}")
                    .Execute<VKPhotoInfo>();

            if (response.Error != null)
            {
                switch (response.Error.Code)
                {
                    case 114:
                        throw new VKApiException($"Недопустимый идентификатор альбома");

                    case 118:
                        throw new VKApiException($"Недопустимый сервер");

                    case 121:
                        throw new VKApiException($"Неверный хэш");

                    default:
                        throw new VKApiException($"Code: {response.Error.Code}\nMessage: {response.Error.Message}");
                }
            }

            return response;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKPhotosGetUploadServerResponse : VKApiResponse
    {
        [JsonProperty("response")]
        public VKPhotosGetUploadServerResponseDetails Response { set; get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKPhotosGetUploadServerResponseDetails
    {
        [JsonProperty("upload_url")]
        public string UploadUrl { set; get; }

        [JsonProperty("album_id")]
        public long AlbumID { set; get; }

        [JsonProperty("user_id")]
        public long UserId { set; get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKPhotosUploadResponseDetails : VKApiResponse
    {
        [JsonProperty("server")]
        public long Server { set; get; }

        [JsonProperty("photo")]
        public string Photo { set; get; }

        [JsonProperty("hash")]
        public string Hash { set; get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKPhotoInfo : VKApiResponse
    {
        [JsonProperty("id")]
        public long ID { set; get; }

        [JsonProperty("album_id")]
        public long AlbumID { set; get; }
        [JsonProperty("owner_id")]
        public long OwnerID { set; get; }

        [JsonProperty("user_id")]
        public long UserId { set; get; }
    }
}
