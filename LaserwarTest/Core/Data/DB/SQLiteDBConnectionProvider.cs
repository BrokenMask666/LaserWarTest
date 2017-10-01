using LaserwarTest.Commons.Async;
using SQLite.Net;
using SQLite.Net.Async;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace LaserwarTest.Core.Data.DB
{
    /// <summary>
    /// Обеспечивает связь с локальной базой данных 
    /// </summary>
    public class SQLiteDBConnectionProvider
    {
        private static readonly AsyncLock _locker = new AsyncLock();

        SQLite.Net.Platform.WinRT.SQLitePlatformWinRT Platform { get; } = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();

        /// <summary>
        /// Получает путь к базе данных в локальном хранилище устройства
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Получает полный путь к базе данных
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// Создает новый объект
        /// </summary>
        /// <param name="dbPath">Путь к базе данных</param>
        public SQLiteDBConnectionProvider(string dbPath)
        {
            Path = dbPath;
            FullPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, dbPath);
        }

        public SQLiteConnection Open()
        {
            return new SQLiteConnection(Platform, FullPath, storeDateTimeAsTicks: true);
        }

        private SQLiteAsyncConnection OpenAsync()
        {
            return new SQLiteAsyncConnection(() => new SQLiteConnectionWithLock(Platform, new SQLiteConnectionString(FullPath, storeDateTimeAsTicks: true)));
        }

        public async Task ExecuteAsyncAction(Func<SQLiteAsyncConnection, Task> asyncAction)
        {
            using (await _locker.LockAsync().ConfigureAwait(false))
                await asyncAction(OpenAsync());
        }

        public async Task<TEntity> ExecuteAsyncAction<TEntity>(Func<SQLiteAsyncConnection, Task<TEntity>> asyncAction) where TEntity : ISQLiteDBEntity
        {
            using (await _locker.LockAsync().ConfigureAwait(false))
                return await asyncAction(OpenAsync());
        }

        public async Task<List<TEntity>> ExecuteAsyncAction<TEntity>(Func<SQLiteAsyncConnection, Task<List<TEntity>>> asyncAction) where TEntity : ISQLiteDBEntity
        {
            using (await _locker.LockAsync().ConfigureAwait(false))
                return await asyncAction(OpenAsync());
        }
    }
}
