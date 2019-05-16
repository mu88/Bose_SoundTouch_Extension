using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLogic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace Tests
{
    [TestFixture]
    public class BoseConnectionTests
    {
        [Test]
        public async Task TurnOff()
        {
            var httpMock = new MockHttpMessageHandler();
            httpMock.Expect(HttpMethod.Post, "http://1.2.3.4:8090/key")
                    .WithContent("<key state=\"press\" sender=\"Gabbo\">POWER</key>")
                    .Respond(HttpStatusCode.Created);
            httpMock.Expect(HttpMethod.Post, "http://1.2.3.4:8090/key")
                    .WithContent("<key state=\"release\" sender=\"Gabbo\">POWER</key>")
                    .Respond(HttpStatusCode.Created);
            var deviceMock = new Mock<ISpeaker>();
            deviceMock.Setup(x => x.IpAddress).Returns("1.2.3.4");
            var httpClient = new HttpClient(httpMock);
            var testee = new BoseConnection(httpClient);

            await testee.TurnOffAsync(deviceMock.Object);

            httpMock.VerifyNoOutstandingExpectation();
        }

        [Test]
        [Ignore("Not yet ready")]
        public async Task GetCurrentContentAsync()
        {
            const string rawXmlContent = "<myContent />";
            var httpMock = new MockHttpMessageHandler();
            httpMock.Expect(HttpMethod.Get, "http://1.2.3.4:8090/now_playing").Respond("application/xml", rawXmlContent);
            var deviceMock = new Mock<ISpeaker>();
            deviceMock.Setup(x => x.IpAddress).Returns("1.2.3.4");
            var httpClient = new HttpClient(httpMock);
            var testee = new BoseConnection(httpClient);

            var result = await testee.GetCurrentContentAsync(deviceMock.Object);

            result.RawContent.Should().Be(rawXmlContent);
            httpMock.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task PlayAsync()
        {
            const string rawXmlContent = "<myContent />";
            var httpMock = new MockHttpMessageHandler();
            httpMock.Expect(HttpMethod.Post, "http://1.2.3.4:8090/select").WithContent(rawXmlContent).Respond(HttpStatusCode.Created);
            var deviceMock = new Mock<ISpeaker>();
            deviceMock.Setup(x => x.IpAddress).Returns("1.2.3.4");
            var httpClient = new HttpClient(httpMock);
            var testee = new BoseConnection(httpClient);

            await testee.PlayAsync(deviceMock.Object, new Content(rawXmlContent));

            httpMock.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task GetPowerStateAsync()
        {
            const string rawXmlContent = "<ContentItem source=\"STANDBY\"";
            var httpMock = new MockHttpMessageHandler();
            httpMock.Expect(HttpMethod.Get, "http://1.2.3.4:8090/now_playing").Respond("application/xml", rawXmlContent);
            var deviceMock = new Mock<ISpeaker>();
            deviceMock.Setup(x => x.IpAddress).Returns("1.2.3.4");
            var httpClient = new HttpClient(httpMock);
            var testee = new BoseConnection(httpClient);

            var result = await testee.GetPowerStateAsync(deviceMock.Object);

            result.Should().Be(PowerState.TurnedOff);
            httpMock.VerifyNoOutstandingExpectation();
        }
    }
}