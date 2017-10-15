using LaserwarTest.Commons.Observables;
using LaserwarTest.Data.DB;
using LaserwarTest.Data.DB.Entities;

namespace LaserwarTest.Presentation.Games
{
    /// <summary>
    /// Представляет данные об игроке
    /// </summary>
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
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = _name;
                    OnPropertyChanged();
                    return;
                }

                SetProperty(ref _name, value);
            }
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

        public void Save()
        {
            Data.Name = Name;
            Data.Accuracy = AccuracyPercentage * 0.01;
            Data.Rating = Rating;
            Data.Shots = Shots;

            using (var db = DBManager.GetLocalDB().Connection.Open())
                db.Update(Data);
        }
    }
}
