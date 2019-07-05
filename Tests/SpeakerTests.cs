using System.Threading.Tasks;
using BusinessLogic;
using BusinessLogic.DTO;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class SpeakerTests
    {
        [Test]
        public async Task ShiftToSpeaker()
        {
            var contentItem = new ContentItem();
            var connectionMock = new Mock<IConnection>();
            var connection = connectionMock.Object;
            var otherSpeaker = new Speaker("Speaker2", "127.0.0.1", connection);
            var testee = new Speaker("Speaker1", "127.0.0.1", connection);
            connectionMock.Setup(x => x.GetPowerStateAsync(otherSpeaker)).ReturnsAsync(PowerState.TurnedOn);
            connectionMock.Setup(x => x.GetCurrentContentAsync(otherSpeaker)).ReturnsAsync(contentItem);
            connectionMock.Setup(x => x.GetCurrentContentAsync(testee)).ReturnsAsync(contentItem);
            await testee.PlayAsync(contentItem);

            await testee.ShiftToSpeakerAsync(otherSpeaker);

            (await otherSpeaker.GetPowerStateAsync()).Should().Be(PowerState.TurnedOn);
            (await otherSpeaker.IsPlayingAsync()).Should().BeTrue();
            (await otherSpeaker.CurrentlyPlayingAsync()).Should().Be(contentItem);
            (await testee.GetPowerStateAsync()).Should().Be(PowerState.TurnedOff);
            connectionMock.Verify(x => x.PlayAsync(otherSpeaker, contentItem), Times.Once);
        }

        [Test]
        public async Task TurnOff()
        {
            var connectionMock = new Mock<IConnection>();
            var testee = new Speaker("Speaker1", "127.0.0.1", connectionMock.Object);

            await testee.TurnOffAsync();

            connectionMock.Verify(x => x.TurnOffAsync(testee), Times.Once);
            (await testee.GetPowerStateAsync()).Should().Be(PowerState.TurnedOff);
        }

        [Test]
        public async Task Play()
        {
            var contentItem = new ContentItem();
            var connectionMock = new Mock<IConnection>();
            var testee = new Speaker("Speaker1", "127.0.0.1", connectionMock.Object);
            connectionMock.Setup(x => x.GetCurrentContentAsync(testee)).ReturnsAsync(contentItem);

            await testee.PlayAsync(contentItem);

            connectionMock.Verify(x => x.PlayAsync(testee, contentItem), Times.Once);
            (await testee.CurrentlyPlayingAsync()).Should().Be(contentItem);
            (await testee.IsPlayingAsync()).Should().BeTrue();
        }
    }
}