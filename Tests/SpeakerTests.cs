using System.Threading.Tasks;
using BusinessLogic;
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
            var content = new Mock<IContent>().Object;
            var connectionMock = new Mock<IConnection>();
            var connection = connectionMock.Object;
            var otherSpeaker = new Speaker("Speaker2", "127.0.0.1", "", connection);
            var testee = new Speaker("Speaker1", "127.0.0.1", "", connection);
            connectionMock.Setup(x => x.GetPowerStateAsync(otherSpeaker)).ReturnsAsync(PowerState.TurnedOn);
            connectionMock.Setup(x => x.GetCurrentContentAsync(otherSpeaker)).ReturnsAsync(content);
            connectionMock.Setup(x => x.GetCurrentContentAsync(testee)).ReturnsAsync(content);
            await testee.PlayAsync(content);

            await testee.ShiftToSpeakerAsync(otherSpeaker);

            (await otherSpeaker.GetPowerStateAsync()).Should().Be(PowerState.TurnedOn);
            (await otherSpeaker.IsPlayingAsync()).Should().BeTrue();
            (await otherSpeaker.CurrentlyPlayingAsync()).Should().Be(content);
            (await testee.GetPowerStateAsync()).Should().Be(PowerState.TurnedOff);
            connectionMock.Verify(x => x.PlayAsync(otherSpeaker, content), Times.Once);
        }

        [Test]
        public async Task TurnOff()
        {
            var connectionMock = new Mock<IConnection>();
            var testee = new Speaker("Speaker1", "127.0.0.1", "", connectionMock.Object);

            await testee.TurnOffAsync();

            connectionMock.Verify(x => x.TurnOffAsync(testee), Times.Once);
            (await testee.GetPowerStateAsync()).Should().Be(PowerState.TurnedOff);
        }

        [Test]
        public async Task Play()
        {
            var content = new Mock<IContent>().Object;
            var connectionMock = new Mock<IConnection>();
            var testee = new Speaker("Speaker1", "127.0.0.1", "", connectionMock.Object);
            connectionMock.Setup(x => x.GetCurrentContentAsync(testee)).ReturnsAsync(content);

            await testee.PlayAsync(content);

            connectionMock.Verify(x => x.PlayAsync(testee, content), Times.Once);
            (await testee.CurrentlyPlayingAsync()).Should().Be(content);
            (await testee.IsPlayingAsync()).Should().BeTrue();
        }
    }
}