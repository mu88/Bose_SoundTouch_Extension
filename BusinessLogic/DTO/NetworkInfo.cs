using System.Xml.Serialization;

namespace BusinessLogic.DTO
{
    [XmlRoot("networkInfo")]
    public class NetworkInfo
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("macAddress")]
        public string MacAddress { get; set; }

        [XmlElement("ipAddress")]
        public string IpAddress { get; set; }
    }
}