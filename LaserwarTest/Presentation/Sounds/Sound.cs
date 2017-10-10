using LaserwarTest.Commons.Observables;
using LaserwarTest.Data.DB.Entities;
using LaserwarTest.Presentation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Presentation.Sounds
{
    /// <summary>
    /// Предоставляет доступ к управлению звуковым файлом
    /// </summary>
    public class Sound : ObservableObject
    {
        string _name;
        FileSizePresenter _size;

        SoundDownloadingInfo _downloadingInfo;
        SoundPlayingInfo _playingInfo;

        /// <summary>
        /// Получает доступ к информации о звуке
        /// </summary>
        SoundEntity Data { get; }

        /// <summary>
        /// Получает название файла
        /// </summary>
        public string Name
        {
            private set => SetProperty(ref _name, value);
            get => _name;
        }

        /// <summary>
        /// Получает доступ к информации о размере файла
        /// </summary>
        public FileSizePresenter Size
        {
            private set => SetProperty(ref _size, value);
            get => _size;
        }

        /// <summary>
        /// Получает информацию о процессе загрузки файла
        /// </summary>
        public SoundDownloadingInfo Downloading
        {
            private set => SetProperty(ref _downloadingInfo, value);
            get => _downloadingInfo;
        }

        /// <summary>
        /// Получает информацию о процессе проигрывания файла
        /// </summary>
        public SoundPlayingInfo Playing
        {
            private set => SetProperty(ref _playingInfo, value);
            get => _playingInfo;
        }

        public Sound(SoundEntity data)
        {
            Data = data;

            Name = data.Name;
            Size = new FileSizePresenter(data.Size);
        }
    }
}
