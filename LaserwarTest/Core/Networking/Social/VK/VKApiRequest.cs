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
                case GetStringRequestResult.NoNetworkConnection:
                    throw new VKApiException("Нет подключения к интернету");

                case GetStringRequestResult.Error:
                    throw new VKApiException("Не удалось загрузить данные");

                case GetStringRequestResult.NoResponse:
                    throw new VKApiException("Удаленыый сервер не отвечает");

                case GetStringRequestResult.Cancelled:
                    throw new VKApiException("Запрос отменен");
            }

            return JsonConvert.DeserializeObject<T>(request.Response);
        }
    }
}
