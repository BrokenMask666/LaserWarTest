using SQLite.Net.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LaserwarTest.Core.Data.DB
{
    /// <summary>
    /// Интерфейс для взаимодействия с отдельной таблицей в БД
    /// </summary>
    public interface ISQLiteDBTable
    {
        /// <summary>
        /// Получает имя таблицы в базе данных
        /// </summary>
        string Name { get; }

        Task CreateOrUpdate(SQLiteAsyncConnection asyncConn);

        Task Clear(SQLiteAsyncConnection asyncConn);
    }

    /// <summary>
    /// Представляет базовые методы управления данными в таблице БД
    /// </summary>
    /// <typeparam name="TEntity">Тип, используемый для представления сущности в БД, связанной с данной таблицей</typeparam>
    public abstract class SQLiteDBTable<TEntity> : ISQLiteDBTable
        where TEntity : class, ISQLiteDBEntity, new()
    {
        /// <summary>
        /// База данных, в которой представлена данная таблица
        /// </summary>
        protected SQLiteDB DB { private set; get; }

        /// <summary>
        /// Получает имя таблицы в базе данных
        /// </summary>
        public string Name { get; }

        public SQLiteDBTable(SQLiteDB ownerDB)
        {
            DB = ownerDB;
            Name = SQLiteDBHelper.GetTableName<TEntity>();
        }

        public async Task CreateOrUpdate(SQLiteAsyncConnection asyncConn)
        {
            await asyncConn.CreateTableAsync<TEntity>();
        }

        public async Task Insert(TEntity entity)
        {
            await DB.Connection.ExecuteAsyncAction(async (conn) => { await conn.InsertAsync(entity); });
        }
        public async Task InsertAll(IEnumerable<TEntity> entities)
        {
            await DB.Connection.ExecuteAsyncAction(async (conn) => { await conn.InsertAllAsync(entities); });
        }

        public async Task Update(TEntity entity)
        {
            await DB.Connection.ExecuteAsyncAction(async (conn) => { await conn.UpdateAsync(entity); });
        }
        public async Task UpdateAll(IEnumerable<TEntity> entities)
        {
            await DB.Connection.ExecuteAsyncAction(async (conn) => { await conn.UpdateAllAsync(entities); });
        }

        public async Task<TEntity> Get(Expression<Func<TEntity, bool>> filter = null)
        {
            return await DB.Connection.ExecuteAsyncAction(async (conn) =>
            {
                return await conn.GetAsync(filter);
            });
        }
        public async Task<List<TEntity>> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            return await DB.Connection.ExecuteAsyncAction(async (conn) =>
            {
                var list = await conn.QueryAsync<TEntity>($"select * from {Name}");

                if (filter != null)
                    list = list.Where(filter.Compile()).ToList();

                return list;
            });
        }

        public async Task Delete(TEntity entity)
        {
            await DB.Connection.ExecuteAsyncAction(async (conn) => { await conn.DeleteAsync(entity); });
        }

        public async Task DeleteAll()
        {
            await DB.Connection.ExecuteAsyncAction(async (conn) => { await conn.DeleteAllAsync<TEntity>(); });
        }

        public async Task Clear(SQLiteAsyncConnection asyncConn)
        {
            await asyncConn.DeleteAllAsync<TEntity>();
        }
    }
}
