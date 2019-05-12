using System.Threading.Tasks;

namespace BusinessLogic
{
    public class Speaker : ISpeaker
    {
        public Speaker(string name, string ipAddress, IConnection connection)
        {
            Name = name;
            IpAddress = ipAddress;
            Connection = connection;
        }

        public string Name { get; }

        public Task<PowerState> PowerState => Connection.GetPowerStateAsync(this);

        public bool IsPlaying => CurrentlyPlaying != null;

        public Task<IContent> CurrentlyPlaying => Connection.GetCurrentContentAsync(this);

        public string IpAddress { get; }

        private IConnection Connection { get; }

        public async Task ShiftToSpeakerAsync(ISpeaker otherSpeaker)
        {
            await otherSpeaker.PlayAsync(await CurrentlyPlaying);

            await TurnOffAsync();
        }

        public async Task TurnOffAsync()
        {
            await Connection.TurnOffAsync(this);
        }

        public async Task PlayAsync(IContent content)
        {
            await Connection.PlayAsync(this, content);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}