using ContribSentry;
using ContribSentry.Interface;
using ContribSentry.Internals;
using Sentry.Protocol;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContribSentry.Cache
{
    internal class EnvelopeCache : IEnvelopeCache
    {
        //Class was based on SessionCache from Sentry.Android with minor changes.
        //Since right now there's only one item per envelope, the logic got simplified.

        internal static readonly string SufixEnvelopeFile = ".envelope";
        internal static readonly string PrefixCurrentSessionFile = ".session";
        /// <summary>
        /// Unused.
        /// </summary>
        internal static readonly string CrashMarkerFile = ".sentry-native/last_crash";

        private string _directory;
        private int _maxSize;
        private Serializer _serializer;

        internal EnvelopeCache(ContribSentryOptions options)
        {
            _directory = options.GetCacheDirPath();
            _maxSize = options.GetCacheDirSize();
            _serializer = options.GetSerializer();
        }

        public void Store(SentryEnvelope envelope)
        {
            if(GetNumberOfStoredEnvelopes() < _maxSize)
            {
                if (envelope.Items[0].Type.Type == Enums.ESentryType.Session)
                    StoreSession(envelope);
                else
                    StoreEnvelope(envelope);
            }
        }

        private void StoreSession(SentryEnvelope sessionEnvelope)
        {
            var envelopePath = GetSessionPath(sessionEnvelope);
            if (!File.Exists(envelopePath))
            {
                try
                {
                    using (var stream = File.Create(envelopePath))
                    {
                        _serializer.Serialize(sessionEnvelope, stream);
                    }
                }
                catch { }
            }
        }

        private void StoreEnvelope(SentryEnvelope envelope)
        {
            var envelopePath = GetEnvelopePath(envelope);
            if (!File.Exists(envelopePath))
            {
                try
                {
                    using (var stream = File.Create(envelopePath))
                    {
                        _serializer.Serialize(envelope, stream);
                    }
                }
                catch { }
            }
        }

        public void Discard(SentryEnvelope envelope)
        {
            var @envelopePath = envelope.Header.EventId.Equals(SentryId.Empty) ?GetSessionPath(envelope) : GetEventPath(@event);
            if (File.Exists(eventPath))
            {
                File.Delete(eventPath);
            }
        }

        private string GetPath(SentryEnvelope envelope)
        {
            var sufix = envelope.Header.EventId.Equals(SentryId.Empty) ? PrefixCurrentSessionFile : SufixEnvelopeFile;
            return $"{_directory}/{envelope.Header.EventId}{sufix}";
        }
        private string GetEnvelopePath(CachedSentryData @event) => $"{_directory}/{@event.EventId}{SufixEnvelopeFile}";
        private string GetSessionPath(CachedSentryData @event) => $"{_directory}/{@event.EventId}";
        private string GetEventIdFromPath(string path) => path.Replace($"{_directory}/", "").Replace(SufixEnvelopeFile, "");
        private int GetNumberOfStoredEnvelopes() => AllEnvelopesFileNames().Count();
        private IEnumerable<string> AllEnvelopesFileNames() => Directory.EnumerateFiles(_directory, SufixEnvelopeFile);

    }
}
