using LaserwarTest.Commons.Observables;
using System.Collections.Generic;

namespace LaserwarTest.Presentation.Games
{
    public class PlayersTeam : GroupSorter<Team, Player>
    {
        public PlayersTeam(Team group) : this(group, null) { }
        public PlayersTeam(Team group, IEnumerable<Player> collection) : base(group, (team, player) => player.TeamID == team.ID, collection) { }
    }
}
