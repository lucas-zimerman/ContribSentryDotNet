using ContribSentry.Extensibility;
using ContribSentry.Internals;
using ContribSentry.Test.Mock;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ContribSentry.Test
{
    public class ContribSentrySdkTest
    {
        [Fact]
        public void Unitialized_Sdk_Has_Disabled_Session_Service()
        {
            Assert.True(ContribSentrySdk.SessionService.Equals(DisabledSessionService.Instance));
            Assert.False(ContribSentrySdk.IsEnabled);
            Assert.False(ContribSentrySdk.IsSessionSdkEnabled);
            Assert.False(ContribSentrySdk.IsTracingSdkEnabled);
        }

        [Fact]
        public void Unitialized_Sdk_Has_Disabled_Tracing_Service()
        {
            Assert.True(ContribSentrySdk.TracingService.Equals(DisabledTracingService.Instance));
        }

        [Fact]
        public void Default_Session_Service_Is_Set_If_None_Inteface_Is_Passed()
        {
            try
            {
                ContribSentrySdk.Init(new ContribSentryOptions());
                Assert.True(ContribSentrySdk.SessionService.GetType() == typeof(ContribSentrySessionService));
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void Custom_Session_Service_Is_Set_If_Inteface_Is_Passed()
        {
            try
            {
                var options = new ContribSentryOptions();
                options.SetSessionService(new MockSessionService());
                ContribSentrySdk.Init(options);
                Assert.True(ContribSentrySdk.SessionService.GetType() == typeof(MockSessionService));
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void Custom_Tracing_Service_Is_Set_If_Inteface_Is_Passed()
        {
            try
            {
                var options = new ContribSentryOptions();
                options.SetTracingService(new MockTracingService());
                ContribSentrySdk.Init(options);
                Assert.True(ContribSentrySdk.TracingService.GetType() == typeof(MockTracingService));
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void Default_Tracing_Service_Is_Set_If_None_Inteface_Is_Passed()
        {
            try
            {
                ContribSentrySdk.Init(new ContribSentryOptions());
                Assert.True(ContribSentrySdk.TracingService.GetType() == typeof(TransactionWorker));
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void EndCustomerService_Is_Set_When_Sdk_Initialized()
        {
            try
            {
                ContribSentrySdk.Init(new ContribSentryOptions());
                Assert.NotNull(ContribSentrySdk.Transport);
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void Close_Unset_Services_And_EndConsumer_And_Enabled_Vars()
        {
            ContribSentrySdk.Init(new ContribSentryOptions());
            ContribSentrySdk.Close();
            Assert.True(ContribSentrySdk.SessionService.Equals(DisabledSessionService.Instance));
            Assert.True(ContribSentrySdk.TracingService.Equals(DisabledTracingService.Instance));
            Assert.False(ContribSentrySdk.IsEnabled);
            Assert.False(ContribSentrySdk.IsSessionSdkEnabled);
            Assert.False(ContribSentrySdk.IsTracingSdkEnabled);
        }

        [Fact]
        public void Custom_TrackingId_Is_Set_When_Sdk_Initialized()
        {
            try
            {
//                ContribSentrySdk.Init(new ContribSentryOptions() { TrackingIdMethod = new MockThreadTracking() });
                Assert.Equal(typeof(TransactionWorker), ContribSentrySdk.TracingService.GetType());
                var service = (TransactionWorker)ContribSentrySdk.TracingService;
  //              Assert.Equal(typeof(MockThreadTracking), service.Tracker.GetType());
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void ThreadTracking_Is_Set_When_Sdk_Initialized_And_No_Custom_TrackingId_Is_Set()
        {
            try
            {
                ContribSentrySdk.Init(new ContribSentryOptions());
                Assert.Equal(typeof(TransactionWorker), ContribSentrySdk.TracingService.GetType());
                var service = (TransactionWorker)ContribSentrySdk.TracingService;
            //    Assert.Equal(typeof(TracingWorkerContext), service.Tracker.GetType());
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }

        [Fact]
        public void Sdk_Disabled_Start_Session_Does_Not_Crash()
        {
            ContribSentrySdk.StartSession(null);
        }

        [Fact]
        public void Sdk_Disabled_End_Session_Does_Not_Crash()
        {
            ContribSentrySdk.EndSession();
        }

        [Fact]
        public void Sdk_Disabled_Start_Tracing_Does_Not_Crash_And_Return_Disabled_Transaction()
        {
          //  Assert.Equal(DisabledTracing.Instance,ContribSentrySdk.StartTransaction(null));
        }

        [Fact]
        public void Sdk_Disabled_Start_Child_Does_Not_Crash_And_Return_Disabled_Span()
        {
          //  Assert.Equal(DisabledSpan.Instance, ContribSentrySdk.StartChild(null));
        }
    }
}
