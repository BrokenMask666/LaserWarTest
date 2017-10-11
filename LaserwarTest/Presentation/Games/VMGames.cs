using LaserwarTest.Core.Data.DB;
using LaserwarTest.Data.DB;
using LaserwarTest.Data.DB.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Presentation.Games
{
    /// <summary>
    /// Модель представления страницы с играми
    /// </summary>
    public class VMGames : BaseViewModel
    {
        ObservableCollection<Game> _items;

        public ObservableCollection<Game> Items
        {
            private set => SetProperty(ref _items, value);
            get => _items;
        }

        public async Task Load()
        {
            await Loading(0);

            LocalDB localDB = DBManager.GetLocalDB();

            string getPlayersCountCommandSql =
                $@"select
                       {GameEntity.ClmnNm_ID} as GameID,
                       (select count(*)
                        from {localDB.Players.Name}
                        where {PlayerEntity.ClmnNm_TeamID} in
                            (select {TeamEntity.ClmnNm_ID}
                             from {localDB.Teams.Name}
                             where {TeamEntity.ClmnNm_GameID} == G.{GameEntity.ClmnNm_ID}
                            )
                       ) as PlayersCount
                   from {localDB.Games.Name} G";

            var result = await localDB.Connection.ExecuteAsyncAction(async db =>
                await db.QueryAsync<PlayersOnGameCountQueryResult>(getPlayersCountCommandSql));

            var playersCountByGameID = result.ToDictionary(
                keySelector: (x) => x.GameID,
                elementSelector: (x) => x.PlayersCount);

            var games = await localDB.Games.GetAll();
            Items = new ObservableCollection<Game>(games.Select((gameData) =>
                new Game(gameData, playersCountByGameID[gameData.ID])));

            Loaded();
        }
    }

    public class PlayersOnGameCountQueryResult : ISQLiteDBEntity
    {
        public int GameID { set; get; }
        public int PlayersCount { set; get; }
    }
}
