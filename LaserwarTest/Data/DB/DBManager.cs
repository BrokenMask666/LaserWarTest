using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Data.DB
{
    /// <summary>
    /// Предоставляет доступ к различным базам данных
    /// </summary>
    public sealed class DBManager
    {
        private static Lazy<DBManager> _instance = new Lazy<DBManager>(() => new DBManager(), true);
        //private static DBManager _instance;
        /// <summary>
        /// Получает менеджер баз данных
        /// </summary>
        /// <returns></returns>
        public static DBManager GetCurrent() => _instance.Value/*_instance ?? (_instance = new DBManager())*/;

        DBManager() { }

        public Lazy<LocalDB> _localDB = new Lazy<LocalDB>(() => new LocalDB(), true);
        /// <summary>
        /// Получает локальную базу данных приложения
        /// </summary>
        public LocalDB LocalDB => _localDB.Value;

        /// <summary>
        /// Получает локальную базу данных приложения
        /// </summary>
        public static LocalDB GetLocalDB() => GetCurrent().LocalDB;
    }
}
