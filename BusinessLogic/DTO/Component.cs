using System.Xml.Serialization;

namespace BusinessLogic.DTO
{
    [XmlRoot("component")]
    public class Component
    {
        [XmlElement("componentCategory")]
        public string ComponentCategory { get; set; }

        [XmlElement("softwareVersion")]
        public string SoftwareVersion { get; set; }

        [XmlElement("serialNumber")]
        public string SerialNumber { get; set; }
    }
}