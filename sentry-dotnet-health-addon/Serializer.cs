using Newtonsoft.Json;
using Sentry.Protocol;
using sentry_dotnet_health_addon.Internals;
using System;
using System.IO;
using System.Text;

namespace sentry_dotnet_health_addon
{
    public class Serializer
    {
        internal Encoding utf8 = Encoding.UTF8;

        JsonSerializerSettings jsonSettings;
        
        public Serializer()
        {
            jsonSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public void Serialize(ISession session, Stream writer)
        {
            var json = JsonConvert.SerializeObject(session, jsonSettings);
            writer.Write(utf8.GetBytes(json), 0, json.Length);
            writer.Flush();
        }

        internal void Serialize(SentryEnvelope envelope, Stream writer)
        {
            var nextLineArray = new Byte[1] { 10 };
            var header = (SentryId.Empty.Equals(envelope.Header.EventId) ? "{}" : JsonConvert.SerializeObject(envelope.Header, jsonSettings));
            writer.Write(utf8.GetBytes(header), 0, header.Length);
            writer.Write(nextLineArray, 0, 1);
            foreach (var item in envelope.Items)
            {
                var itemTypeJson = JsonConvert.SerializeObject(item.Type, jsonSettings);
                writer.Write(utf8.GetBytes(itemTypeJson), 0, itemTypeJson.Length);
                writer.Write(nextLineArray, 0, 1);
                CopyBytesByKb(item.Data, writer);
                writer.Write(nextLineArray, 0, 1);
            }
            writer.Flush();
        }


        private void CopyBytesByKb(byte[] data, Stream writer)
        {
            long size = data.Length - 1024;
            int offset = 0;
            for (; offset < size; offset += 1024)
            {
                writer.Write(data, offset, 1024);
            }
            writer.Write(data, offset, data.Length - offset);
        }
    }
}
