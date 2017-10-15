using LaserwarTest.Data.DB;
using LaserwarTest.Presentation.Games.Comparers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LaserwarTest.Presentation.Games
{
    /// <summary>
    /// Модель представления деталей игры
    /// </summary>
    public sealed class VMGameDetails : BaseViewModel
    {
        ObservableCollection<PlayersTeam> _teams;
        PlayersTeam _Team;

        public ObservableCollection<PlayersTeam> Teams
        {
            set => SetProperty(ref _teams, value);
            get => _teams;
        }

        public PlayersTeam Team
        {
            set => SetProperty(ref _Team, value);
            get => _Team;
        }

        public async Task Load(int gameID)
        {
            await Loading(0);

            LocalDB localDB = DBManager.GetLocalDB();

            var teamsData = await localDB.Teams.GetAll(x => x.GameID == gameID);
            var playersData = await localDB.Players.GetAll(x => teamsData.Any(team => team.ID == x.TeamID));

            var teams = teamsData.Select(teamData => new Team(teamData));
            var players = playersData.Select(playerData => new Player(playerData));

            Teams = new ObservableCollection<PlayersTeam>(
                teams.Select(team => new PlayersTeam(team, players)));

            Team = Teams.First();

            Loaded();
        }

        public async Task Sort(PlayerComparer sorter)
        {
            if (sorter == null) return;

            await Loading(0);

            foreach (var team in Teams)
                team.SortItems(sorter);

            Teams = new ObservableCollection<PlayersTeam>(Teams);

            Loaded();
        }
    }

    
}
