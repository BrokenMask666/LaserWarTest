using LaserwarTest.Core.Data.DB;
using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace LaserwarTest.Data.DB.Entities
{
    /// <summary>
    /// Представляет хранимую в БД информацию о загружаемых звуках
    /// </summary>
    [Table("Sounds")]
    [JsonObject(MemberSerialization.OptIn)]
    public class SoundEntity : ISQLiteDBEntity<int>
    {
        #region ColumnNames

        public const string ClmnNm_ID = "id";

        public const string ClmnNm_Name = "name";

        public const string ClmnNm_URL = "url";
        public const string ClmnNm_Size = "size";

        public const string ClmnNm_Downloaded = "downloaded";

        #endregion ColumnNames

        /// <summary>
        /// Идентификатор
        /// </summary>
        [PrimaryKey, AutoIncrement, Column(ClmnNm_ID)]
        public int ID { set; get; }

        /// <summary>
        /// Название звукового файла
        /// </summary>
        [Column(ClmnNm_Name)]
        [JsonProperty("name")]
        public string Name { set; get; }

        /// <summary>
        /// Адрес файла на сервере
        /// </summary>
        [Column(ClmnNm_URL)]
        [JsonProperty("url")]
        public string URL { set; get; }

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        [Column(ClmnNm_Size)]
        public int Size { set; get; }

        /// <summary>
        /// Указывает, что объект был загружен на устройство
        /// </summary>
        [Column(ClmnNm_Size)]
        [JsonProperty("size")]
        public bool Downloaded { set; get; }
    }
}
