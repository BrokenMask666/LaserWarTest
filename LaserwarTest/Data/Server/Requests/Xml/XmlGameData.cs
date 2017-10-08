﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace LaserwarTest.Data.Server.Requests.Xml
{
    [XmlRoot("game")]
    public class XmlGameData
    {
        long _dateAsTicksUtc;

        [XmlAttribute("name")]
        public string Name { set; get; }

        [XmlAttribute("date")]
        public long DateAsTicksUtc
        {
            set
            {
                _dateAsTicksUtc = value;
                Date = new DateTime(_dateAsTicksUtc).ToLocalTime();
            }
            get { return _dateAsTicksUtc; }
        }

        [XmlIgnore]
        public DateTime Date { private set; get; }

        [XmlElement("team")]
        public List<XmlTeamData> Teams { set; get; }

        public static XmlGameData FromString(string xmlData)
        {
            XmlGameData ret = null;
            using (var reader = new StringReader(xmlData))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlGameData));
                ret = serializer.Deserialize(reader) as XmlGameData;
            }

            return ret;
        }
    }

    public class XmlTeamData
    {
        [XmlAttribute("name")]
        public string Name { set; get; }

        [XmlElement("player")]
        public List<XmlPlayerData> Players { set; get; }
    }

    public class XmlPlayerData
    {
        [XmlAttribute("name")]
        public string Name { set; get; }

        [XmlAttribute("rating")]
        public int Rating { set; get; }

        [XmlAttribute("accuracy")]
        public double Accuracy { set; get; }

        [XmlAttribute("shots")]
        public int Shots { set; get; }
    }
}
