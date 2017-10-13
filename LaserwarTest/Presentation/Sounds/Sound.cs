using LaserwarTest.Commons.Observables;
using LaserwarTest.Core.Media;
using LaserwarTest.Data.DB;
using LaserwarTest.Data.DB.Entities;
using LaserwarTest.Presentation.Common;
using System;

namespace LaserwarTest.Presentation.Sounds
{
    /// <summary>
    /// Предоставляет доступ к управлению звуковым файлом
    /// </summary>
    public class Sound : ObservableObject
    {
        string _name;
        FileSizePresenter _size;

        /// <summary>
        /// Получает доступ к информации о звуке
        /// </summary>
        SoundEntity Data { get; }

        /// <summary>
        /// Получает доступ к управлению загрузкой файла
        /// </summary>
        public SoundDownloader SoundDownloader { get; }
        /// <summary>
        /// Получает доступ к управлению проигрывания файла
        /// </summary>
        public SoundPlayer SoundPlayer { get; }

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

        public Sound(SoundEntity data, AudioPlayer player)
        {
            Data = data;

            Name = data.Name;
            Size = new FileSizePresenter(data.Size);

            string fileName = $"Downloads/Sounds/{data.URL.Substring(data.URL.LastIndexOfAny(new char[] { '\\', '/' }) + 1)}";
            bool isDownloaded = data.Downloaded;

            SoundDownloader = new SoundDownloader(data.ID, data.URL, fileName, isDownloaded);
            SoundDownloader.Downloaded += SoundDownloader_Downloaded;

            if (!isDownloaded && SoundDownloader.IsDownloaded)
                SetEntityIsDownloaded();

            SoundPlayer = new SoundPlayer(player, fileName, SoundDownloader.IsDownloaded);
        }

        private void SoundDownloader_Downloaded(object sender, EventArgs e)
        {
            SetEntityIsDownloaded();
            SoundPlayer.SetIsDownloaded();
        }

        void SetEntityIsDownloaded()
        {
            Data.Downloaded = true;

            using (var db = DBManager.GetLocalDB().Connection.Open())
                db.Update(Data);
        }

        /// <summary>
        /// Освобождает используемые ресурсы
        /// </summary>
        public void Dispose()
        {
            SoundDownloader.Dispose();
            SoundPlayer.Dispose();
        }
    }
}
