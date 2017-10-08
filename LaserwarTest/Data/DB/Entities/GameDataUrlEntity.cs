using LaserwarTest.Core.Data.DB;
using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace LaserwarTest.Data.DB.Entities
{
    /// <summary>
    /// Представляет хранимую в БД информацию о загружаемых звуках
    /// </summary>
    [Table("GameDataUrls")]
    [JsonObject(MemberSerialization.OptIn)]
    public class GameDataUrlEntity : ISQLiteDBEntity<int>
    {
        string _url;

        #region ColumnNames

        public const string ClmnNm_ID = "id";

        public const string ClmnNm_URL = "url";

        public const string ClmnNm_Downloaded = "downloaded";

        #endregion ColumnNames

        /// <summary>
        /// Идентификатор
        /// </summary>
        [PrimaryKey, AutoIncrement, Column(ClmnNm_ID)]
        public int ID { set; get; }

        /// <summary>
        /// Адрес файла на сервере
        /// </summary>
        [Column(ClmnNm_URL)]
        [JsonProperty("url")]
        public string URL
        {
            set
            {
                _url = value;
                Name = _url.Substring(_url.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
            }
            get { return _url; }
        }

        /// <summary>
        /// Получает имя файла
        /// </summary>
        [Ignore]
        public string Name { private set; get; }

        /// <summary>
        /// Указывает, что объект был загружен на устройство
        /// </summary>
        [Column(ClmnNm_Downloaded)]
        public bool Downloaded { set; get; }
    }
}
