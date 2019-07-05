using System.Xml.Serialization;

namespace BusinessLogic.DTO
{
    [XmlRoot("nowPlaying")]
    public class NowPlaying
    {
        [XmlElement("ContentItem")]
        public ContentItem ContentItem { get; set; }
        
        [XmlElement("track")]
        public string Track { get; set; }

        [XmlElement("artist")]
        public string Artist { get; set; }

        [XmlElement("album")]
        public string Album { get; set; }

        [XmlElement("stationName")]
        public string StationName { get; set; }

        [XmlElement("art")]
        public string Art { get; set; }
    }
}