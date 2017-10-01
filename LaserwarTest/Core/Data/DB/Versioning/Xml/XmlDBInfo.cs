using System.Collections.Generic;
using System.Xml.Serialization;

namespace LaserwarTest.Core.Data.DB.Versioning.Xml
{
    [XmlRoot("DBInfo")]
    public sealed class XmlDBInfo
    {
        [XmlAttribute("InstalledVersion")]
        public double InstalledVersionNumber { set; get; }

        [XmlArray("Versions"), XmlArrayItem("VersionInfo")]
        public List<XmlDBVersionInfo> Versions { set; get; }
    }
}
