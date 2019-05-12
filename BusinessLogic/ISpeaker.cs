using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface ISpeaker
    {
        string Name { get; }

        Task<PowerState> PowerState { get; }

        bool IsPlaying { get; }

        Task<IContent> CurrentlyPlaying { get; }

        string IpAddress { get; }

        Task ShiftToSpeakerAsync(ISpeaker otherSpeaker);

        Task TurnOffAsync();

        Task PlayAsync(IContent content);
    }
}