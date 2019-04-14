namespace BusinessLogic
{
    public interface IConnection
    {
        void TurnOff(IDevice device);
        PowerState GetPowerState(IDevice device);
        void Play(IDevice device, IContent content);
        IContent GetCurrentContent(IDevice device);
    }
}