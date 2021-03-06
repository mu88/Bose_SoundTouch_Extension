﻿using System.Threading.Tasks;
using BusinessLogic.DTO;

namespace BusinessLogic
{
    public class Speaker : ISpeaker
    {
        public Speaker(string name, string ipAddress, IConnection connection)
        {
            Name = name;
            IpAddress = ipAddress;
            Connection = connection;
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
        public Task<PowerState> GetPowerStateAsync()
        {
            return Connection.GetPowerStateAsync(this);
        }

        /// <inheritdoc />
        public async Task<bool> IsPlayingAsync()
        {
            return await CurrentlyPlayingAsync() != null;
        }

        /// <inheritdoc />
        public Task<ContentItem> CurrentlyPlayingAsync()
        {
            return Connection.GetCurrentContentAsync(this);
        }

        public async Task ShiftToSpeakerAsync(ISpeaker otherSpeaker)
        {
            await otherSpeaker.PlayAsync(await CurrentlyPlayingAsync());

            await TurnOffAsync();

            await Task.Delay(1000); // to make sure that the other speaker has initialized itself successfully
        }

        public async Task TurnOffAsync()
        {
            await Connection.TurnOffAsync(this);
        }

        public async Task PlayAsync(ContentItem content)
        {
            await Connection.PlayAsync(this, content);
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