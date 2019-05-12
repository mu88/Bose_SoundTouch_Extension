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
            var otherSpeaker = new Speaker("Speaker2", "127.0.0.1", connection);
            var testee = new Speaker("Speaker1", "127.0.0.1", connection);
            connectionMock.Setup(x => x.GetPowerStateAsync(otherSpeaker)).ReturnsAsync(PowerState.TurnedOn);
            connectionMock.Setup(x => x.GetCurrentContentAsync(otherSpeaker)).ReturnsAsync(content);
            connectionMock.Setup(x => x.GetCurrentContentAsync(testee)).ReturnsAsync(content);
            await testee.PlayAsync(content);

            await testee.ShiftToSpeakerAsync(otherSpeaker);

            otherSpeaker.PowerState.Should().Be(PowerState.TurnedOn);
            otherSpeaker.IsPlaying.Should().BeTrue();
            otherSpeaker.CurrentlyPlaying.Should().Be(content);
            testee.PowerState.Should().Be(PowerState.TurnedOff);
            connectionMock.Verify(x => x.PlayAsync(otherSpeaker, content), Times.Once);
        }

        [Test]
        public async Task TurnOff()
        {
            var connectionMock = new Mock<IConnection>();
            var testee = new Speaker("Speaker1", "127.0.0.1", connectionMock.Object);

            await testee.TurnOffAsync();

            connectionMock.Verify(x => x.TurnOffAsync(testee), Times.Once);
            testee.PowerState.Should().Be(PowerState.TurnedOff);
        }

        [Test]
        public async Task Play()
        {
            var content = new Mock<IContent>().Object;
            var connectionMock = new Mock<IConnection>();
            var testee = new Speaker("Speaker1", "127.0.0.1", connectionMock.Object);
            connectionMock.Setup(x => x.GetCurrentContentAsync(testee)).ReturnsAsync(content);

            await testee.PlayAsync(content);

            connectionMock.Verify(x => x.PlayAsync(testee, content), Times.Once);
            testee.CurrentlyPlaying.Should().Be(content);
            testee.IsPlaying.Should().BeTrue();
        }
    }
}