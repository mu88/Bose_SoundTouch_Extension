namespace BusinessLogic
{
    public interface IConnection
    {
        string GetName(ISpeaker speaker);

        void TurnOffAsync(ISpeaker speaker);

        PowerState GetPowerStateAsync(ISpeaker speaker);

        void PlayAsync(ISpeaker speaker, IContent content);

        IContent GetCurrentContentAsync(ISpeaker speaker);
    }
}