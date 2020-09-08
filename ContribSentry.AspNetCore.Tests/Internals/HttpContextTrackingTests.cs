using ContribSentry;
using ContribSentry.AspNetCore.Internals;
using ContribSentry.Testing;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Sentry.AspNetCore;
using Sentry.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ContribSentry.AspNetCore.Tests.Internals
{
    public class HttpContextTrackingTests : ContribSentrySdkTestFixture
    {
        protected Action<SentryAspNetCoreOptions> Configure;

        public HttpContextTrackingTests()
        {
            ConfigureWehHost = builder =>
            {
                _ = builder.UseSentry(options =>
                {
                    options.Dsn = DsnHelper.ValidDsnWithoutSecret;
                });
            };
        }

        private static HttpContextTracking _tracker;

        private static List<KeyValuePair<string, int>> _trackerList;

        private Action<HttpContext> TrackerIdMiddleware = (context) => {
            var id = _tracker.ReserveNewId();
            _tracker.AssociateId(id);
            _trackerList.Add(_tracker._contextReference.Last(p => p.Value == id));
        };

        [Fact]
        public async Task Tracker_Receives_Unique_KeyPair_ForEach_Request()
        {
            _tracker = new HttpContextTracking();
            _trackerList = new List<KeyValuePair<string, int>>();
            Build(TrackerIdMiddleware);
            _ = await HttpClient.GetAsync("");
            _ = await HttpClient.GetAsync("");
            _ = await HttpClient.GetAsync("");
            _ = await HttpClient.GetAsync("");
            _ = await HttpClient.GetAsync("");
            foreach(var valuePair in _trackerList)
            {
                Assert.Equal(1, _trackerList.Count(p => p.Key == valuePair.Key));
                Assert.Equal(1, _trackerList.Count(p => p.Value == valuePair.Value));
            }
            foreach (var valuePair in _trackerList)
            {
                Assert.True(_tracker.UnsetId(valuePair.Value));
            }
        }
    }
}
