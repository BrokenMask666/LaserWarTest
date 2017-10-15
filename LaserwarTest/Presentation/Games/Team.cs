using LaserwarTest.Commons.Observables;
using LaserwarTest.Data.DB.Entities;

namespace LaserwarTest.Presentation.Games
{
    public class Team : ObservableObject
    {
        public int ID { get; }
        public string Name { get; }

        public Team(TeamEntity data)
        {
            ID = data.ID;
            Name = data.Name;
        }
    }
}
