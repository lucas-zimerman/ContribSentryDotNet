using ContribSentry.Internals;
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
    }
}
