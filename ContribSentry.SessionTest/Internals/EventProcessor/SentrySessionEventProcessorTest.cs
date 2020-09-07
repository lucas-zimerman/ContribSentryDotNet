using ContribSentry.Extensibility;
using ContribSentry.Interface;
using ContribSentry.Internals.EventProcessor;
using Moq;
using Sentry;
using Sentry.Protocol;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace ContribSentry.SessionTest.Internals.EventProcessor
{
    public class SentrySessionEventProcessorTest
    {
        [Fact]
        public void Error_Event_Not_Handled_Should_Close_Any_Opened_Session()
        {
            try
            {
                var evt = new ManualResetEvent(false);

                var mock = new Mock<IContribSentrySessionService>();
                mock.Setup(p => p.GetCurrent()).Returns(DisabledSession.Instance);
                ContribSentrySdk.SessionService = mock.Object;

                var mock2 = new Mock<IEndConsumerService>();
                mock2.Setup(p => p.CaptureSession(DisabledSession.Instance)).Callback(() => evt.Set());
                ContribSentrySdk.EndConsumer = mock2.Object;

                var @event = new SentryEvent();
                @event.Level = SentryLevel.Error;
                @event.SentryExceptions = new List<SentryException>()
                {
                    new SentryException(){Mechanism = new Mechanism(){ Handled = false}}
                };
                var processor = new SentrySessionEventProcessor();
                processor.Process(@event);
                Assert.True(evt.WaitOne(1000));
            }
            finally
            {
                ContribSentrySdk.SessionService = null;
                ContribSentrySdk.EndConsumer = null;
            }
        }

        [Fact]
        public void Fatal_Event_Not_Handled_Should_Close_Any_Opened_Session()
        {
            try
            {
                var evt = new ManualResetEvent(false);

                var mock = new Mock<IContribSentrySessionService>();
                mock.Setup(p => p.GetCurrent()).Returns(DisabledSession.Instance);
                ContribSentrySdk.SessionService = mock.Object;

                var mock2 = new Mock<IEndConsumerService>();
                mock2.Setup(p => p.CaptureSession(DisabledSession.Instance)).Callback(() => evt.Set());
                ContribSentrySdk.EndConsumer = mock2.Object;

                var @event = new SentryEvent();
                @event.Level = SentryLevel.Fatal;
                @event.SentryExceptions = new List<SentryException>()
                {
                    new SentryException(){Mechanism = new Mechanism(){ Handled = false}}
                };
                var processor = new SentrySessionEventProcessor();
                processor.Process(@event);
                Assert.True(evt.WaitOne(1000));
            }
            finally
            {
                ContribSentrySdk.SessionService = null;
                ContribSentrySdk.EndConsumer = null;
            }
        }

        [Fact]
        public void Error_Event_Handled_Should_Be_Ignored()
        {
            try
            {
                var evt = new ManualResetEvent(false);

                var mock = new Mock<IContribSentrySessionService>();
                mock.Setup(p => p.GetCurrent()).Returns(DisabledSession.Instance);
                ContribSentrySdk.SessionService = mock.Object;

                var mock2 = new Mock<IEndConsumerService>();
                mock2.Setup(p => p.CaptureSession(DisabledSession.Instance)).Callback(() => evt.Set());
                ContribSentrySdk.EndConsumer = mock2.Object;

                var @event = new SentryEvent();
                @event.Level = SentryLevel.Error;
                @event.SentryExceptions = new List<SentryException>()
                {
                    new SentryException(){Mechanism = new Mechanism(){ Handled = true}}
                };
                var processor = new SentrySessionEventProcessor();
                processor.Process(@event);
                Assert.False(evt.WaitOne(1000));
            }
            finally
            {
                ContribSentrySdk.SessionService = null;
                ContribSentrySdk.EndConsumer = null;
            }
        }

        [Fact]
        public void Info_Event_Should_Be_Ignored()
        {
            try
            {
                var evt = new ManualResetEvent(false);

                var mock = new Mock<IContribSentrySessionService>();
                mock.Setup(p => p.GetCurrent()).Returns(DisabledSession.Instance);
                ContribSentrySdk.SessionService = mock.Object;

                var mock2 = new Mock<IEndConsumerService>();
                mock2.Setup(p => p.CaptureSession(DisabledSession.Instance)).Callback(() => evt.Set());
                ContribSentrySdk.EndConsumer = mock2.Object;

                var @event = new SentryEvent();
                @event.Level = SentryLevel.Info;

                var processor = new SentrySessionEventProcessor();
                processor.Process(@event);
                Assert.False(evt.WaitOne(1000));
            }
            finally
            {
                ContribSentrySdk.SessionService = null;
                ContribSentrySdk.EndConsumer = null;
            }
        }
    }
}
