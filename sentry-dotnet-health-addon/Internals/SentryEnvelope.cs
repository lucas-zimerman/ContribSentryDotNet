﻿using Newtonsoft.Json;
using Sentry.Protocol;
using System.Collections.Generic;

namespace sentry_dotnet_health_addon.Internals
{

    public class SentryEnvelope
    {
        [JsonExtensionData]
        public SentryEnvelopeHeader Header { get; private set; }
        [JsonExtensionData]
        public List<SentryEnvelopeItem> Items { get; private set; }

        public SentryEnvelope(SentryEnvelopeHeader header, List<SentryEnvelopeItem> items)
        {
            Header = header;
            Items = items;
        }

        public SentryEnvelope(SentryId eventId, SdkVersion sdkVersion,
            List<SentryEnvelopeItem> items)
        {
            Header = new SentryEnvelopeHeader(eventId, sdkVersion);
            Items = items;
        }
        public SentryEnvelope(SentryId eventId, SdkVersion sdkVersion,
            SentryEnvelopeItem item)
        {
            Header = new SentryEnvelopeHeader(eventId, sdkVersion);
            Items = new List<SentryEnvelopeItem>() { item };
        }

        public static SentryEnvelope FromSession(ISession session, SdkVersion sdkVersion,
            Serializer serializer)
        {
            return new SentryEnvelope(SentryId.Empty, sdkVersion, SentryEnvelopeItem.FromSession(session, serializer));
        }
    }
}