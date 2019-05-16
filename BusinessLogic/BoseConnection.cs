using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BusinessLogic
{
    public class BoseConnection : IConnection
    {
        public BoseConnection(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        private HttpClient HttpClient { get; }

        /// <inheritdoc />
        public async Task TurnOffAsync(ISpeaker speaker)
        {
            await HttpClient.PostAsync($"http://{speaker.IpAddress}:8090/key",
                                       new StringContent("<key state=\"press\" sender=\"Gabbo\">POWER</key>"));

            await HttpClient.PostAsync($"http://{speaker.IpAddress}:8090/key",
                                       new StringContent("<key state=\"release\" sender=\"Gabbo\">POWER</key>"));
        }

        /// <inheritdoc />
        public async Task<PowerState> GetPowerStateAsync(ISpeaker speaker)
        {
            var rawXmlResponse = await HttpClient.GetStringAsync($"http://{speaker.IpAddress}:8090/now_playing");

            return rawXmlResponse.Contains("source=\"STANDBY\"") ? PowerState.TurnedOff : PowerState.TurnedOn;
        }

        /// <inheritdoc />
        public async Task PlayAsync(ISpeaker speaker, IContent content)
        {
            var httpResponseMessage = await HttpClient.PostAsync($"http://{speaker.IpAddress}:8090/select",
                                                                 new StringContent(content.RawContent, Encoding.UTF8, "text/xml"));
        }

        /// <inheritdoc />
        public async Task<IContent> GetCurrentContentAsync(ISpeaker speaker)
        {
            var rawXmlString = await HttpClient.GetStringAsync($"http://{speaker.IpAddress}:8090/now_playing");
            
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(rawXmlString);

            var xmlElement = xmlDocument["nowPlaying"];
            var element = xmlElement["ContentItem"];

            var elementInnerXml = element.InnerXml;
            var elementOuterXml = element.OuterXml;

            return new Content(elementOuterXml);
        }
    }
}