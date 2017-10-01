using LaserwarTest.Core.Data.DB.Versioning.Xml;

namespace LaserwarTest.Core.Data.DB.Versioning
{
    /// <summary>
    /// Представляет информацию о версии базы данных
    /// </summary>
    public sealed class DBVersionInfo
    {
        private XmlDBVersionInfo Data { get; }

        /// <summary>
        /// Получает номер версии базы данных
        /// </summary>
        public double VersionNumber { get { return Data.VersionNumber; } }

        /// <summary>
        /// Указывает, являются ли обновления в текущей версии критическими с точки зрения внутренней структуры.
        /// Критическими считаются обновления, приводящие к серьезным изменениям в базе данных
        /// (формат данных, предустановленные значения)
        /// </summary>
        public bool IsCritical { get { return Data.IsCritical; } }

        public DBVersionInfo(XmlDBVersionInfo xmlData)
        {
            Data = xmlData;
        }
    }
}
