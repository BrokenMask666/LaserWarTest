using LaserwarTest.Core.Data.DB.Versioning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace LaserwarTest.Core.Data.DB
{
    /// <summary>
    /// Абстрактный класс, представляющий базовый механизм управления базой данных
    /// </summary>
    public abstract class SQLiteDB
    {
        /// <summary>
        /// Получает доступ к управлению соединением с локальной базой данных
        /// </summary>
        public SQLiteDBConnectionProvider Connection { get; }

        /// <summary>
        /// Получает доступный только для чтения набор представлений таблиц в базе данных
        /// </summary>
        public IReadOnlyCollection<ISQLiteDBTable> Tables { private set; get; }

        public SQLiteDB(string dbPath)
        {
            Connection = new SQLiteDBConnectionProvider(dbPath);
        }

        protected void SetTables(params ISQLiteDBTable[] tables)
        {
            Tables = new ReadOnlyCollection<ISQLiteDBTable>(tables);
        }

        /// <summary>
        /// Получает таблицу, хранящую данные указанной сущности.
        /// Выбрасывает <see cref="InvalidOperationException"/>, если такой таблицы не обнаружено либо она не имеет тип SQLiteDBTable для TEntity
        /// </summary>
        /// <typeparam name="TEntity">Тип сущности</typeparam>
        /// <returns></returns>
        public SQLiteDBTable<TEntity> GetTable<TEntity>() where TEntity : class, ISQLiteDBEntity, new()
        {
            string tableName = SQLiteDBHelper.GetTableName<TEntity>();
            return Tables.FirstOrDefault(x => x.Name == tableName) as SQLiteDBTable<TEntity>
                ?? throw new InvalidOperationException($"Not such table with name '{tableName}'");
        }

        private async Task<bool> IsExists()
        {
            try { await ApplicationData.Current.LocalFolder.GetFileAsync(Connection.Path); }
            catch (FileNotFoundException) { return false; }

            return true;
        }

        /// <summary>
        /// Производит инициализацию БД.
        /// Если база не существовала, будет произведено создание.
        /// Если база уже была создана, будет произведено сравнение версий между локальной базой на устройстве и версии базы в приложении.
        /// Если локальная версия базы ниже, будет запущен процесс обновления
        /// </summary>
        /// <param name="ignoreUpdate">Указывает, что обновление текущей локальной базы не должно выполняться</param>
        /// <returns></returns>
        public async Task Init(bool ignoreUpdate = false)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            bool notExists = !(await IsExists());

            // Database cannot be created if subdirectories doesn't exist
            // So we need to check its path (relative/platform-independent)
            string dbRelDirectory = Path.GetDirectoryName(DBInfoLocal.DB_PATH);
            if (!string.IsNullOrWhiteSpace(dbRelDirectory))
                await localFolder.CreateFolderAsync(dbRelDirectory, CreationCollisionOption.OpenIfExists);

            await Connection.ExecuteAsyncAction(async (asyncConn) =>
            {
                foreach (ISQLiteDBTable table in Tables)
                    await table.CreateOrUpdate(asyncConn);
            });

            Debug.WriteLine($"SQLiteDB -> Init: \n\tFullPath = {Connection.FullPath}");

            DBInfo info = await DBInfo.GetInfo();
            if (notExists)
            {
                DBInfoLocal localInfo = await DBInfoLocal.Create(info.InstalledVersionNumber);

                Debug.WriteLine($"SQLiteDB -> OnCreate");
                await OnCreate(info);
            }
            else if (!ignoreUpdate)
            {
                DBInfoLocal localInfo = await DBInfoLocal.Get();
                if (localInfo.InstalledVersionNumber < info.InstalledVersionNumber)
                {
                    Debug.WriteLine($"SQLiteDB -> OnUpdate");
                    await OnUpdate(info, localInfo);
                }
            }
        }

        /// <summary>
        /// Полность удаляет файл базы данных с устройства,
        /// если она существует
        /// </summary>
        /// <returns></returns>
        public async Task DeleteIfExists()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(Connection.Path);
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (FileNotFoundException) {  }
            finally { await DBInfoLocal.Delete(); }
        }

        protected virtual Task OnCreate(DBInfo info) { return Task.Delay(0); }
        protected virtual Task OnUpdate(DBInfo info, DBInfoLocal infoLocal) { return Task.Delay(0); }
    }
}
