using ContribSentry.Enums;
using ContribSentry.Interface;
using Sentry;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContribSentry.Cache
{
    /// <summary>
    /// A simple cache implementation storing the events to a disk, each event in a separater file in a<br/>
    /// configured directory.
    /// </summary>
    internal class DiskCache : IEventCache
    {
        //Class was based on DiskCache from Sentry.Android with minor changes like not requiring to parse again
        //the SentryEvent inside of the File.
        //Because if the Event ended up here, it's already a valid event to be send.


        /// <summary>
        /// File suffix added to all serialized event files.
        /// </summary>
        public static readonly string FileSufix = ".sentry-event";
        internal static readonly char PathSeparator = Path.DirectorySeparatorChar;

        private string _directory;
        private int _maxSize;
        private Serializer _serializer;

        internal DiskCache(ContribSentryOptions options)
        {
            _directory = options.CacheDirPath;
            _maxSize = options.CacheDirSize;
            _serializer = ContribSentrySdk.Serializer;
        }
            
        public void Store(SentryEvent @event)
        {
            if(GetNumberOfStoredEvents() < _maxSize)
            {
                var @eventPath = GetEventPath(@event);
                if (!File.Exists(eventPath))
                {
                    try
                    {
                        using (var stream = File.Create(eventPath))
                        {
                            _serializer.Serialize(@event, stream);
                        }
                    }
                    catch { }
                }
            }
        }

        public void Discard(SentryEvent @event)
        {
            var @eventPath = GetEventPath(@event);
            if (File.Exists(eventPath))
            {
                File.Delete(eventPath);
            }
        }
        public void Discard(CachedSentryData @event)
        {
            var @eventPath = GetEventPath(@event);
            if (File.Exists(eventPath))
            {
                File.Delete(eventPath);
                ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry event removed from Cache.");
            }
        }

        public List<CachedSentryData> Iterator(){
            var filePaths = AllEventFileNames();
            var list = new List<CachedSentryData>();
            foreach (var filePath in filePaths)
            {
                try
                {
                    var data = File.ReadAllBytes(filePath);
                    list.Add(new CachedSentryData(Guid.Parse(GetEventIdFromPath(filePath)), data, ESentryType.Event));
                }
                catch(UnauthorizedAccessException) { }
                catch
                {
                    File.Delete(filePath);
                }
            }
            return list;
        }

        private string GetEventPath(SentryEvent @event) => $"{_directory}{PathSeparator}{@event.EventId}{FileSufix}";
        private string GetEventPath(CachedSentryData @event) => $"{_directory}{PathSeparator}{@event.EventId}{FileSufix}";
        private string GetEventIdFromPath(string path)
        {
            var a = _directory.Length + 1;
            var b = path.LastIndexOf('.');
            return path.Substring(a, b - a);
        }

        private int GetNumberOfStoredEvents() => AllEventFileNames().Count();
        private IEnumerable<string> AllEventFileNames() => Directory.EnumerateFiles(_directory, $"*{FileSufix}");
    }
}