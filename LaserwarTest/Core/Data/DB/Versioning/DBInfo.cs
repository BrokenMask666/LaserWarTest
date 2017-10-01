using LaserwarTest.Core.Data.DB.Versioning.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace LaserwarTest.Core.Data.DB.Versioning
{
    /// <summary>
    /// Предоставляет информацию о базе данных
    /// </summary>
    public sealed class DBInfo
    {
        private const string RES_NAME = "ms-appx:///AppInfo/DBInfo.dbconfig";

        /// <summary>
        /// Получает список всех версий базы данных
        /// </summary>
        public List<DBVersionInfo> Versions { get; }
        /// <summary>
        /// Получает номер версии установленной базы
        /// </summary>
        public double InstalledVersionNumber { get; }
        /// <summary>
        /// Получает информацию об установленной версии базы данных
        /// </summary>
        public DBVersionInfo InstalledVersion { get; }

        private DBInfo(XmlDBInfo xmlData)
        {
            Versions = new List<DBVersionInfo>(xmlData.Versions.Count);
            Versions.AddRange(xmlData.Versions.Select(x => new DBVersionInfo(x)));

            InstalledVersionNumber = xmlData.InstalledVersionNumber;
            InstalledVersion = Versions.First(x => x.VersionNumber == InstalledVersionNumber);
        }

        public static async Task<DBInfo> GetInfo()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(RES_NAME));
            using (var fileStream = await file.OpenStreamForReadAsync())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlDBInfo));
                return new DBInfo(xmlSerializer.Deserialize(fileStream) as XmlDBInfo);
            }
        }
    }
}
