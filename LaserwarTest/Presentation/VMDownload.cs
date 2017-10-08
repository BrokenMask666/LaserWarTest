using LaserwarTest.Core.Networking.Downloading;
using LaserwarTest.Core.Networking.Server.Requests;
using LaserwarTest.Data.DB;
using LaserwarTest.Data.DB.Entities;
using LaserwarTest.Data.Server.Requests.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaserwarTest.Presentation
{
    /// <summary>
    /// Модель представления страницы загрузки
    /// </summary>
    public sealed class VMDownload : BaseViewModel
    {
        string _fileUrl = "https://laserwar.com/testtask/get?dataType=Json";
        string _status;
        string _jsonContent;

        List<GameDataUrlEntity> _gameDataUrls = new List<GameDataUrlEntity>();
        string _gameDataFilesInfo;

        Downloader Downloader => Downloader.GetCurrent();

        public string FileUrl
        {
            set => SetProperty(ref _fileUrl, value.Trim());
            get => _fileUrl;
        }

        public string Status
        {
            set => SetProperty(ref _status, value);
            get => _status;
        }

        public string JsonContent
        {
            set => SetProperty(ref _jsonContent, value);
            get => _jsonContent;
        }

        public string GameDataFilesInfo
        {
            set => SetProperty(ref _gameDataFilesInfo, value);
            get => _gameDataFilesInfo;
        }

        /// <summary>
        /// Проверяет статус загружаемых объектов
        /// </summary>
        /// <returns></returns>
        public async Task CheckDataDownloadedStatus()
        {
            await Loading(0);

            LocalDB localDB = DBManager.GetLocalDB();
            _gameDataUrls = await localDB.GameDataUrls.GetAll();
            if (_gameDataUrls.Count != 0)
            {
                UpdateGameDataFilesInfo(_gameDataUrls);
            }

            Loaded();
        }

        private void UpdateGameDataFilesInfo(List<GameDataUrlEntity> gameDataUrls)
        {
            IEnumerable<string> gameDataFilesInfos = gameDataUrls.Select(x =>
                $"Имя файла: {x.Name}\nАдрес файла: {x.URL}\nСтатус: {((x.Downloaded) ? "Загружен" : "Не загружен")}\n______________________________");

            GameDataFilesInfo = string.Join("\n", gameDataFilesInfos);
        }

        public async Task DownloadFile()
        {
            if (FileUrl.EndsWith(".xml"))
                await DownloadXml();
            else if (FileUrl.Contains("dataType=Json"))
                await DownloadJson();
            else
                ShowError("Неверный формат адреса", "Адрес не указан или имеет неподдерживаемый формат");
        }

        private async Task DownloadJson()
        {
            if (Downloader.State == DownloaderState.Active)
            {
                ShowError("Выполняются загрузки", "Дождитесь окончания загрузки файлов и повторите попытку");
                return;
            }

            JsonContent = "";
            Status = "Получение данных от сервера...";
            await Loading(60);

            GetStringRequest request = await GetStringRequest.Execute(FileUrl);
            switch (request.Result)
            {
                case GetStringRequestResult.NoNetworkConnection:
                    ShowError("Нет подключения к интернету", "Проверьте подключение к интернету и повторите попытку");
                    return;

                case GetStringRequestResult.Error:
                    ShowError("Не удалось загрузить данные");
                    return;

                case GetStringRequestResult.NoResponse:
                    ShowError("Удаленыый сервер не отвечает");
                    return;

                case GetStringRequestResult.Cancelled:
                    return;
            }

            JsonContent = request.Response;

            JsonServerResponse serverResponse = JsonServerResponse.FromString(JsonContent);
            await HandleJson(serverResponse);

            Status = "Данные получены";
            Loaded();
        }

        private async Task HandleJson(JsonServerResponse serverResponse)
        {
            Status = "Обработка данных...";

            if (!string.IsNullOrWhiteSpace(serverResponse.Error))
            {
                ShowError(serverResponse.Error);
                return;
            }

            LocalDB localDB = DBManager.GetLocalDB();
            await localDB.Clear();

            await localDB.Sounds.InsertAll(serverResponse.Sounds);
            await localDB.GameDataUrls.InsertAll(serverResponse.Games);

            UpdateGameDataFilesInfo(serverResponse.Games);
        }

        private async Task DownloadXml()
        {
            GameDataUrlEntity gameDataUrl = _gameDataUrls.FirstOrDefault(x => x.URL == FileUrl);
            if (gameDataUrl == null)
            {
                ShowError(
                    "Указанный файл не найден",
                    "Невозможно загрузить файл, поскольку данные о файле отсутсвуют.\nПопробуйте загрузить JSON с данными");
                return;
            }
        }
    }
}
