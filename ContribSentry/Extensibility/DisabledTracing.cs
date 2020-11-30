using ContribSentry.Enums;
using ContribSentry.Interface;
using ContribSentry.Internals;
using Sentry;
using Sentry.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContribSentry.Extensibility
{
    public class DisabledTracing : ISentryTracing
    {
        public static DisabledTracing Instance = new DisabledTracing();
        public List<ISpanBase> Spans => null;

        public DateTimeOffset StartTimestamp { get; }

        public ConcurrentDictionary<string, object> Extra => null;

        public IScopeOptions ScopeOptions => throw new NotImplementedException();

        public SentryLevel? Level { get; set; }
        public string Transaction { get; set; }
        public Request Request { get; set; }
        public Contexts Contexts { get; set; }
        public User User { get; set; }
        public string Environment { get; set; }

        public SdkVersion Sdk { get; }

        public IEnumerable<string> Fingerprint { get; set; }

        public IEnumerable<Breadcrumb> Breadcrumbs { get; }

        IReadOnlyDictionary<string, object> IScope.Extra { get; }

        public IReadOnlyDictionary<string, string> Tags { get; }

        public void Dispose() { }
        public void Finish() { }

        public ISpanBase GetCurrentSpan() => DisabledSpan.Instance;

        public ISpanBase GetSpan(string op) => DisabledSpan.Instance;

        public ISpanBase StartChild(string description, string op = null) => DisabledSpan.Instance;

        public ISpanBase StartChild(string url, ESpanRequest requestType) => DisabledSpan.Instance;

        /// <summary>
        /// Despite being disabled we must execute the user code
        /// </summary>
        /// <returns>A task where the user code is running</returns>
        public Task IsolateTracking(Func<Task> trackedCode)
        {
            return trackedCode.Invoke();
        }

        public void Finish(Exception ex) { }
    }
}
