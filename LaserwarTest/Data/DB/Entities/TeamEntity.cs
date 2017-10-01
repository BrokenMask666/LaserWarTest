using LaserwarTest.Core.Data.DB;
using SQLite.Net.Attributes;

namespace LaserwarTest.Data.DB.Entities
{
    /// <summary>
    /// Представляет хранящуюся в БД информацию о команде игроков
    /// </summary>
    [Table("Teams")]
    public sealed class TeamEntity : ISQLiteDBEntity<int>
    {
        #region ColumnNames

        public const string ClmnNm_ID = "id";

        public const string ClmnNm_GameID = "game_id";

        public const string ClmnNm_Name = "name";

        #endregion ColumnNames

        /// <summary>
        /// Идентификатор
        /// </summary>
        [PrimaryKey, AutoIncrement, Column(ClmnNm_ID)]
        public int ID { set; get; }

        /// <summary>
        /// Идентифкатор игры, в которой принимала участие команда
        /// </summary>
        [Column(ClmnNm_GameID)]
        public int GameID { set; get; }

        /// <summary>
        /// Название команды
        /// </summary>
        [Column(ClmnNm_Name)]
        public string Name { set; get; }
    }
}
