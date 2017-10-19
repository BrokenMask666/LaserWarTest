using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LaserwarTest.Commons.Management.Navigation
{
    /// <summary>
    /// Представляет базовые параметры навигации и методы для их получения и установки.
    /// Необходимо использовать наследников данного класса, чтобы добавлять новые параметры
    /// </summary>
    public class NavigationParameters
    {
        /// <summary>
        /// Получает пустой экземпляр параметров,
        /// использующийся вместо null-значения для определения отсутствия параметров
        /// </summary>
        public static readonly NavigationParameters Empty = new NavigationParameters();

        private Dictionary<string, object> _params = new Dictionary<string, object>();

        protected NavigationParameters()
        { }

        public bool IsEmpty() { return this == Empty; }

        protected void SetParam(object value, [CallerMemberName]string key = "") { _params.Add(key, value); }
        protected T GetParam<T>(T defaultValue = default(T), [CallerMemberName]string key = null)
        {
            if (_params.ContainsKey(key))
                return (T)_params[key];

            return defaultValue;
        }
    }
}
