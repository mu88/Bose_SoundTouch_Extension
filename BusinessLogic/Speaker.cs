using System.Threading.Tasks;
using WebSocketSharp;

namespace BusinessLogic
{
    public class Speaker : ISpeaker
    {
        public Speaker(string name,
                       string ipAddress,
                       string macAddress,
                       IConnection connection)
        {
            Name = name;
            IpAddress = ipAddress;
            Connection = connection;
            MacAddress = macAddress;
        }

        public string Name { get; }

        public string IpAddress { get; }

        public string MacAddress { get; }

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
        public Task<IContent> CurrentlyPlayingAsync()
        {
            return Connection.GetCurrentContentAsync(this);
        }

        /// <inheritdoc />
        public Task<NowPlaying> NowPlayingAsync()
        {
            return Connection.NowPlayingAsync(this);
        }

        public async Task ShiftToSpeakerAsync(ISpeaker otherSpeaker)
        {
//            await otherSpeaker.PlayAsync(await CurrentlyPlayingAsync());

            using (var ws = new WebSocket($"ws://{otherSpeaker.IpAddress}:8080", "gabbo"))
            {
                ws.Connect();
                ws.Send(TranslateContent(otherSpeaker, await NowPlayingAsync()));
            }

//            await TurnOffAsync();

            await Task.Delay(1000); // to make sure that the other speaker has initialized itself successfully
        }

        public async Task TurnOffAsync()
        {
            await Connection.TurnOffAsync(this);
        }

        public async Task PlayAsync(IContent content)
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

        private static string TranslateContent(ISpeaker otherSpeaker, NowPlaying currentlyPlaying)
        {
            // deviceID and requestID MUST be specified, but only deviceID seems to be important. Setting the requestID to nonsense has no effect.
            //var result = $"<msg><header deviceID=\"{otherSpeaker.MacAddress}\" url=\"playbackRequest\" method=\"POST\">" +
            //             "<request requestID=\"9999\"><info type=\"new\"/></request></header>" +
            //             $"<body><playbackRequest source=\"AMAZON\" sourceAccount=\"mirkoulfig@hotmail.de\">" +
            //             "<container type=\"tracklist\" location=\"catalog/playlists/B07SHH8NCP/chunk=0/#playable\" " +
            //             "isPresetable=\"true\" source=\"AMAZON\" sourceAccount=\"mirkoulfig@hotmail.de\">" +
            //             "<itemName>Best of Prime Music</itemName>" +
            //             "<containerArt>https://images-na.ssl-images-amazon.com/images/I/511B58sMm0L._SCLZZZZZZZ__SX150_SY150_.jpg</containerArt>" +
            //             "</container>" + "<track type=\"track\" location=\"index:1::uri:catalog/playlists/B07SHH8NCP/chunk=0/#chunk\" " +
            //             "isPresetable=\"true\" source=\"AMAZON\" sourceAccount=\"mirkoulfig@hotmail.de\">" +
            //             "<itemName>Nothing Breaks Like a Heart</itemName></track></playbackRequest></body></msg>";

            // ONLY the track index within playlist is missing

            var result = $"<msg><header deviceID=\"{otherSpeaker.MacAddress}\" url=\"playbackRequest\" method=\"POST\">" +
                         "<request requestID=\"9999\"><info type=\"new\"/></request></header>" +
                         $"<body><playbackRequest source=\"{currentlyPlaying.ContentItem.Source}\" sourceAccount=\"{currentlyPlaying.ContentItem.SourceAccount}\">" +
                         $"<container type=\"{currentlyPlaying.ContentItem.Type}\" location=\"{currentlyPlaying.ContentItem.Location}\" " +
                         $"isPresetable=\"{currentlyPlaying.ContentItem.IsPresetable.ToString().ToLower()}\" source=\"{currentlyPlaying.ContentItem.Source}\" sourceAccount=\"{currentlyPlaying.ContentItem.SourceAccount}\">" +
                         $"<itemName>{currentlyPlaying.ContentItem.ItemName}</itemName>" +
                         $"<containerArt>{currentlyPlaying.ContentItem.ContainerArt}</containerArt>" +
                         $"</container><track type=\"track\" location=\"index:1::uri:{currentlyPlaying.ContentItem.Location.Replace("#playable","#chunk")}\" " +
                         $"isPresetable=\"{currentlyPlaying.ContentItem.IsPresetable.ToString().ToLower()}\" source=\"{currentlyPlaying.ContentItem.Source}\" sourceAccount=\"{currentlyPlaying.ContentItem.SourceAccount}\">" +
                         $"<itemName>{currentlyPlaying.Track}</itemName></track></playbackRequest></body></msg>";

            return result;
        }
    }
}