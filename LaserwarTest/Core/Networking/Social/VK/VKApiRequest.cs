using LaserwarTest.Core.Networking.Server.Requests;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace LaserwarTest.Core.Networking.Social.VK
{
    /// <summary>
    /// Выполняет запрос к серверу API ВКонтакте
    /// </summary>
    public sealed class VKApiRequest
    {
        public string RequestUri { get; }

        public VKApiRequest(string requestUri)
        {
            RequestUri = requestUri;
        }

        public async Task<T> Execute<T>() 
            where T : VKApiResponse
        {
            GetStringRequest request = await GetStringRequest.Execute(RequestUri);
            switch (request.Result)
            {
                case RequestResult.NoNetworkConnection:
                    throw new VKApiException("Нет подключения к интернету");

                case RequestResult.Error:
                    throw new VKApiException("Не удалось загрузить данные");

                case RequestResult.NoResponse:
                    throw new VKApiException("Удаленыый сервер не отвечает");

                case RequestResult.Cancelled:
                    throw new VKApiException("Запрос отменен");
            }

            return JsonConvert.DeserializeObject<T>(request.Response);
        }

        public async Task<T> ExecuteUpload<T>(byte[] contentBytes, string contentName, string fileName)
            where T : VKApiResponse
        {
            PostFileRequest request = await PostFileRequest.Execute(RequestUri, contentBytes, contentName, fileName);
            switch (request.Result)
            {
                case RequestResult.NoNetworkConnection:
                    throw new VKApiException("Нет подключения к интернету");

                case RequestResult.Error:
                    throw new VKApiException("Не удалось загрузить данные");

                case RequestResult.NoResponse:
                    throw new VKApiException("Удаленыый сервер не отвечает");

                case RequestResult.Cancelled:
                    throw new VKApiException("Запрос отменен");
            }

            return JsonConvert.DeserializeObject<T>(request.Response);
        }
    }
}
