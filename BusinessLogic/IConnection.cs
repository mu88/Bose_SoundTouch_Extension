﻿using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface IConnection
    {
        Task TurnOffAsync(ISpeaker speaker);

        Task<PowerState> GetPowerStateAsync(ISpeaker speaker);

        Task PlayAsync(ISpeaker speaker, IContent content);

        Task<IContent> GetCurrentContentAsync(ISpeaker speaker);
    }
}