using LaserwarTest.Core.Data.DB;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Data.DB.Entities
{
    /// <summary>
    /// Представляет хранящуюся в БД информацию об игре
    /// </summary>
    [Table("Games")]
    public sealed class GameEntity : ISQLiteDBEntity<int>
    {
        #region ColumnNames

        public const string ClmnNm_ID = "id";

        public const string ClmnNm_Name = "name";
        public const string ClmnNm_Date = "date";

        #endregion ColumnNames

        /// <summary>
        /// Идентификатор
        /// </summary>
        [PrimaryKey, AutoIncrement, Column(ClmnNm_ID)]
        public int ID { set; get; }

        /// <summary>
        /// Название игры
        /// </summary>
        [Column(ClmnNm_Name)]
        public string Name { set; get; }

        /// <summary>
        /// Дата проведения игры
        /// </summary>
        [Column(ClmnNm_Date)]
        public DateTime Date { set; get; }
    }
}
