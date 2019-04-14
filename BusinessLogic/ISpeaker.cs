namespace BusinessLogic
{
    public interface ISpeaker:IDevice
    {
        string Name { get; }
        PowerState PowerState { get; }
        bool IsPlaying { get; }
        IContent CurrentlyPlaying { get; }
        void ShiftToSpeaker(ISpeaker otherSpeaker);
        void TurnOff();
        void Play(IContent content);
    }
}