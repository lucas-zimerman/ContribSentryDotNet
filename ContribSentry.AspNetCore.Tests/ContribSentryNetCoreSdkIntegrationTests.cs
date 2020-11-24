using ContribSentry.AspNetCore.Internals;
using ContribSentry.Internals;
using Sentry;
using Xunit;

namespace ContribSentry.AspNetCore.Tests
{
    public class ContribSentryNetCoreSdkIntegrationTests
    {
        [Fact]
        public void Http_Tracking_Is_Set_On_Register_To_Main_Tracking_Service()
        {
            try
            {
                var integration = new ContribSentryNetCoreSdkIntegration();
                integration.Register(null,new SentryOptions());
                Assert.True(ContribSentrySdk.IsEnabled);
                Assert.True(ContribSentrySdk.IsTracingSdkEnabled);
                Assert.Equal(typeof(TransactionWorker), ContribSentrySdk.TracingService.GetType());
                var service = (TransactionWorker)ContribSentrySdk.TracingService;
                Assert.Equal(typeof(HttpContextTracking), service.Tracker.GetType());
            }
            finally
            {
                ContribSentrySdk.Close();
            }
        }
    }
}
