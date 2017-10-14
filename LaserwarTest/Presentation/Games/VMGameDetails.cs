using LaserwarTest.Commons.Observables;
using LaserwarTest.Data.DB;
using LaserwarTest.Data.DB.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

    public class Player : ObservableObject
    {
        string _name;
        int _rating;
        int _accuracyPercentage;
        int _shots;

        PlayerEntity Data { get; }

        public int ID => Data.ID;
        public int TeamID => Data.TeamID;

        public string Name
        {
            set => SetProperty(ref _name, value);
            get => _name;
        }

        public int Rating
        {
            set => SetProperty(ref _rating, value);
            get => _rating;
        }

        public int AccuracyPercentage
        {
            set
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;

                _accuracyPercentage = value;
                OnPropertyChanged();
            }
            get => _accuracyPercentage;
        }

        public int Shots
        {
            set => SetProperty(ref _shots, value);
            get => _shots;
        }

        public Player(PlayerEntity data)
        {
            Data = data;

            Name = data.Name;
            Rating = data.Rating;
            AccuracyPercentage = (int)(data.Accuracy * 100);
            Shots = data.Shots;
        }
    }


    public class PlayersTeam : GroupSorter<Team, Player>
    {
        public PlayersTeam(Team group) : this(group, null) { }
        public PlayersTeam(Team group, IEnumerable<Player> collection) : base(group, (team, player) => player.TeamID == team.ID, collection) { }
    }

    public class PlayerComparer : IComparer<Player>
    {
        public bool SortByDesc { get; }

        public PlayerComparer(bool desc = false)
        {
            SortByDesc = desc;
        }

        public virtual int Compare(Player x, Player y)
        {
            Player first = (SortByDesc) ? y : x;
            Player second = (SortByDesc) ? x : y;

            return first.Name.CompareTo(second.Name);
        }
    }

    public class PlayerByRatingComparer : PlayerComparer
    {
        public PlayerByRatingComparer(bool desc = false) : base(desc)
        {
        }

        public override int Compare(Player x, Player y)
        {
            int result = x.Rating.CompareTo(y.Rating);
            if (result == 0) return base.Compare(x, y);

            return result;
        }
    }

    public class PlayerByAccuracComparer : PlayerComparer
    {
        public PlayerByAccuracComparer(bool desc = false) : base(desc)
        {
        }

        public override int Compare(Player x, Player y)
        {
            int result = x.AccuracyPercentage.CompareTo(y.AccuracyPercentage);
            if (result == 0) return base.Compare(x, y);

            return result;
        }
    }

    public class PlayerByShotsComparer : PlayerComparer
    {
        public PlayerByShotsComparer(bool desc = false) : base(desc)
        {
        }

        public override int Compare(Player x, Player y)
        {
            int result = x.Shots.CompareTo(y.Shots);
            if (result == 0) return base.Compare(x, y);

            return result;
        }
    }
}
