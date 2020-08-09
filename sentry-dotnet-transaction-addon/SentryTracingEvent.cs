using Newtonsoft.Json;
using Sentry;
using sentry_dotnet_transaction_addon.Converters;
using sentry_dotnet_transaction_addon.Extensions;
using sentry_dotnet_transaction_addon.Interface;
using sentry_dotnet_transaction_addon.Internals;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace sentry_dotnet_transaction_addon
{
    public class SentryTracingEvent : SentryEvent
    {
        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("spans")]
        public List<ISpanBase> Spans { get; private set; }

        [JsonProperty("start_timestamp")]
        public DateTimeOffset StartTimestamp { get; private set; }

        internal SentryTracingEvent(SentryTracing transactionEvent)
        {
            Transaction = transactionEvent.Transaction;
            Type = "transaction";
            Contexts.AddOrUpdate("trace", transactionEvent.Trace, (id, trace) => trace);
            Spans = transactionEvent.Spans;
            StartTimestamp = transactionEvent.StartTimestamp;
        }

        public void SendTransaction()
        {
            Level = null;
            var json = JsonConvert.SerializeObject(this, new JsonDateEpochConverter());
            new Task(async () =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var @event = JsonConvert.SerializeObject(new EventIdSentData()
                        {
                            EventId = EventId.ToString(),
                            SentAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:MM:ss.ffZ")
                        });
                        var @type = "{\"type\":\"transaction\"}";
                        var content = new StringContent(@event + '\n' + @type + '\n' + json);
                        var url = SentryTracingSdk.TracingOptions.Dsn.GetTracingUrl();
                        var @return = await client.PostAsync(url, content);
                    }
                }
                finally { }
            }).Start();
        }
    }
}