using LaserwarTest.Commons.Management.Settings;
using System;

namespace LaserwarTest.Management.SettingsStorage.Storages
{
    /// <summary>
    /// Предоставляет доступ к настройкам, связанным с социальной сетью ВКонтакте
    /// </summary>
    public class VKSettings : SettingsStorageContainer
    {
        /// <summary>
        /// Создает объект, не привязанный ни к одному контейнеру хранилища настроек!
        /// </summary>
        public VKSettings() { }

        public string AT
        {
            set => SetValue("AT", value);
            get => GetValue<string>("AT");
        }

        public string UserID
        {
            set => SetValue("UserID", value);
            get => GetValue<string>("UserID");
        }

        public DateTime ExpirationTime
        {
            set => SetValue("ExpirationTime", value);
            get => GetValue("ExpirationTime");
        }
    }
}
