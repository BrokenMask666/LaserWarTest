using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LaserwarTest.Core.Networking.Social.VK.Docs
{
    // <summary>
    /// Предоставляет единый доступ к API-функциям документов пользователя
    /// </summary>
    public sealed class VKDocsApi : VKApiBase
    {
        public async Task<VKDocsGetUploadServerResponse> GetWallUploadServer(long groupID)
        {
            var response = await new VKApiRequest($"https://api.vk.com/method/docs.getWallUploadServer?" +
                    $"access_token={ApiInfo.AccessToken}" +
                    $"&group_id={groupID}" +
                    $"&v={ApiInfo.APIVersion}")
                    .Execute<VKDocsGetUploadServerResponse>();

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

        public async Task<VKDocsUploadResponseDetails> UploadWallFile(StorageFile file, string uploadUrl)
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

            var response = await request.ExecuteUpload<VKDocsUploadResponseDetails>(fileBytes, "file", file.Name);
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

        public async Task<VKDocsSaveResponse> SaveWallFile(string file, string title = null, string tags = null)
        {
            var response = await new VKApiRequest($"https://api.vk.com/method/docs.save?" +
                    $"access_token={ApiInfo.AccessToken}" +
                    $"&file={file}" +
                    ((title != null) ? $"&title={title}" : "") +
                    ((tags != null) ? $"&tags={tags}" : "") +
                    $"&v={ApiInfo.APIVersion}")
                    .Execute<VKDocsSaveResponse>();

            if (response.Error != null)
            {
                switch (response.Error.Code)
                {
                    case 105:
                        throw new VKApiException($"Невозможно сохранить файл");

                    default:
                        throw new VKApiException($"Code: {response.Error.Code}\nMessage: {response.Error.Message}");
                }
            }

            return response;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKDocsGetUploadServerResponse : VKApiResponse
    {
        [JsonProperty("response")]
        public VKDocsGetUploadServerResponseDetails Response { set; get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKDocsGetUploadServerResponseDetails
    {
        [JsonProperty("upload_url")]
        public string UploadUrl { set; get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKDocsUploadResponseDetails : VKApiResponse
    {
        [JsonProperty("file")]
        public string File { set; get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKDocsSaveResponse : VKApiResponse
    {
        [JsonProperty("response")]
        public List<VKDocsSaveResponseDetails> Response { set; get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class VKDocsSaveResponseDetails
    {
        [JsonProperty("id")]
        public long ID { set; get; }
        [JsonProperty("owner_id")]
        public long OwnerID { set; get; }

        [JsonProperty("title")]
        public string Title { set; get; }

        [JsonProperty("size")]
        public long Size { set; get; }

        [JsonProperty("ext")]
        public string Extention { set; get; }

        [JsonProperty("url")]
        public string Url { set; get; }
    }
}
