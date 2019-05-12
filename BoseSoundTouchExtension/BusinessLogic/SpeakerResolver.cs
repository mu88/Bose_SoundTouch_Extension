using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Android.Content;
using Android.Net.Wifi;
using BusinessLogic;
using Zeroconf;

namespace BoseSoundTouchExtension.BusinessLogic
{
    public class SpeakerResolver
    {
        public SpeakerResolver(Context applicationContext)
        {
            ApplicationContext = applicationContext;
        }

        private Context ApplicationContext { get; }

        public async Task<IEnumerable<ISpeaker>> ResolveSpeakersAsync()
        {
            var results = new Collection<ISpeaker>();

            var wifi = (WifiManager)ApplicationContext.GetSystemService(Context.WifiService);
            var multicastLock = wifi.CreateMulticastLock("Zeroconf lock");
            try
            {
                multicastLock.Acquire();

                var zeroconfHosts = await ZeroconfResolver.ResolveAsync("_soundtouch._tcp.local.");
                var boseConnection = new BoseConnection(new HttpClient());
                foreach (var zeroconfHost in zeroconfHosts)
                {
                    results.Add(new Speaker(zeroconfHost.DisplayName, zeroconfHost.IPAddress, boseConnection));
                }
            }
            finally
            {
                multicastLock.Release();
            }

            return results;
        }
    }
}