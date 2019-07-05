using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface ISpeaker
    {
        string Name { get; }

        string IpAddress { get; }

        string MacAddress { get; }

        Task<PowerState> GetPowerStateAsync();

        Task<bool> IsPlayingAsync();

        Task<IContent> CurrentlyPlayingAsync();

        Task<NowPlaying> NowPlayingAsync();

        Task ShiftToSpeakerAsync(ISpeaker otherSpeaker);

        Task TurnOffAsync();

        Task PlayAsync(IContent content);
    }
}