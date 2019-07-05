using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Content;
using Android.Net.Wifi;
using BusinessLogic;
using Zeroconf;
using ProtocolType = System.Net.Sockets.ProtocolType;

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
//            return GetSpeakersViaZeroconf();
            return await Task.Run(GetSpeakersViaUdp);
        }

        private async Task<IEnumerable<ISpeaker>> GetSpeakersViaUdp()
        {
            var ipAddresses = new HashSet<string>();

            for (var i = 0; i < 5; i++)
            {
                var multicastEndPoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
                const string searchString = "M-SEARCH * HTTP/1.1\r\nHOST:239.255.255.250:1900\r\nMAN:\"ssdp:discover\"\r\n" +
                                            "ST:urn:schemas-upnp-org:device:MediaRenderer:1\r\nMX:3\r\n\r\n";

                var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                udpSocket.SendTo(Encoding.UTF8.GetBytes(searchString), SocketFlags.None, multicastEndPoint);

                var receiveBuffer = new byte[64000];

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var continueWaiting = true;
                while (continueWaiting)
                {
                    if (udpSocket.Available > 0)
                    {
                        var receivedBytes = udpSocket.Receive(receiveBuffer, SocketFlags.None);

                        if (receivedBytes > 0)
                        {
                            var s = Encoding.UTF8.GetString(receiveBuffer, 0, receivedBytes);
                            var match = new Regex("Location: http://(.*):8091/").Match(s);
                            if (match.Success && match.Groups.Count == 2)
                            {
                                ipAddresses.Add(match.Groups[1].Value);
                                continueWaiting = false;
                            }
                        }
                    }

                    if (stopwatch.ElapsedMilliseconds > 15000)
                    {
                        stopwatch.Stop();
                        continueWaiting = false;
                    }
                }
            }

            var results = new Collection<ISpeaker>();

            foreach (var ipAddress in ipAddresses)
            {
                results.Add(await ConstructSpeaker(ipAddress));
            }

            return results;
        }

        private async Task<Speaker> ConstructSpeaker(string ipAddress)
        {
            var httpClient = new HttpClient();
            var boseConnection = new BoseConnection(httpClient);

            var (name, macAddress) = await boseConnection.GetBasicInfoAsync(ipAddress);

            return new Speaker(name, ipAddress, macAddress, boseConnection);
        }

        private IEnumerable<ISpeaker> GetSpeakersViaZeroconf()
        {
            var results = new Collection<ISpeaker>();

            var wifi = (WifiManager)ApplicationContext.GetSystemService(Context.WifiService);
            var multicastLock = wifi.CreateMulticastLock("Zeroconf lock");
            try
            {
                multicastLock.Acquire();

                var zeroconfHosts = ZeroconfResolver.ResolveAsync("_soundtouch._tcp.local.").Result;
                var boseConnection = new BoseConnection(new HttpClient());
                foreach (var zeroconfHost in zeroconfHosts)
                {
                    results.Add(new Speaker(zeroconfHost.DisplayName, zeroconfHost.IPAddress, "", boseConnection));
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