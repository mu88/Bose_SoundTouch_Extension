using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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
        public async Task TurnOffAsync(ISpeaker speaker)
        {
            if (await GetPowerStateAsync(speaker) == PowerState.TurnedOn)
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
        public async Task<PowerState> GetPowerStateAsync(ISpeaker speaker)
        {
            var response = await HttpClient.GetStringAsync($"http://{speaker.IpAddress}:8090/now_playing");

            return response.Contains("source=\"STANDBY\"") ? PowerState.TurnedOff : PowerState.TurnedOn;
        }

        /// <inheritdoc />
        public async Task PlayAsync(ISpeaker speaker, IContent content)
        {
            await HttpClient.PostAsync($"http://{speaker.IpAddress}:8090/select",
                                       new StringContent(content.RawContent, Encoding.UTF8, "text/xml"));
        }

        /// <inheritdoc />
        public async Task<IContent> GetCurrentContentAsync(ISpeaker speaker)
        {
            var response = await HttpClient.GetStringAsync($"http://{speaker.IpAddress}:8090/now_playing");

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(response);

            var content = xmlDocument["nowPlaying"]?["ContentItem"]?.OuterXml;

            return !response.Contains("source=\"STANDBY\"") ? new Content(content) : null;
        }

        /// <inheritdoc />
        public async Task<NowPlaying> NowPlayingAsync(ISpeaker speaker)
        {
            var response = await HttpClient.GetStringAsync($"http://{speaker.IpAddress}:8090/now_playing");

            var serializer = new XmlSerializer(typeof(NowPlaying));
            NowPlaying result;
            using (TextReader reader = new StringReader(response))
            {
                result = (NowPlaying)serializer.Deserialize(reader);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<(string, string)> GetBasicInfoAsync(string ipAddress)
        {
            var name = "Unknown";
            var macAddress = string.Empty;

            var response = await HttpClient.GetStringAsync($"http://{ipAddress}:8090/info");
            var nameMatch = new Regex(@"<name>(\w*)</name>").Match(response);
            var macAddressMatch = new Regex(@"deviceID=\""(\w*)\""").Match(response);
            if (nameMatch.Success && nameMatch.Groups.Count == 2)
            {
                name = nameMatch.Groups[1].Value;
            }

            if (macAddressMatch.Success && macAddressMatch.Groups.Count == 2)
            {
                macAddress = macAddressMatch.Groups[1].Value;
            }

            return (name, macAddress);
        }
    }
}