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
        internal static readonly string PrefixCurrentSessionFile = "session";
        /// <summary>
        /// Unused.
        /// </summary>
        internal static readonly string CrashMarkerFile = ".sentry-native/last_crash";

        private string _directory;
        private int _maxSize;

        internal EnvelopeCache(ContribSentryOptions options)
        {
            _directory = options.CacheDirPath;
            _maxSize = options.CacheDirSize;
        }

        public void Store(CachedSentryData envelope)
        {
            if (GetNumberOfStoredEnvelopes() < _maxSize)
            {
                if (envelope.Type == ESentryType.Session)
                    StoreSession(envelope);
                else if(envelope.Type == ESentryType.CurrentSession)
                    StoreCurrentSession(envelope);
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

        private void StoreCurrentSession(CachedSentryData sessionEnvelope)
        {
            var envelopePath = GetCurrentSessionPath();
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
            var currentSessionPath = GetCurrentSessionPath();
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
            //Get Current Session
            if (File.Exists(currentSessionPath))
            {
                try
                {
                    var data = File.ReadAllBytes(currentSessionPath);
                    list.Add(new CachedSentryData(Guid.Empty, data, ESentryType.CurrentSession));
                }
                catch { }
            }
            return list;
        }

        public void Discard(CachedSentryData envelope)
        {
            var @envelopePath = GetPath(envelope);
            if (File.Exists(@envelopePath))
            {
                File.Delete(@envelopePath);
                ContribSentrySdk.Options?.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry {envelope.Type} removed from Cache.");
            }
        }

        public string GetPath(CachedSentryData data)
        {
            if (data.Type == ESentryType.Transaction)
                return GetEnvelopePath(data);
            if (data.Type == ESentryType.Session)
                return GetSessionPath(data);
            return GetCurrentSessionPath();
        }
        public string GetEnvelopePath(CachedSentryData envelope) => $"{_directory}/{envelope.EventId}{SufixEnvelopeFile}";
        private string GetSessionPath(CachedSentryData envelope) => $"{_directory}/{envelope.EventId}{SufixSessionFile}";
        private string GetCurrentSessionPath() => $"{_directory}/{PrefixCurrentSessionFile}";
        private string GetEventIdFromPath(string path) => path.Replace($"{_directory}/", "").Replace(SufixEnvelopeFile, "").Replace(SufixSessionFile,"");
        private int GetNumberOfStoredEnvelopes() => AllEnvelopesFileNames().Count();
        private IEnumerable<string> AllEnvelopesFileNames() => Directory.EnumerateFiles(_directory, $"*{SufixEnvelopeFile}");
        private IEnumerable<string> AllSessionFileNames() => Directory.EnumerateFiles(_directory, $"*{SufixSessionFile}");

    }
}
