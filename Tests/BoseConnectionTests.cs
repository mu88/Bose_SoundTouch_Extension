using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLogic;
using BusinessLogic.DTO;
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
            httpMock.Expect(HttpMethod.Get, "http://1.2.3.4:8090/now_playing")
                    .Respond("application/xml", "<nowPlaying><ContentItem source=\"TUNEIN\" /></nowPlaying>");
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
        public async Task GetCurrentContentAsync()
        {
            const string rawXmlContent = "<ContentItem source=\"Amazon\"><itemName>MyItem</itemName></ContentItem>";
            var rawXmlResponse = $"<nowPlaying>{rawXmlContent}</nowPlaying>";
            var httpMock = new MockHttpMessageHandler();
            httpMock.Expect(HttpMethod.Get, "http://1.2.3.4:8090/now_playing").Respond("application/xml", rawXmlResponse);
            var deviceMock = new Mock<ISpeaker>();
            deviceMock.Setup(x => x.IpAddress).Returns("1.2.3.4");
            var httpClient = new HttpClient(httpMock);
            var testee = new BoseConnection(httpClient);

            var result = await testee.GetCurrentContentAsync(deviceMock.Object);

            result.ItemName.Should().Be("MyItem");
            httpMock.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task PlayAsync()
        {
            const string rawXmlContent = "<itemName>MyItem</itemName>";
            var httpMock = new MockHttpMessageHandler();
            httpMock.Expect(HttpMethod.Post, "http://1.2.3.4:8090/select")
                    .WithPartialContent(rawXmlContent)
                    .Respond(HttpStatusCode.Created);
            var deviceMock = new Mock<ISpeaker>();
            deviceMock.Setup(x => x.IpAddress).Returns("1.2.3.4");
            var httpClient = new HttpClient(httpMock);
            var testee = new BoseConnection(httpClient);

            await testee.PlayAsync(deviceMock.Object, new ContentItem { ItemName = "MyItem" });

            httpMock.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task GetPowerStateAsync()
        {
            const string rawXmlContent = "<nowPlaying><ContentItem source=\"STANDBY\" /></nowPlaying>";
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