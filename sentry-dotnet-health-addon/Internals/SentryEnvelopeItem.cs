using Newtonsoft.Json;
using ContribSentry.Enums;
using System.IO;

namespace ContribSentry.Internals
{
    public class SentryEnvelopeItem
    {
        [JsonIgnore]
        internal SentryItemType Type {get; private set; }

        [JsonExtensionData]
        internal byte[] Data { get; private set; }

        public SentryEnvelopeItem(SentryItemType type, byte[] data)
        {
            Type = type;
            Data = data;
        }

        public static SentryEnvelopeItem FromSession(ISession session, Serializer serializer)
        {
            var memoryStream = new MemoryStream();
            serializer.Serialize(session, memoryStream);
            var array = memoryStream.ToArray();
            memoryStream.Close();
            return new SentryEnvelopeItem(new SentryItemType(ESentryType.Session), array);
        }

        public static SentryEnvelopeItem FromTransaction(SentryTracingEvent tracing, Serializer serializer)
        {
            var memoryStream = new MemoryStream();
            serializer.Serialize(tracing, memoryStream);
            var array = memoryStream.ToArray();
            memoryStream.Close();
            return new SentryEnvelopeItem(new SentryItemType(ESentryType.Transaction), array);
        }
    }
}