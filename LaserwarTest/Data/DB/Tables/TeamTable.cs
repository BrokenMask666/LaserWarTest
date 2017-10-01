using LaserwarTest.Core.Data.DB;
using LaserwarTest.Data.DB.Entities;

namespace LaserwarTest.Data.DB.Tables
{
    public class TeamTable : SQLiteDBTable<TeamEntity>
    {
        public TeamTable(SQLiteDB ownerDB) : base(ownerDB)
        {
        }
    }

}
