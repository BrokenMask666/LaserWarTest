using LaserwarTest.Core.Media;
using LaserwarTest.Data.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Presentation.Sounds
{
    /// <summary>
    /// Модель представления страницы звуков
    /// </summary>
    public sealed class VMSounds : BaseViewModel
    {
        ObservableCollection<Sound> _items;

        AudioPlayer AudioPlayer { get; } = new AudioPlayer();

        public ObservableCollection<Sound> Items
        {
            private set
            {
                var oldItems = _items;
                if (!SetProperty(ref _items, value)) return;

                if (oldItems != null)
                {
                    foreach (var sound in oldItems)
                        sound.Dispose();
                }
            }
            get { return _items; }
        }

        public async Task Load()
        {
            await Loading(0);

            AudioPlayer.Stop();

            LocalDB localDB = DBManager.GetLocalDB();
            var sounds = await localDB.Sounds.GetAll();

            Items = new ObservableCollection<Sound>(sounds.Select(x => new Sound(x, AudioPlayer)));

            Loaded();
        }

        /// <summary>
        /// Освобождает используемые ресурсы
        /// </summary>
        public void Dispose()
        {
            AudioPlayer.Stop();
            Items = null;
        }
    }
}
