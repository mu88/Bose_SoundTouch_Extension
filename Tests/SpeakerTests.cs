using BusinessLogic;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class SpeakerTests
    {
        [Test]
        public void ShiftToSpeaker()
        {
            var content = new Mock<IContent>().Object;
            var connectionMock = new Mock<IConnection>();
            var connection = connectionMock.Object;
            var otherSpeaker = new Speaker("Speaker2", "127.0.0.1", connection);
            var testee = new Speaker("Speaker1", "127.0.0.1", connection);
            connectionMock.Setup(x => x.GetPowerStateAsync(otherSpeaker)).Returns(PowerState.TurnedOn);
            connectionMock.Setup(x => x.GetCurrentContentAsync(otherSpeaker)).Returns(content);
            connectionMock.Setup(x => x.GetCurrentContentAsync(testee)).Returns(content);
            testee.PlayAsync(content);

            testee.ShiftToSpeakerAsync(otherSpeaker);

            otherSpeaker.GetPowerStateAsync().Should().Be(PowerState.TurnedOn);
            otherSpeaker.IsPlayingAsync().Should().BeTrue();
            otherSpeaker.CurrentlyPlayingAsync().Should().Be(content);
            testee.GetPowerStateAsync().Should().Be(PowerState.TurnedOff);
            connectionMock.Verify(x => x.PlayAsync(otherSpeaker, content), Times.Once);
        }

        [Test]
        public void TurnOff()
        {
            var connectionMock = new Mock<IConnection>();
            var testee = new Speaker("Speaker1", "127.0.0.1", connectionMock.Object);

            testee.TurnOffAsync();

            connectionMock.Verify(x => x.TurnOffAsync(testee), Times.Once);
            testee.GetPowerStateAsync().Should().Be(PowerState.TurnedOff);
        }

        [Test]
        public void Play()
        {
            var content = new Mock<IContent>().Object;
            var connectionMock = new Mock<IConnection>();
            var testee = new Speaker("Speaker1", "127.0.0.1", connectionMock.Object);
            connectionMock.Setup(x => x.GetCurrentContentAsync(testee)).Returns(content);

            testee.PlayAsync(content);

            connectionMock.Verify(x => x.PlayAsync(testee, content), Times.Once);
            testee.CurrentlyPlayingAsync().Should().Be(content);
            testee.IsPlayingAsync().Should().BeTrue();
        }
    }
}