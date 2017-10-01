using SQLite.Net.Attributes;
using System;
using System.Reflection;

namespace LaserwarTest.Core.Data.DB
{
    public static class SQLiteDBHelper
    {
        public static string GetTableName<TEntity>() where TEntity : ISQLiteDBEntity
        {
            Type type = typeof(TEntity);
            TableAttribute tableAttr = type.GetTypeInfo().GetCustomAttribute<TableAttribute>(true);

            return (tableAttr != null ? tableAttr.Name : type.Name);
        }
    }
}
