using LaserwarTest.Core.Data.DB;
using LaserwarTest.Data.DB.Entities;

namespace LaserwarTest.Data.DB.Tables
{
    public class GameTable : SQLiteDBTable<GameEntity>
    {
        public GameTable(SQLiteDB ownerDB) : base(ownerDB)
        {
        }
    }
}
