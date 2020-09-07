using ContribSentry.Interface;
using ContribSentry.Internals;
using Moq;
using System.Threading;
using Xunit;

namespace ContribSentry.SessionTest.Internals
{
    public class ContribSentrySessionServiceTest
    {
        private ContribSentrySessionService GetGlobalSession(IEndConsumerService endConsumer)
        {
            var sessionService = new ContribSentrySessionService();
            sessionService.Init(new ContribSentryOptions() { GlobalSessionMode = true }, endConsumer);
            return sessionService;
        }

        [Fact]
        public void EndSession_Sends_Session_To_End_Consumer()
        {
            var evt = new ManualResetEvent(false);
            var httpMoq = new Mock<IEndConsumerService>();
            httpMoq.Setup(p => p.CaptureSession(It.IsAny<ISession>())).Callback(() => evt.Set());
            var session = GetGlobalSession(httpMoq.Object);
            session.StartSession(null, null, null, null);
            session.EndSession();
            Assert.True(evt.WaitOne(1000));
        }

        [Fact]
        public void EndSession_Twice_Sends_Only_One_Session_To_End_Consumer()
        {
            var evt = new ManualResetEvent(false);
            var httpMoq = new Mock<IEndConsumerService>();
            httpMoq.Setup(p => p.CaptureSession(It.IsAny<ISession>())).Callback(() => { evt.Set(); });
            var session = GetGlobalSession(httpMoq.Object);
            session.StartSession(null, null, null, null);

            session.EndSession();
            Assert.True(evt.WaitOne(10));

            evt = new ManualResetEvent(false);
            session.EndSession();
            Assert.False(evt.WaitOne(10));
        }

        [Fact]
        public void EndSession_Without_Start_Doesnt_Send_Session_To_End_Consumer()
        {
            var called = false;
            var httpMoq = new Mock<IEndConsumerService>();
            httpMoq.Setup(p => p.CaptureSession(It.IsAny<ISession>())).Callback(() => called = true);
            var session = GetGlobalSession(httpMoq.Object);
            session.EndSession();
            Assert.False(called);
        }

        [Fact]
        public void EndSession_With_Session_Close_Call_Send_Session_To_End_Consumer()
        {
            var evt = new ManualResetEvent(false);
            var httpMoq = new Mock<IEndConsumerService>();
            httpMoq.Setup(p => p.CaptureSession(It.IsAny<ISession>())).Callback(() => evt.Set());
            var session = GetGlobalSession(httpMoq.Object);
            session.StartSession(null, null, null, null);
            session.Close();
            Assert.True(evt.WaitOne(1000));
        }//SessionContainerGlobal

        [Fact]
        public void Init_With_Global_Session_Creates_Global_Container()
        {
            var evt = new ManualResetEvent(false);
            var httpMoq = new Mock<IEndConsumerService>();
            httpMoq.Setup(p => p.CaptureSession(It.IsAny<ISession>())).Callback(() => evt.Set());
            var session = GetGlobalSession(httpMoq.Object);
            Assert.True(session.HealthContainer.GetType() == typeof(SessionContainerGlobal));
        }
    }
}
