using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DTO;

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
            if (await GetPowerStateAsync(speaker) == PowerState.TurnedOn)
            {
                await HttpClient.PostAsync($"http://{speaker.IpAddress}:8090/key",
                                           new StringContent("<key state=\"press\" sender=\"Gabbo\">POWER</key>"));

                await HttpClient.PostAsync($"http://{speaker.IpAddress}:8090/key",
                                           new StringContent("<key state=\"release\" sender=\"Gabbo\">POWER</key>"));
            }
        }

        /// <inheritdoc />
        public async Task<PowerState> GetPowerStateAsync(ISpeaker speaker)
        {
            var response = await HttpClient.GetStringAsync($"http://{speaker.IpAddress}:8090/now_playing");

            var nowPlaying = SerializationHelper.Deserialize<NowPlaying>(response);

            return nowPlaying.ContentItem.Source.Equals("STANDBY") ? PowerState.TurnedOff : PowerState.TurnedOn;
        }

        /// <inheritdoc />
        public async Task PlayAsync(ISpeaker speaker, ContentItem content)
        {
            await HttpClient.PostAsync($"http://{speaker.IpAddress}:8090/select",
                                       new StringContent(SerializationHelper.Serialize(content), Encoding.UTF8, "text/xml"));
        }

        /// <inheritdoc />
        public async Task<ContentItem> GetCurrentContentAsync(ISpeaker speaker)
        {
            var response = await HttpClient.GetStringAsync($"http://{speaker.IpAddress}:8090/now_playing");

            var nowPlaying = SerializationHelper.Deserialize<NowPlaying>(response);

            return !nowPlaying.ContentItem.Source.Equals("STANDBY") ? nowPlaying.ContentItem : null;
        }
    }
}