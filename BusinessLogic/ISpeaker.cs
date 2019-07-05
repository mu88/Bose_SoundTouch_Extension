using System.Threading.Tasks;
using BusinessLogic.DTO;

namespace BusinessLogic
{
    public interface ISpeaker
    {
        string Name { get; }

        string IpAddress { get; }

        Task<PowerState> GetPowerStateAsync();

        Task<bool> IsPlayingAsync();

        Task<ContentItem> CurrentlyPlayingAsync();

        Task ShiftToSpeakerAsync(ISpeaker otherSpeaker);

        Task TurnOffAsync();

        Task PlayAsync(ContentItem content);
    }
}