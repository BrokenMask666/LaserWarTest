using LaserwarTest.Core.UI.Popups;
using LaserwarTest.Data.DB;
using LaserwarTest.Presentation.Games.Comparers;
using LaserwarTest.UI.Dialogs;
using LaserwarTest.UI.Popups.Animations;
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

        Player EditedPlayer { set; get; }

        public ObservableCollection<PlayersTeam> Teams
        {
            set => SetProperty(ref _teams, value);
            get => _teams;
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

        public void EditPlayer(Player player)
        {
            EditedPlayer = player;

            PlayerEditor editor = new PlayerEditor(player.ID);
            editor.PlayerSaved += Editor_PlayerSaved;

            new PopupContent(editor, new ScalePopupOpenAnimation(), new ScalePopupCloseAnimation()).Open();
        }

        private void Editor_PlayerSaved(object sender, Player e)
        {
            if (EditedPlayer != null)
            {
                PlayersTeam oldTeam = Teams.FirstOrDefault(x => x.IsItemOfGroup(EditedPlayer));
                oldTeam?.Remove(EditedPlayer);
            }

            PlayersTeam newTeam = Teams.FirstOrDefault(x => x.IsItemOfGroup(e));
            newTeam?.Add(e);

            EditedPlayer = null;
        }
    }
}
