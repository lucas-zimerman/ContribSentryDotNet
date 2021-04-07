using Newtonsoft.Json;
using Sentry;
using ContribSentry.Interface;
using System;
using System.Collections.Generic;
using ContribSentry.Internals;

namespace ContribSentry
{
    public class SentryTracingEvent : IEventLike
    {

        [JsonConverter(typeof(SentryIdJsonConverter))]
        [JsonProperty("event_id")]
        public SentryId EventId { get; set; }

        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("spans")]
        public List<ISpanBase> Spans { get; private set; }

        [JsonProperty("start_timestamp")]
        public DateTimeOffset StartTimestamp { get; private set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("level")]
        public SentryLevel? Level { get; set; }

        [JsonConverter(typeof(SentryNewtonsoftJsonConverter))]
        [JsonProperty("request")]
        public Request Request { get ; set; }

        [JsonConverter(typeof(SentryNewtonsoftJsonConverter))]
        [JsonProperty("contexts")]
        public Contexts Contexts { get; set; }

        [JsonConverter(typeof(SentryNewtonsoftJsonConverter))]
        [JsonProperty("user")]
        public User User { get ; set ; }

        [JsonProperty("release")]
        public string Release { get ; set ; }

        [JsonProperty("environment")]
        public string Environment { get ; set ; }

        [JsonProperty("transaction")]
        public string TransactionName { get ; set ; }

        [JsonProperty("sdk")]
        public SdkVersion Sdk { get; set; }

        [JsonProperty("fingerprint")]

        public IReadOnlyList<string> Fingerprint { get ; set ; }

        private List<Breadcrumb> _breadcrumbs { get; set; }

        [JsonProperty("breadcrumbs")]
        public IReadOnlyCollection<Breadcrumb> Breadcrumbs => _breadcrumbs;

        private Dictionary<string, string> _tags { get; set; }

        [JsonProperty("tags")]
        public IReadOnlyDictionary<string, string> Tags => _tags;


        private Dictionary<string, object> _extra;

        [JsonProperty("extra")]
        public IReadOnlyDictionary<string, object> Extra => _extra;

        [JsonProperty("platform")]
        public string Platform { get ; set ; }

        [JsonIgnore]
        internal Trace Trace { get; }

        internal SentryTracingEvent(SentryTracing transactionEvent, bool hasError)
        {
            TransactionName = transactionEvent.Transaction;
            Type = "transaction";

            EventId = SentryId.Create();
            Level = hasError ? SentryLevel.Error : SentryLevel.Info;
            Trace = transactionEvent.Trace;
            Spans = transactionEvent.Spans;
            StartTimestamp = transactionEvent.StartTimestamp;
            Timestamp = DateTimeOffset.UtcNow;
            Sdk = ContribSentrySdk.Options.ContribSdk;
            _extra = new Dictionary<string, object>();
            _tags = new Dictionary<string, string>();
            _breadcrumbs = new List<Breadcrumb>();
            this.SetExtras(transactionEvent.Extra);
        }

        public void AddBreadcrumb(Breadcrumb breadcrumb)
        {
            _breadcrumbs.Add(breadcrumb);
        }

        public void SetTag(string key, string value)
        {
            _tags[key] = value;
        }

        public void UnsetTag(string key)
        {
            _tags.Remove(key);
        }

        public void SetExtra(string key, object value)
        {
            _extra[key] = value;
        }
    }
}