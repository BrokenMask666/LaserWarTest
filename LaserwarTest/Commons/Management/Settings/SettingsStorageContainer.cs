using LaserwarTest.Commons.Helpers.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.Storage;

namespace LaserwarTest.Commons.Management.Settings
{
    /// <summary>
    /// Предоставляет универсальный механизм доступа и управления хранилищем настроек, используемых в приложении.
    /// Позволяет работать с отдельным контейнером настроек, а также формировать новые контейнеры внутри текущего.
    /// Должен являться базовым типом для всех остальных классов для работы с настройками
    /// </summary>
    public class SettingsStorageContainer
    {
        protected static SettingsStorageContainer Root { get; } = new SettingsStorageContainer(ApplicationData.Current.LocalSettings);

        private ApplicationDataContainer _container;
        private Dictionary<string, SettingsStorageContainer> _containers = new Dictionary<string, SettingsStorageContainer>();

        protected SettingsStorageContainer()
        {
        }

        protected SettingsStorageContainer(ApplicationDataContainer container)
        {
            if (Root != null && container == ApplicationData.Current.LocalSettings)
                throw new ArgumentException("SettingsStorageContainer can't be ApplicationLocalSettings");

            _container = container;
        }

        #region Container

        /// <summary>
        /// Создает новый контейнер указанного типа или возвращает существующий
        /// </summary>
        /// <typeparam name="TContainer">Тип контейнера</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public TContainer GetContainer<TContainer>(string name) where TContainer : SettingsStorageContainer, new()
        {
            if (_containers.ContainsKey(name))
                return (TContainer)_containers[name];

            TContainer container = new TContainer
            {
                _container = _container.CreateContainer(name, ApplicationDataCreateDisposition.Always)
            };

            _containers.Add(name, container);

            return container;
        }

        /// <summary>
        /// Создает новый контейнер указанного типа или возвращает существующий
        /// </summary>
        /// <typeparam name="TContainer">Тип контейнера</typeparam>
        /// <param name="name"></param>
        /// <param name="createFunc"></param>
        /// <returns></returns>
        public TContainer GetContainer<TContainer>(string name, Func<ApplicationDataContainer, TContainer> createFunc) where TContainer : SettingsStorageContainer
        {
            if (_containers.ContainsKey(name))
                return (TContainer)_containers[name];

            TContainer container = createFunc(_container.CreateContainer(name, ApplicationDataCreateDisposition.Always));
            _containers.Add(name, container);

            return container;
        }

        /// <summary>
        /// Удаляет вложеный контейнер из текущего
        /// </summary>
        /// <param name="name">Имя удаляемого контейнера</param>
        protected void DeleteContainer(string name)
        {
            _container.DeleteContainer(name);
            if (_containers.ContainsKey(name)) _containers.Remove(name);
        }

        /// <summary>
        /// Удаляет все вложенные контейнеры
        /// </summary>
        protected void DeleteContainers()
        {
            IEnumerable<string> availableContainers = _container.Containers.Keys;
            foreach (var container in availableContainers)
                _container.DeleteContainer(container);

            _containers.Clear();
        }

        /// <summary>
        /// Удаляет те из вложенных контейнеров, названия которых соответствуют определенному условию
        /// </summary>
        /// <param name="predicate">Условие, по которому определяется возможность удаления контейнера</param>
        protected void DeleteContainers(Predicate<string> predicate)
        {
            IEnumerable<string> availableContainers = _container.Containers.Keys;
            foreach (var container in availableContainers)
                if (predicate(container)) DeleteContainer(container);
        }

        #endregion Container

        #region Settings

        /// <summary>
        /// Устанавливает значение параметра настроек по ключу в указанное значение.
        /// Возвращает true, если устанавливаемое значение изменилось
        /// </summary>
        /// <param name="key">Ключ параметра</param>
        /// <param name="value">Устанавливаемое значение</param>
        protected bool SetValue(string key, object value)
        {
            if (_container.Values.ContainsKey(key) && _container.Values[key] == value)
                return false;

            _container.Values[key] = value;
            return true;
        }

        /// <summary>
        /// Устанавливает значение параметра настроек по ключу в указанное значение.
        /// Возвращает true, если устанавливаемое значение изменилось
        /// </summary>
        /// <param name="key">Ключ параметра</param>
        /// <param name="value">Устанавливаемое значение</param>
        protected bool SetValue(string key, DateTime value)
        {
            DateTimeKind kind = value.Kind;
            if (kind == DateTimeKind.Local)
                value = value.ToLocalTime();

            ApplicationDataCompositeValue compositeValue = new ApplicationDataCompositeValue
            {
                ["Ticks"] = value.Ticks,
                ["Kind"] = kind.ToString()
            };

            return SetValue(key, compositeValue);
        }

        /// <summary>
        /// Устанавливает значение параметра настроек по ключу в указанное значение.
        /// Возвращает true, если устанавливаемое значение изменилось
        /// </summary>
        /// <param name="key">Ключ параметра</param>
        /// <param name="value">Устанавливаемое значение</param>
        protected bool SetValue(string key, Enum value)
        {
            return SetValue(key, value.ToString());
        }

        /// <summary>
        /// Удаляет из настроек параметр по указанному ключу
        /// </summary>
        /// <param name="key">Ключ параметра</param>
        protected void DeleteValue(string key)
        {
            _container.Values.Remove(key);
        }

        /// <summary>
        /// Определяет, имеется ли в настройках указанный ключ параметра
        /// </summary>
        /// <param name="key">Проверяемый ключ</param>
        /// <returns></returns>
        protected bool IsKeyExists(string key)
        {
            return _container.Values.ContainsKey(key);
        }

        /// <summary>
        /// Получает значение из хранилища настроек, преобразуя в указанный тип, по ключу.
        /// Если ключ не найден, возвращает значение по умолчанию.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения</typeparam>
        /// <param name="key">Ключ параметра</param>
        /// <param name="defaultValue">Значение по умолчанию для случая, когда не найден указанный ключ</param>
        /// <returns></returns>
        protected T GetValue<T>(string key, T defaultValue = default(T))
        {
            if (!_container.Values.ContainsKey(key))
                return defaultValue;

            object value = _container.Values[key];

            Type vType = typeof(T);
            if (vType.GetTypeInfo().IsEnum)
                return EnumHelper.Parse<T>(value.ToString());

            return (T)value;
        }

        /// <summary>
        /// Получает значение из хранилища настроек, преобразуя в указанный тип, по ключу.
        /// Если ключ не найден, возвращает значение по умолчанию.
        /// </summary>
        /// <param name="key">Ключ параметра</param>
        /// <param name="defaultValue">Значение по умолчанию для случая, когда не найден указанный ключ</param>
        /// <returns></returns>
        protected DateTime GetValue(string key, DateTime defaultValue = default(DateTime))
        {
            if (!_container.Values.ContainsKey(key))
                return defaultValue;

            var value = (ApplicationDataCompositeValue)_container.Values[key];
            if (!(value.ContainsKey("Kind") || value.ContainsKey("Ticks")))
                return defaultValue;

            DateTimeKind kind = EnumHelper.Parse<DateTimeKind>(value["Kind"].ToString());
            long ticks = (long)(value["Ticks"]);

            return (kind == DateTimeKind.Local) 
                ? new DateTime(ticks, DateTimeKind.Utc).ToLocalTime()
                : new DateTime(ticks, kind);
        }

        #endregion Settings
    }
}
