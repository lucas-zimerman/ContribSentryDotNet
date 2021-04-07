using ContribSentry.Internals;
using Sentry;
using System.Collections.Generic;

namespace ContribSentry.Transaction
{
    public static class SentryTracingExtensions
    {
        /// <summary>
        /// Sets the extra key-value pairs to the <see cref="BaseScope"/>.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="values">The values.</param>
        public static void SetExtras(this ISentryTracing tracing, IEnumerable<KeyValuePair<string, object>> values)
        {
            var extra = tracing.Extra;
            foreach (var keyValuePair in values)
            {
                _ = extra.AddOrUpdate(keyValuePair.Key, keyValuePair.Value, (s, o) => keyValuePair.Value);
            }
        }

        internal static void SetSentryEvent(this SentryTracingEvent tracing, SentryEvent sentryEvent)
        {
            tracing.Breadcrumbs = sentryEvent.Breadcrumbs;
            tracing.Contexts = sentryEvent.Contexts;
            tracing.Environment = sentryEvent.Environment;
            tracing.Extra = sentryEvent.Extra;
            tracing.EventId = sentryEvent.EventId;
            tracing.Fingerprint = sentryEvent.Fingerprint;
            tracing.Level = sentryEvent.Level;
            tracing.Release = sentryEvent.Release;
            tracing.Request = sentryEvent.Request;
            tracing.Sdk = sentryEvent.Sdk;
            tracing.Tags = sentryEvent.Tags;
            tracing.User = sentryEvent.User;
            tracing.Contexts.TryRemove(SentryTracing.TracingEventMessageKey, out _);
        }
    }
}
