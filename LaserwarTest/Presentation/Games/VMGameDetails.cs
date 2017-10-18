using LaserwarTest.Core.UI.Popups;
using LaserwarTest.Data.DB;
using LaserwarTest.Presentation.Games.Comparers;
using LaserwarTest.UI.Dialogs;
using LaserwarTest.UI.Popups.Animations;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Xfinium.Pdf;
using Xfinium.Pdf.Graphics;
using Xfinium.Pdf.Graphics.FormattedContent;
using Windows.System;
using LaserwarTest.Data.PDF;
using System.Collections.Generic;
using LaserwarTest.UI.Popups;

namespace LaserwarTest.Presentation.Games
{
    /// <summary>
    /// Модель представления деталей игры
    /// </summary>
    public sealed class VMGameDetails : BaseViewModel
    {
        Game _game;

        ObservableCollection<PlayersTeam> _playerTeams;

        Player EditedPlayer { set; get; }

        public ObservableCollection<PlayersTeam> Teams
        {
            set => SetProperty(ref _playerTeams, value);
            get => _playerTeams;
        }

        public async Task Load(Game game)
        {
            await Loading(0);

            _game = game;

            LocalDB localDB = DBManager.GetLocalDB();

            var teamsData = await localDB.Teams.GetAll(x => x.GameID == game.ID);
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

        public async Task SaveToPDF()
        {
            await Loading();

            LocalDB localDB = DBManager.GetLocalDB();

            var teamsData = await localDB.Teams.GetAll(x => x.GameID == _game.ID);
            var playersData = await localDB.Players.GetAll(x => teamsData.Any(team => team.ID == x.TeamID));

            var teams = teamsData.Select(teamData => new Team(teamData));
            var players = playersData.Select(playerData => new Player(playerData));

            var doc = await new PDFGameInfoGenerator().GenerateGamePDF(_game, teams, players);

            try
            {
                StorageFile pdfFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("doc.pdf", CreationCollisionOption.ReplaceExisting);
                using (var pdfStream = await pdfFile.OpenAsync(FileAccessMode.ReadWrite))
                using (Stream stm = pdfStream.AsStream())
                {
                    doc.Save(stm);
                    await stm.FlushAsync();
                }

                await Launcher.LaunchFileAsync(pdfFile);
            }
            catch (UnauthorizedAccessException)
            {
                new ErrorPopupContent("Не удалось сохранить файл", "Возможно, файл открыт в другом приложении").Open();
            }
            finally
            {
                Loaded();
            }
        }
    }
}
