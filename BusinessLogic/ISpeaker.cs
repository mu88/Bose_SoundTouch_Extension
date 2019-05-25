namespace BusinessLogic
{
    public interface ISpeaker
    {
        string Name { get; }

        string IpAddress { get; }

        PowerState GetPowerStateAsync();

        bool IsPlayingAsync();

        IContent CurrentlyPlayingAsync();

        void ShiftToSpeakerAsync(ISpeaker otherSpeaker);

        void TurnOffAsync();

        void PlayAsync(IContent content);
    }
}