using Newtonsoft.Json;
using Sentry;
using ContribSentry.Interface;
using System;
using System.Collections.Generic;

namespace ContribSentry
{
    public class SentryTracingEvent : IEventLike
    {

        [JsonProperty("sentry_id")]
        public SentryId EventId { get; }

        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("spans")]
        public List<ISpanBase> Spans { get; private set; }

        [JsonProperty("start_timestamp")]
        public DateTimeOffset StartTimestamp { get; private set; }

        [JsonProperty("level")]
        public SentryLevel? Level { get; set; }

        [JsonProperty("request")]
        public Request Request { get ; set; }

        private Contexts _contexts;

        [JsonProperty("contexts")]
        public Contexts Contexts
        {
            get 
            {
                if(_contexts is null)
                {
                    _contexts = new Contexts();
                }
                return _contexts;
            }
            set => _contexts = value;
        }

        [JsonProperty("users")]
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

        [JsonProperty("breadcrumbs")]
        public IReadOnlyCollection<Breadcrumb> Breadcrumbs { get; set; }

        [JsonProperty("tags")]
        public IReadOnlyDictionary<string, string> Tags { get; set;}

        [JsonProperty("extra")]
        public IReadOnlyDictionary<string, object> Extra { get; set; }


        internal SentryTracingEvent(SentryTracing transactionEvent, bool hasError)
        {
            TransactionName = transactionEvent.Transaction;
            Type = "transaction";
            Level = hasError ? SentryLevel.Error : SentryLevel.Info;
            Contexts.AddOrUpdate("trace", transactionEvent.Trace, (id, trace) => trace);
            Spans = transactionEvent.Spans;
            StartTimestamp = transactionEvent.StartTimestamp;
            this.SetExtras(transactionEvent.Extra);
        }

        public void AddBreadcrumb(Breadcrumb breadcrumb)
        {
            throw new NotImplementedException();
        }

        public void SetTag(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void UnsetTag(string key)
        {
            throw new NotImplementedException();
        }

        public void SetExtra(string key, object value)
        {
            throw new NotImplementedException();
        }
    }
}