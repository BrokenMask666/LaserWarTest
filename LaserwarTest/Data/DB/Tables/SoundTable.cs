using LaserwarTest.Core.Data.DB;
using LaserwarTest.Data.DB.Entities;

namespace LaserwarTest.Data.DB.Tables
{
    public class SoundTable : SQLiteDBTable<SoundEntity>
    {
        public SoundTable(SQLiteDB ownerDB) : base(ownerDB)
        {
        }
    }
}
