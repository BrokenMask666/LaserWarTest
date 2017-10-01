using System.Xml.Serialization;

namespace LaserwarTest.Core.Data.DB.Versioning.Xml
{
    public sealed class XmlDBVersionInfo
    {
        [XmlAttribute("Version")]
        public double VersionNumber { set; get; }

        [XmlAttribute("IsCritical")]
        public bool IsCritical { set; get; }
    }
}
