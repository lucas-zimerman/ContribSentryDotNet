using ContribSentry.Enums;
using ContribSentry.Interface;
using ContribSentry.Internals;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ContribSentry.TracingTest.UseCasesTests
{
    public class SimpleScenarioTests
    {
        public IContribSentryTracingService Service;

        private void SetupService()
        {
            Service = new ContribSentryTracingService(null);
            Service.Init(null);
        }

        private async Task GetRequest(int fixedDelay, string url)
        {
            using (Service.StartChild(url, ESpanRequest.Get))
            {
                var random = new Random();
                await Task.Delay(fixedDelay);
            }
        }

        [Fact]
        public async Task ScenarioFinishPaymentTests()
        {
            try
            {
                SetupService();
                ISentryTracing _tCreditCardValid;
                ISentryTracing _tStorageRequest;
                ISentryTracing _tAnalytics;
                ISentryTracing _tFrontEnd;
                var creditCardValidUrl = "https://validcard.com";
                var storageUrl = "https://storage.com";
                var analyticsUrl = "https://analytics.com";
                var frontUrl = "https://front.com";

                using (_tCreditCardValid = Service.StartTransaction("CreditCardValidation"))
                {
                    await _tCreditCardValid.IsolateTracking(async () =>
                    {
                        await Task.Delay(55);
                        await GetRequest(500, creditCardValidUrl);
                    });
                }
                //lets assume it's a valid credit card
                using (_tStorageRequest = Service.StartTransaction("Storage Request"))
                {
                    await _tStorageRequest.IsolateTracking(async () =>
                    {
                        await Task.Delay(50);
                        await GetRequest(1500, storageUrl);
                    });
                }

                var tasks = new Task[2];

                _tAnalytics = Service.StartTransaction("analytics");
                tasks[0] = _tAnalytics.IsolateTracking(async () =>
                {
                    await Task.Delay(50);
                    _ = GetRequest(60, analyticsUrl);
                });
                await Task.Delay(50);

                _tFrontEnd = Service.StartTransaction("front");
                tasks[1] = _tFrontEnd.IsolateTracking(async () =>
                {
                    await Task.Delay(50);
                    _ = GetRequest(30, frontUrl);
                });

                Task.WaitAll(tasks);

                _tAnalytics.Finish();
                _tFrontEnd.Finish();

                Assert.NotNull(_tCreditCardValid.Spans.FirstOrDefault(p => p.Description.Contains(creditCardValidUrl)));
                Assert.NotNull(_tStorageRequest.Spans.FirstOrDefault(p => p.Description.Contains(storageUrl)));
                Assert.NotNull(_tAnalytics.Spans.FirstOrDefault(p => p.Description.Contains(analyticsUrl)));
                Assert.NotNull(_tFrontEnd.Spans.FirstOrDefault(p => p.Description.Contains(frontUrl)));
            }
            finally
            {
                Service = null;
            }
        }
    }
}
