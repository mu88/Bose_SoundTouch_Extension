using System;
using System.Net.Http;
using System.Threading.Tasks;

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
        public Task TurnOffAsync(ISpeaker speaker)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<PowerState> GetPowerStateAsync(ISpeaker speaker)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task PlayAsync(ISpeaker speaker, IContent content)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IContent> GetCurrentContentAsync(ISpeaker speaker)
        {
            throw new NotImplementedException();
        }
    }
}