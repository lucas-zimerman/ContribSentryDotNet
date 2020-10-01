using ContribSentry.Enums;
using ContribSentry.Extensions;
using Newtonsoft.Json;
using System;

namespace ContribSentry.Internals
{
    internal class SentryTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ESentryType)value).ConvertString());
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}