using ContribSentry.Internals;
using Sentry;
using System;
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
            foreach (var breadcrumb in sentryEvent.Breadcrumbs)
            {
                tracing.AddBreadcrumb(breadcrumb);
            }
            tracing.Contexts = sentryEvent.Contexts;
            tracing.Contexts.Trace.Operation = tracing.Trace.Op;
            tracing.Contexts.Trace.TraceId = Guid.Parse(tracing.Trace.TraceId);
            tracing.Contexts.Trace.SpanId = new SpanId(tracing.Trace.SpanId);
            tracing.Environment = sentryEvent.Environment;
            tracing.SetExtras(sentryEvent.Extra);
            tracing.Release = sentryEvent.Release;
            tracing.Request = sentryEvent.Request;
            tracing.SetTags(sentryEvent.Tags);
            tracing.User = sentryEvent.User;
            tracing.Contexts.TryRemove(SentryTracing.TracingEventMessageKey, out _);
        }
    }
}