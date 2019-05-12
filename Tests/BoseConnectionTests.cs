using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLogic;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class BoseConnectionTests
    {
        [Test]
        [Ignore("Not yet ready")]
        public async Task TurnOff()
        {
            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(new Uri("127.0.0.1:8090/key"), new HttpResponseMessage(HttpStatusCode.OK));
            var deviceMock = new Mock<ISpeaker>();
            deviceMock.Setup(x => x.IpAddress).Returns("127.0.0.1");
            var httpClient = new HttpClient(fakeResponseHandler);
            var testee = new BoseConnection(httpClient);

            await testee.TurnOffAsync(deviceMock.Object);

            testee.GetPowerStateAsync(deviceMock.Object).Should().Be(PowerState.TurnedOff);
        }
    }
}