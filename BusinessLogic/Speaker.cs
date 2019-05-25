﻿namespace BusinessLogic
{
    public class Speaker : ISpeaker
    {
        public Speaker(string name, string ipAddress, IConnection connection)
        {
            Name = name;
            IpAddress = ipAddress;
            Connection = connection;
        }

        public Speaker(string ipAddress, IConnection connection)
        {
            IpAddress = ipAddress;
            Connection = connection;
            Name = Connection.GetName(this);
        }

        public string Name { get; }

        public string IpAddress { get; }

        private IConnection Connection { get; }

        public static bool operator ==(Speaker left, Speaker right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Speaker left, Speaker right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Speaker)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode() * 397) ^ IpAddress.GetHashCode();
            }
        }

        /// <inheritdoc />
        public PowerState GetPowerStateAsync()
        {
            return Connection.GetPowerStateAsync(this);
        }

        /// <inheritdoc />
        public bool IsPlayingAsync()
        {
            return CurrentlyPlayingAsync() != null;
        }

        /// <inheritdoc />
        public IContent CurrentlyPlayingAsync()
        {
            return Connection.GetCurrentContentAsync(this);
        }

        public void ShiftToSpeakerAsync(ISpeaker otherSpeaker)
        {
            otherSpeaker.PlayAsync(CurrentlyPlayingAsync());

            TurnOffAsync();

            //Task.Delay(1000); // to make sure that the other speaker has initialized itself successfully
        }

        public void TurnOffAsync()
        {
            Connection.TurnOffAsync(this);
        }

        public void PlayAsync(IContent content)
        {
            Connection.PlayAsync(this, content);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        protected bool Equals(Speaker other)
        {
            return string.Equals(Name, other.Name) && string.Equals(IpAddress, other.IpAddress);
        }
    }
}