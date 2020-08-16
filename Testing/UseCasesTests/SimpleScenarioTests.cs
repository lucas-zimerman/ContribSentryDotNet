using sentry_dotnet_transaction_addon;
using sentry_dotnet_transaction_addon.Enums;
using sentry_dotnet_transaction_addon.Internals;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Testing.UseCasesTests
{
    public class SimpleScenarioTests
    {
        public SimpleScenarioTests()
        {
            Initializer.Init();
        }

        private async Task GetRequest(int fixedDelay, string url)
        {
            using (SentryTracingSdk.StartChild(url, ESpanRequest.Get))
            {
                var random = new Random();
                await Task.Delay(fixedDelay);
            }
        }

        [Fact]
        public async Task ScenarioFinishPaymentTests()
        {
            ISentryTracing _tCreditCardValid;
            ISentryTracing _tStorageRequest;
            ISentryTracing _tAnalytics;
            ISentryTracing _tFrontEnd;
            var creditCardValidUrl = "https://validcard.com";
            var storageUrl = "https://storage.com";
            var analyticsUrl = "https://analytics.com";
            var frontUrl = "https://front.com";

            using (_tCreditCardValid = SentryTracingSdk.StartTransaction("CreditCardValidation"))
            {
                await _tCreditCardValid.IsolateTracking(async () =>
                {
                    await Task.Delay(55);
                    await GetRequest(500, creditCardValidUrl);
                });
            }
            //lets assume it's a valid credit card
            using (_tStorageRequest = SentryTracingSdk.StartTransaction("Storage Request"))
            {
                await _tStorageRequest.IsolateTracking(async () =>
                {
                    await Task.Delay(50);
                    await GetRequest(1500, storageUrl);
                });
            }

            var tasks = new Task[2];

            _tAnalytics = SentryTracingSdk.StartTransaction("analytics");
            tasks[0] = _tAnalytics.IsolateTracking(async () =>
            {
                await Task.Delay(50);
                _ = GetRequest(60, analyticsUrl);
            });
            await Task.Delay(50);

            _tFrontEnd = SentryTracingSdk.StartTransaction("front");
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
    }
}
