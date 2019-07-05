using System.Xml.Serialization;

namespace BusinessLogic.DTO
{
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