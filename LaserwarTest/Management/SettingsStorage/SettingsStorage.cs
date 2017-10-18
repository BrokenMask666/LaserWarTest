using LaserwarTest.Commons.Management.Settings;
using LaserwarTest.Management.SettingsStorage.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Management.Settings
{
    /// <summary>
    /// Представляет единую точку входа для настроек приложения
    /// </summary>
    public class SettingsStorage : SettingsStorageContainer
    {
        Lazy<VKSettings> _vKSettings = new Lazy<VKSettings>(() => Root.GetContainer<VKSettings>("VK"));

        public SettingsStorage() { }

        /// <summary>
        /// Получает настройки социальной сети ВКонтакте
        /// </summary>
        public VKSettings VK => _vKSettings.Value;
    }
}
