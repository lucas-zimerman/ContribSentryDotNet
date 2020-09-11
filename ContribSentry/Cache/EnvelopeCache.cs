using ContribSentry.Enums;
using ContribSentry.Interface;
using Sentry.Protocol;
using System;
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
        internal static readonly string SufixSessionFile = ".session";
        /// <summary>
        /// Unused.
        /// </summary>
        internal static readonly string CrashMarkerFile = ".sentry-native/last_crash";

        private string _directory;
        private int _maxSize;
        private Serializer _serializer;

        internal EnvelopeCache(ContribSentryOptions options)
        {
            _directory = options.CacheDirPath;
            _maxSize = options.CacheDirSize;
            _serializer = ContribSentrySdk.Serializer;
        }

        public void Store(CachedSentryData envelope)
        {
            if (GetNumberOfStoredEnvelopes() < _maxSize)
            {
                if (envelope.Type == Enums.ESentryType.Session)
                    StoreSession(envelope);

                else
                    StoreEnvelope(envelope);
            }
        }

        private void StoreSession(CachedSentryData sessionEnvelope)
        {
            sessionEnvelope.EventId = Guid.NewGuid();
            var envelopePath = GetSessionPath(sessionEnvelope);
            if (!File.Exists(envelopePath))
            {
                try
                {
                    using (var stream = File.Create(envelopePath))
                    {
                        stream.Write(sessionEnvelope.Data, 0, sessionEnvelope.Data.Length);
                    }
                }
                catch { }
            }
        }

        private void StoreEnvelope(CachedSentryData envelope)
        {
            var envelopePath = GetEnvelopePath(envelope);
            if (!File.Exists(envelopePath))
            {
                try
                {
                    using (var stream = File.Create(envelopePath))
                    {
                        stream.Write(envelope.Data, 0, envelope.Data.Length);
                    }
                }
                catch { }
            }
        }

        public List<CachedSentryData> Iterator()
        {
            var envelopePaths = AllEnvelopesFileNames();
            var sessionPaths = AllSessionFileNames();
            var list = new List<CachedSentryData>();

            //Get All Envelopes
            foreach (var filePath in envelopePaths)
            {
                try
                {
                    var data = File.ReadAllBytes(filePath);
                    list.Add(new CachedSentryData(Guid.Empty, data, ESentryType.Transaction));
                }
                catch { }
            }

            //Get All Sessions
            foreach (var filePath in sessionPaths)
            {
                try
                {
                    var data = File.ReadAllBytes(filePath);
                    list.Add(new CachedSentryData(Guid.Parse(GetEventIdFromPath(filePath)), data, ESentryType.Session));
                }
                catch { }
            }
            return list;
        }

        public void Discard(CachedSentryData envelope)
        {
            var @envelopePath = envelope.Type == ESentryType.Session ? GetSessionPath(envelope) : GetEnvelopePath(envelope);
            if (File.Exists(@envelopePath))
            {
                File.Delete(@envelopePath);
                ContribSentrySdk.Options?.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry {envelope.Type} removed from Cache.");
            }
        }

        public string GetEnvelopePath(CachedSentryData envelope) => $"{_directory}/{envelope.EventId}{SufixEnvelopeFile}";
        private string GetSessionPath(CachedSentryData envelope) => $"{_directory}/{envelope.EventId}{SufixSessionFile}";
        private string GetEventIdFromPath(string path) => path.Replace($"{_directory}/", "").Replace(SufixEnvelopeFile, "").Replace(SufixSessionFile,"");
        private int GetNumberOfStoredEnvelopes() => AllEnvelopesFileNames().Count();
        private IEnumerable<string> AllEnvelopesFileNames() => Directory.EnumerateFiles(_directory, $"*{SufixEnvelopeFile}");
        private IEnumerable<string> AllSessionFileNames() => Directory.EnumerateFiles(_directory, $"*{SufixSessionFile}");

    }
}
