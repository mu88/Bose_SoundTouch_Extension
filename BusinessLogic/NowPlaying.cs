using System.Xml.Serialization;

namespace BusinessLogic
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

    [XmlRoot("ContentItem")]
    public class ContentItem
    {
        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("location")]
        public string Location { get; set; }

        [XmlAttribute("sourceAccount")]
        public string SourceAccount { get; set; }

        [XmlAttribute("isPresetable")]
        public bool IsPresetable { get; set; }

        [XmlElement("itemName")]
        public string ItemName { get; set; }

        [XmlElement("containerArt")]
        public string ContainerArt { get; set; }
    }
}