using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace LaserwarTest.Core.Networking.Server.Requests
{
    /// <summary>
    /// Предствляет Get-запрос серверу с возвращаемым результатом в виде строки
    /// </summary>
    public class GetStringRequest
    {
        /// <summary>
        /// Получает ответ сервера в виде строки.
        /// NULL в случае отсутсвия ответа или ошибки
        /// </summary>
        public string Response { get; }

        /// <summary>
        /// Получает результат выполнения запроса
        /// </summary>
        public GetStringRequestResult Result { get; }

        protected GetStringRequest(GetStringRequestResult result, string response)
        {
            Result = result;
            Response = response;
        }

        /// <summary>
        /// Выполняет Get-запрос по указанному адресу и возвращает результат в виде строки
        /// </summary>
        /// <param name="requestUri">Адрес ресурса</param>
        /// <returns></returns>
        public static async Task<GetStringRequest> Execute(string requestUri)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return new GetStringRequest(GetStringRequestResult.NoNetworkConnection, null);

            HttpClient client = new HttpClient();
            string response = null;
            GetStringRequestResult result = GetStringRequestResult.Success;

            try { response = await client.GetStringAsync(requestUri); }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    result = GetStringRequestResult.Cancelled;
                else
                    result = GetStringRequestResult.NoResponse;
            }
            catch (Exception)
            {
                result = GetStringRequestResult.Error;
            }

            return new GetStringRequest(result, response);
        }
    }

    /// <summary>
    /// Перечисление возможных результатов выполнения Get-запроса
    /// </summary>
    public enum GetStringRequestResult
    {
        /// <summary>
        /// Запрос был успешно выполнен
        /// </summary>
        Success = 0,
        /// <summary>
        /// Отсутствует подключение к интернету
        /// </summary>
        NoNetworkConnection,
        /// <summary>
        /// Сервер не отвечает или время ожидания ответа превысило допустимое значение
        /// </summary>
        NoResponse,
        /// <summary>
        /// Запрос был отменен
        /// </summary>
        Cancelled,
        /// <summary>
        /// В ходе операции вознакла ошибка
        /// </summary>
        Error
    }
}
