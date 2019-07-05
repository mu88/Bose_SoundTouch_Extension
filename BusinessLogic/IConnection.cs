using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface IConnection
    {
        Task<(string, string)> GetBasicInfoAsync(string ipAddress);

        Task TurnOffAsync(ISpeaker speaker);

        Task<PowerState> GetPowerStateAsync(ISpeaker speaker);

        Task PlayAsync(ISpeaker speaker, IContent content);

        Task<IContent> GetCurrentContentAsync(ISpeaker speaker);

        Task<NowPlaying> NowPlayingAsync(ISpeaker speaker);
    }
}