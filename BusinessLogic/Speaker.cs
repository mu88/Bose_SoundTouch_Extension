using System.Runtime.InteropServices.ComTypes;

namespace BusinessLogic
{
    public class Speaker : ISpeaker
    {
        public string Name { get; }

        public Speaker(string name, IConnection connection)
        {
            Name = name;
            Connection = connection;
        }

        public void ShiftToSpeaker(ISpeaker otherSpeaker)
        {
            otherSpeaker.Play(CurrentlyPlaying);

            TurnOff();
        }

        public void TurnOff()
        {
            Connection.TurnOff(this);
        }

        private IConnection Connection { get; }

        public PowerState PowerState => Connection.GetPowerState(this);

        public bool IsPlaying => CurrentlyPlaying != null;

        public IContent CurrentlyPlaying => Connection.GetCurrentContent(this);

        public void Play(IContent content)
        {
            Connection.Play(this, content);
        }
    }
}