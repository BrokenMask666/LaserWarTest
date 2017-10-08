using LaserwarTest.Core.Data.DB;
using LaserwarTest.Data.DB.Entities;

namespace LaserwarTest.Data.DB.Tables
{
    public class GameDataUrlTable : SQLiteDBTable<GameDataUrlEntity>
    {
        public GameDataUrlTable(SQLiteDB ownerDB) : base(ownerDB)
        {
        }
    }
}
