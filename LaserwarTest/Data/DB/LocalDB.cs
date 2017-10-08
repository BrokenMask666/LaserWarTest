using LaserwarTest.Core.Data.DB;
using LaserwarTest.Core.Data.DB.Versioning;
using LaserwarTest.Data.DB.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Data.DB
{
    /// <summary>
    /// Представляет локальную базу данных приложения
    /// </summary>
    public sealed class LocalDB : SQLiteDB
    {
        public SoundTable Sounds { get; }

        public GameDataUrlTable GameDataUrls { get; }

        public GameTable Games { get; }

        public TeamTable Teams { get; }

        public PlayerTable Players { get; }


        public LocalDB() : this(DBInfoLocal.DB_PATH) { }

        public LocalDB(string dbPath) : base(dbPath)
        {
            SetTables(
                (Sounds = new SoundTable(this)),
                (GameDataUrls = new GameDataUrlTable(this)),
                (Games = new GameTable(this)),
                (Teams = new TeamTable(this)),
                (Players = new PlayerTable(this)));
        }
    }
}
