using System.Threading.Tasks;
using BusinessLogic.DTO;

namespace BusinessLogic
{
    public interface IConnection
    {
        Task TurnOffAsync(ISpeaker speaker);

        Task<PowerState> GetPowerStateAsync(ISpeaker speaker);

        Task PlayAsync(ISpeaker speaker, ContentItem content);

        Task<ContentItem> GetCurrentContentAsync(ISpeaker speaker);
    }
}