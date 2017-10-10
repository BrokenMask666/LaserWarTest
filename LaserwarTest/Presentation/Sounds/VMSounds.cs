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
        ObservableCollection<Sound> _items = new ObservableCollection<Sound>();

        public ObservableCollection<Sound> Items
        {
            private set => SetProperty(ref _items, value);
            get => _items;
        }

        public async Task Load()
        {
            await Loading(0);

            LocalDB localDB = DBManager.GetLocalDB();
            var sounds = await localDB.Sounds.GetAll();

            Items = new ObservableCollection<Sound>(sounds.Select(x => new Sound(x)));

            Loaded();
        }
    }
}
