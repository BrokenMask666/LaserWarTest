using LaserwarTest.Core.Data.DB;
using SQLite.Net.Attributes;

namespace LaserwarTest.Data.DB.Entities
{
    /// <summary>
    /// Представляет хранящуюся в БД информацию об игроке
    /// </summary>
    [Table("Teams")]
    public sealed class PlayerEntity : ISQLiteDBEntity<int>
    {
        #region ColumnNames

        public const string ClmnNm_ID = "id";

        public const string ClmnNm_TeamID = "team_id";

        public const string ClmnNm_Name = "name";

        public const string ClmnNm_Rating = "rating";
        public const string ClmnNm_Accuracy = "accuracy";
        public const string ClmnNm_Shots = "shots";

        #endregion ColumnNames

        /// <summary>
        /// Идентификатор
        /// </summary>
        [PrimaryKey, AutoIncrement, Column(ClmnNm_ID)]
        public int ID { set; get; }

        /// <summary>
        /// Идентифкатор команды, в которой принимал участие игрок
        /// </summary>
        [Column(ClmnNm_TeamID)]
        public int TeamID { set; get; }

        /// <summary>
        /// Имя игрока
        /// </summary>
        [Column(ClmnNm_Name)]
        public string Name { set; get; }

        /// <summary>
        /// Рейтинг
        /// </summary>
        [Column(ClmnNm_Rating)]
        public int Rating { set; get; }
        /// <summary>
        /// Точность попаданий
        /// </summary>
        [Column(ClmnNm_Accuracy)]
        public double Accuracy { set; get; }
        /// <summary>
        /// Количество сделанных выстрелов
        /// </summary>
        [Column(ClmnNm_Shots)]
        public int Shots { set; get; }
    }
}
