using LaserwarTest.Core.Data.DB.Versioning.Xml;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace LaserwarTest.Core.Data.DB.Versioning
{
    /// <summary>
    /// Предоставляет информацию о базе данных, установленной на локальном устройстве
    /// </summary>
    public sealed class DBInfoLocal
    {
        /// <summary>
        /// Получает путь к локальному файлу БД относительно локального расположения файлового хранилища приложения
        /// </summary>
        public const string DB_PATH = "Data\\Database.db";

        private const string CONFIG_PATH = "Data\\DBInfo.local.dbconfig";

        /// <summary>
        /// Получает файл, с которым связан данный объект
        /// </summary>
        StorageFile LinkedFile { get; }
        /// <summary>
        /// Получает xml-содержимое связанного файла
        /// </summary>
        XmlDBInfoLocal Data { get; }

        /// <summary>
        /// Получает версию установленной базы
        /// </summary>
        public double InstalledVersionNumber { get { return Data.InstalledVersionNumber; } }

        private DBInfoLocal(StorageFile file, XmlDBInfoLocal xmlData)
        {
            LinkedFile = file;
            Data = xmlData;
        }

        /// <summary>
        /// Обновляет данные о текущей версии базы данных и записывает данные в файл
        /// </summary>
        /// <param name="version">Новая версия БД. Должна быть больше имеющейся, в противном случае будет выброшено исключение <see cref="ArgumentException"/></param>
        /// <returns></returns>
        public async Task WriteVersion(double version)
        {
            if (version <= Data.InstalledVersionNumber)
                throw new ArgumentException("New version must more than exists");

            Data.InstalledVersionNumber = version;

            using (var stream = await LinkedFile.OpenStreamForWriteAsync())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlDBInfoLocal));
                xmlSerializer.Serialize(stream, Data);
            }
        }

        public static async Task<bool> IsExists()
        {
            try { await ApplicationData.Current.LocalFolder.GetFileAsync(CONFIG_PATH); }
            catch (FileNotFoundException) { return false; }

            return true;
        }

        /// <summary>
        /// Создает новый локальный файл версии БД и возвращает объект представления.
        /// Вызывает исключение, если файл существует
        /// </summary>
        /// <param name="version">Версия БД</param>
        /// <returns></returns>
        public static async Task<DBInfoLocal> Create(double version)
        {
            return await Create(version, CreationCollisionOption.FailIfExists);
        }

        /// <summary>
        /// Создает новый или заменяет локальный файл версии БД и возвращает объект представления
        /// </summary>
        /// <param name="version">Версия БД</param>
        /// <returns></returns>
        public static async Task<DBInfoLocal> CreateOrReplace(double version)
        {
            return await Create(version, CreationCollisionOption.ReplaceExisting);
        }

        private static async Task<DBInfoLocal> Create(double version, CreationCollisionOption collisionOption)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CONFIG_PATH, collisionOption);

            DBInfoLocal dbInfo = new DBInfoLocal(file, new XmlDBInfoLocal());
            await dbInfo.WriteVersion(version);

            return dbInfo;
        }

        /// <summary>
        /// Получает локальный файл версии БД и возвращает объект представления.
        /// Вызывает исключение <see cref="FileNotFoundException"/>, если файл отсутствуетт
        /// </summary>
        /// <returns></returns>
        public static async Task<DBInfoLocal> Get()
        {
            StorageFile file = null;
            try { file = await ApplicationData.Current.LocalFolder.GetFileAsync(CONFIG_PATH); }
            catch (FileNotFoundException ex) { throw new FileNotFoundException("Local db version file not found", ex); }

            using (var stream = await file.OpenStreamForReadAsync())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlDBInfoLocal));
                return new DBInfoLocal(file, xmlSerializer.Deserialize(stream) as XmlDBInfoLocal);
            }
        }

        /// <summary>
        /// Удаляет локальный файл версии БД, если он имеется
        /// </summary>
        /// <returns></returns>
        public static async Task Delete()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(CONFIG_PATH);
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (FileNotFoundException) { }
        }
    }
}
