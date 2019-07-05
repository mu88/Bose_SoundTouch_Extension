using System.Xml.Serialization;

namespace BusinessLogic.DTO
{
    [XmlRoot("info")]
    public class Info
    {
        [XmlAttribute("deviceID")]
        public string DeviceId { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("margeAccountUUID")]
        public string MargeAccountUuid { get; set; }

        [XmlArray("components")]
        [XmlArrayItem("component")]
        public Component[] Components { get; set; }

        [XmlElement("margeURL")]
        public string MargeUrl { get; set; }
        
        [XmlElement("networkInfo")]
        public NetworkInfo NetworkInfo { get; set; }

        [XmlElement("moduleType")]
        public string ModuleType { get; set; }

        [XmlElement("variant")]
        public string Variant { get; set; }

        [XmlElement("variantMode")]
        public string VariantMode { get; set; }

        [XmlElement("countryCode")]
        public string CountryCode { get; set; }

        [XmlElement("regionCode")]
        public string RegionCode { get; set; }
    }
}