using LaserwarTest.Commons.Observables;
using LaserwarTest.Data.DB.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Presentation.Games
{
    /// <summary>
    /// Предоставляет информацию об игре
    /// </summary>
    public class Game : ObservableObject
    {
        string _name;
        string _date;
        int _playerCount;

        /// <summary>
        /// Получает доступ к информации о звуке
        /// </summary>
        GameEntity Data { get; }

        /// <summary>
        /// Получает название игры
        /// </summary>
        public string Name
        {
            private set => SetProperty(ref _name, value);
            get => _name;
        }

        /// <summary>
        /// Получает дату проведения игры
        /// </summary>
        public string Date
        {
            private set => SetProperty(ref _date, value);
            get => _date;
        }

        public int PlayersCount
        {
            private set => SetProperty(ref _playerCount, value);
            get => _playerCount;
        }

        public Game(GameEntity data, int playersCount)
        {
            Data = data;

            Name = data.Name;
            Date = data.Date.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentUICulture);

            PlayersCount = playersCount;
        }
    }
}
