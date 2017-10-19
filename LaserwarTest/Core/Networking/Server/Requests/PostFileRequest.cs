using System;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace LaserwarTest.Core.Networking.Server.Requests
{
    /// <summary>
    /// Предствляет POST-запрос серверу с возвращаемым результатом в виде строки
    /// </summary>
    public class PostFileRequest
    {
        /// <summary>
        /// Получает ответ сервера в виде строки.
        /// NULL в случае отсутсвия ответа или ошибки
        /// </summary>
        public string Response { get; }

        /// <summary>
        /// Получает результат выполнения запроса
        /// </summary>
        public RequestResult Result { get; }

        protected PostFileRequest(RequestResult result, string response)
        {
            Result = result;
            Response = response;
        }

        /// <summary>
        /// Выполняет Get-запрос по указанному адресу и возвращает результат в виде строки
        /// </summary>
        /// <param name="requestUri">Адрес ресурса</param>
        /// <returns></returns>
        public static async Task<PostFileRequest> Execute(string requestUri, byte[] contentBytes, string contentName, string fileName)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return new PostFileRequest(RequestResult.NoNetworkConnection, null);

            HttpClient client = new HttpClient();
            string response = null;
            RequestResult result = RequestResult.Success;

            MultipartContent content = new MultipartFormDataContent();
            var file = new ByteArrayContent(contentBytes);
            file.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = contentName,
                FileName = fileName,
            };
            content.Add(file);

            try
            {
                HttpResponseMessage responseMessage = await client.PostAsync(requestUri, content);
                responseMessage.EnsureSuccessStatusCode();

                byte[] bytes = await responseMessage.Content.ReadAsByteArrayAsync();
                //response = Encoding.ASCII.GetString(bytes);

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding encoding = Encoding.GetEncoding("windows-1251");
                response = encoding.GetString(bytes, 0, bytes.Length);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    result = RequestResult.Cancelled;
                else
                    result = RequestResult.NoResponse;
            }
            catch (Exception ex)
            {
                result = RequestResult.Error;
            }

            return new PostFileRequest(result, response);
        }
    }
}
