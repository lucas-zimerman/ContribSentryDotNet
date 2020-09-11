using ContribSentry.Interface;
using ContribSentry.Internals.EventProcessor;
using Moq;
using Sentry;
using System.Threading;
using Xunit;

namespace ContribSentry.Test.Internals.EventProcessor
{
    public class SentryOfflineEventProcessorTests
    {
        [Fact]
        public void IsConnected_Not_Set_Ignore_Cached_Events()
        {
            try
            {
                ContribSentrySdk.Init(new ContribSentryOptions());
                var processor = new SentryOfflineEventProcessor();
                var @event = new SentryEvent();
                Assert.NotNull(processor.Process(@event));
                Assert.Null(@event.Contexts.Device.IsOnline);
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void IsConnect_Dont_Ignore_Cache_And_Set_IsOnline_True()
        {
            try
            {
                var options = new ContribSentryOptions();
                options.SetHasInternetCallback(() => { return true; });
                ContribSentrySdk.Init(options);
                var processor = new SentryOfflineEventProcessor();
                var @event = new SentryEvent();
                Assert.NotNull(processor.Process(@event));
                Assert.True(@event.Contexts.Device.IsOnline);
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void NotConnect_Dont_Set_Cache_And_Return_Null()
        {
            try
            {
                var options = new ContribSentryOptions();
                options.SetHasInternetCallback(() => { return false; });
                ContribSentrySdk.Init(options);
                var processor = new SentryOfflineEventProcessor();
                var @event = new SentryEvent();
                Assert.Null(processor.Process(@event));
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void NotConnect_Calls_BeforeSend_And_Return_Null()
        {
            try
            {
                var options = new ContribSentryOptions();
                var evt = new ManualResetEvent(false);
                var evt2 = new ManualResetEvent(false);
                var mock = new Mock<IEventCache>();
                mock
                    .Setup(p => p.Store(It.IsAny<SentryEvent>()))
                    .Callback(() => evt2.Set());

                options.SetHasInternetCallback(() => { return false; });
                options.BeforeSend = (@event) => {
                    evt.Set();
                    return @event; 
                };
                ContribSentrySdk.Init(options);
                ContribSentrySdk.EventCache = mock.Object;

                var processor = new SentryOfflineEventProcessor();
                var @event = new SentryEvent();
                Assert.Null(processor.Process(@event));
                Assert.True(evt.WaitOne(100));
                Assert.True(evt2.WaitOne(100));
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void NotConnect_Calls_BeforeSend_And_DiscartCache()
        {
            try
            {
                var options = new ContribSentryOptions();
                var evt = new ManualResetEvent(false);
                var evt2 = new ManualResetEvent(false);
                var mock = new Mock<IEventCache>();
                mock
                    .Setup(p => p.Store(It.IsAny<SentryEvent>()))
                    .Callback(() => evt2.Set());

                options.SetHasInternetCallback(() => { return false; });
                options.BeforeSend = (@event) => {
                    evt.Set();
                    return null;
                };
                ContribSentrySdk.Init(options);
                ContribSentrySdk.EventCache = mock.Object;

                var processor = new SentryOfflineEventProcessor();
                var @event = new SentryEvent();
                Assert.Null(processor.Process(@event));
                Assert.True(evt.WaitOne(100));
                Assert.False(evt2.WaitOne(100));
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

    }
}
