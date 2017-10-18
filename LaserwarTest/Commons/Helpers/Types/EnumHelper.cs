using System;

namespace LaserwarTest.Commons.Helpers.Types
{
    /// <summary>
    /// Помогает при работе с перечислениями
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Преобразует объект в элемент перечисления типа T 
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления</typeparam>
        /// <param name="value">Преобразуемый объект</param>
        /// <returns></returns>
        public static TEnum Parse<TEnum>(string value)
        {
            if (value == null)
                return default(TEnum);

            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }

        /// <summary>
        /// Пытается преобразовать строку указанного перечисление в соответствующий значение.
        /// Выбрасывает <see cref="ArgumentException"/>, если преобразование неосуществимо
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления</typeparam>
        /// <param name="value">Строковое представления имени перечисляемой константы</param>
        /// <returns></returns>
        public static TEnum TryParse<TEnum>(string value) where TEnum : struct
        {
            TEnum enumValue;

            if (!Enum.TryParse(value, out enumValue))
                throw new ArgumentException($"Cannot convert enum value '{value}' of {typeof(TEnum)}! Check for it existance");

            return enumValue;
        }
    }
}
