using LaserwarTest.Core.Networking.Downloading;
using LaserwarTest.Core.Networking.Server.Requests;
using LaserwarTest.Data.DB;
using LaserwarTest.Data.DB.Entities;
using LaserwarTest.Data.Server.Requests.Json;
using LaserwarTest.Data.Server.Requests.Xml;
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

            UpdateGameDataFilesInfo();
            Loaded();
        }

        private void UpdateGameDataFilesInfo()
        {
            if (_gameDataUrls.Count > 0 && !_gameDataUrls.Any(x => x.Downloaded))
            {
                IEnumerable<string> gameDataFilesInfos = _gameDataUrls.Select(x =>
                    $"Имя файла: {x.Name}\nАдрес файла: {x.URL}\nСтатус: {((x.Downloaded) ? "Загружен" : "Не загружен")}\n______________________________");

                GameDataFilesInfo = string.Join("\n", gameDataFilesInfos);
                return;
            }

            GameDataFilesInfo = "";
        }

        private async Task<GetStringRequest> HandleRequest(string url, bool ignoreErrors = false)
        {
            GetStringRequest request = await GetStringRequest.Execute(url);
            switch (request.Result)
            {
                case GetStringRequestResult.NoNetworkConnection:
                    if (!ignoreErrors)
                        ShowError("Нет подключения к интернету", "Проверьте подключение к интернету и повторите попытку");

                    return null;

                case GetStringRequestResult.Error:
                    if (!ignoreErrors)
                        ShowError("Не удалось загрузить данные");

                    return null;

                case GetStringRequestResult.NoResponse:
                    if (!ignoreErrors)
                        ShowError("Удаленыый сервер не отвечает");

                    return null;

                case GetStringRequestResult.Cancelled:
                    return null;
            }

            return request;
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

            GetStringRequest request = await HandleRequest(FileUrl);
            if (request == null)
            {
                Status = "Ошибка получения данных";
                Loaded();

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

            _gameDataUrls = serverResponse.Games;

            LocalDB localDB = DBManager.GetLocalDB();
            await localDB.Clear();

            await localDB.Sounds.InsertAll(serverResponse.Sounds);
            await localDB.GameDataUrls.InsertAll(serverResponse.Games);

            bool hasErrors = false;
            foreach (var gameDataUrl in serverResponse.Games)
            {
                FileUrl = gameDataUrl.URL;
                Status = "Файл загружается...";
                await Task.Delay(60);

                GetStringRequest xmlRequest = await HandleRequest(gameDataUrl.URL, ignoreErrors: true);
                if (xmlRequest == null)
                {
                    hasErrors = true;
                    Status = "Ошибка загрузки файла";
                    continue;
                }

                XmlGameData gameData = XmlGameData.FromString(xmlRequest.Response);
                await HandleXml(gameData, gameDataUrl);
            }

            if (hasErrors)
            {
                ShowError("Во время загрузки некоторых файлов возникли ошибки.\nПопробуйте загрузить их вручную");
            }

            UpdateGameDataFilesInfo();
        }

        private async Task HandleXml(XmlGameData gameData, GameDataUrlEntity gameDataUrlEntity)
        {
            Status = "Файл обрабатывается...";

            GameEntity gameEntity = new GameEntity()
            {
                Name = gameData.Name,
                Date = gameData.Date,
            };

            LocalDB localDB = DBManager.GetLocalDB();
            await localDB.Games.Insert(gameEntity);

            List<PlayerEntity> playerEntities = new List<PlayerEntity>();
            foreach (var teamData in gameData.Teams)
            {
                TeamEntity teamEntity = new TeamEntity()
                {
                    GameID = gameEntity.ID,

                    Name = teamData.Name,
                };

                await localDB.Teams.Insert(teamEntity);

                foreach (var playerData in teamData.Players)
                {
                    playerEntities.Add(new PlayerEntity()
                    {
                        TeamID = teamEntity.ID,

                        Name = playerData.Name,
                        Rating = playerData.Rating,
                        Accuracy = playerData.Accuracy,
                        Shots = playerData.Shots,
                    });
                }
            }

            await localDB.Players.InsertAll(playerEntities);

            gameDataUrlEntity.Downloaded = true;
            await localDB.GameDataUrls.Update(gameDataUrlEntity);
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

            if (gameDataUrl.Downloaded)
            {
                ShowError("Указанный файл уже загружен", "Файл уже загружен");
                return;
            }

            Status = "Файл загружается...";
            await Loading(60);

            GetStringRequest request = await HandleRequest(gameDataUrl.URL);
            if (request == null)
            {
                Status = "Не удалось загрузить файл";
                Loaded();
                return;
            }

            XmlGameData gameData = XmlGameData.FromString(request.Response);
            await HandleXml(gameData, gameDataUrl);

            UpdateGameDataFilesInfo();

            Status = "Файл успешно загружен";
            Loaded();
        }
    }
}
