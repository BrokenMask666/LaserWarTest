using System.Xml.Serialization;

namespace LaserwarTest.Core.Data.DB.Versioning.Xml
{
    [XmlRoot("DBInfo")]
    public sealed class XmlDBInfoLocal
    {
        [XmlAttribute("InstalledVersion")]
        public double InstalledVersionNumber { set; get; }
    }
}
