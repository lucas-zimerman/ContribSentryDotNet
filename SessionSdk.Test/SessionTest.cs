using Moq;
using Sentry.Protocol;
using ContribSentry;
using ContribSentry.Enums;
using ContribSentry.Internals;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SessionSdk.Test
{
    public class SessionTest
    {         
        [Fact]
        public void Session_Start_Should_Set_Default_Parameters()
        {
            var user = new User()
            {
                Id = "123",
                IpAddress = "127.0.0.1"
            };
            var session = CreateSession(user);
            Assert.Equal(user.IpAddress,session.Attributes.IpAddress);
            Assert.Equal(Initializer.TestRelease, session.Attributes.Release);
            Assert.Equal(Initializer.TestEnvironment, session.Attributes.Environment);
            Assert.Null(session.DistinctId);
            Assert.NotNull(session.Init);
            Assert.Equal(ESessionState.Ok, session.Status);
            Assert.NotNull(session.SessionId);
        }

        [Fact]
        public async Task Session_End_Status_Is_Exited_And_Timestamp_Higher_Than_Start()
        {
            var user = new User()
            {
                Id = "123",
                IpAddress = "127.0.0.1"
            };
            var session = CreateSession(user);
            await Task.Delay(10);
            session.End();
            Assert.Equal(ESessionState.Exited, session.Status);
            Assert.True(session.Timestamp > session.Started);
        }

        [Fact]
        private void Session_Crashed_When_Ended_Has_Status_Crashed()
        {
            var user = new User()
            {
                Id = "123",
                IpAddress = "127.0.0.1"
            };
            var session = CreateSession(user);
            session.Status = ESessionState.Crashed;
            session.End(null);
            Assert.Equal(ESessionState.Crashed, session.Status);
        }

        [Fact]
        private void Session_End_With_TimeStamp_Has_Timestamp()
        {
            var user = new User()
            {
                Id = "123",
                IpAddress = "127.0.0.1"
            };
            var session = CreateSession(user);
            var date = DateTime.Now.AddSeconds(5);
            session.End(date);
            Assert.Equal(date, session.Timestamp);
        }

        private Session CreateSession(User user)
        {
            return new Session(null, user, Initializer.TestEnvironment, Initializer.TestRelease);
        }
    }
}
