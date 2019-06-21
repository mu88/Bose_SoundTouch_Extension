using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace BusinessLogic
{
    public class BoseConnection : IConnection
    {
        // TODO mu88: XML deserialize

        public BoseConnection(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        private HttpClient HttpClient { get; }

        /// <inheritdoc />
        public string GetName(ISpeaker speaker)
        {
            var response = HttpClient.GetStringAsync($"http://{speaker.IpAddress}:8090/info").Result;
            var match = new Regex("<name>(.*)</name>").Match(response);
            if (match.Success && match.Groups.Count == 2)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return "Unknown";
            }
        }

        /// <inheritdoc />
        public void TurnOffAsync(ISpeaker speaker)
        {
            if (GetPowerStateAsync(speaker) == PowerState.TurnedOn)
            {
                HttpClient.PostAsync($"http://{speaker.IpAddress}:8090/key",
                                     new StringContent("<key state=\"press\" sender=\"Gabbo\">POWER</key>"))
                          .Wait();

                HttpClient.PostAsync($"http://{speaker.IpAddress}:8090/key",
                                     new StringContent("<key state=\"release\" sender=\"Gabbo\">POWER</key>"))
                          .Wait();
            }
        }

        /// <inheritdoc />
        public PowerState GetPowerStateAsync(ISpeaker speaker)
        {
            var response = HttpClient.GetStringAsync($"http://{speaker.IpAddress}:8090/now_playing").Result;

            return response.Contains("source=\"STANDBY\"") ? PowerState.TurnedOff : PowerState.TurnedOn;
        }

        /// <inheritdoc />
        public void PlayAsync(ISpeaker speaker, IContent content)
        {
            HttpClient.PostAsync($"http://{speaker.IpAddress}:8090/select",
                                 new StringContent(content.RawContent, Encoding.UTF8, "text/xml"))
                      .Wait();
        }

        /// <inheritdoc />
        public IContent GetCurrentContentAsync(ISpeaker speaker)
        {
            var response = HttpClient.GetStringAsync($"http://{speaker.IpAddress}:8090/now_playing").Result;

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(response);

            var content = xmlDocument["nowPlaying"]?["ContentItem"]?.OuterXml;

            return !response.Contains("source=\"STANDBY\"") ? new Content(content) : null;
        }
    }
}