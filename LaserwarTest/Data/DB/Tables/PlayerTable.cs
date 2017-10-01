using LaserwarTest.Core.Data.DB;
using LaserwarTest.Data.DB.Entities;

namespace LaserwarTest.Data.DB.Tables
{
    public class PlayerTable : SQLiteDBTable<PlayerEntity>
    {
        public PlayerTable(SQLiteDB ownerDB) : base(ownerDB)
        {
        }
    }
}
