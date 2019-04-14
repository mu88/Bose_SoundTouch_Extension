using System.Net;
using System.Net.Mime;
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
            var otherSpeaker = new Speaker("Speaker2", connection);
            var testee = new Speaker("Speaker1", connection);
            connectionMock.Setup(x => x.GetPowerState(otherSpeaker)).Returns(PowerState.TurnedOn);
            connectionMock.Setup(x => x.GetCurrentContent(otherSpeaker)).Returns(content);
            connectionMock.Setup(x => x.GetCurrentContent(testee)).Returns(content);
            testee.Play(content);
            
            testee.ShiftToSpeaker(otherSpeaker);

            otherSpeaker.PowerState.Should().Be(PowerState.TurnedOn);
            otherSpeaker.IsPlaying.Should().BeTrue();
            otherSpeaker.CurrentlyPlaying.Should().Be(content);
            testee.PowerState.Should().Be(PowerState.TurnedOff);
            connectionMock.Verify(x=>x.Play(otherSpeaker, content), Times.Once);
        }

        [Test]
        public void TurnOff()
        {
            var connectionMock = new Mock<IConnection>();
            var testee = new Speaker("Speaker1", connectionMock.Object);

            testee.TurnOff();

            connectionMock.Verify(x=>x.TurnOff(testee), Times.Once);
            testee.PowerState.Should().Be(PowerState.TurnedOff);
        }

        [Test]
        public void Play()
        {
            var content = new Mock<IContent>().Object;
            var connectionMock = new Mock<IConnection>();
            var testee = new Speaker("Speaker1", connectionMock.Object);
            connectionMock.Setup(x => x.GetCurrentContent(testee)).Returns(content);
            
            testee.Play(content);

            connectionMock.Verify(x=>x.Play(testee, content), Times.Once);
            testee.CurrentlyPlaying.Should().Be(content);
            testee.IsPlaying.Should().BeTrue();
        }
    }
}