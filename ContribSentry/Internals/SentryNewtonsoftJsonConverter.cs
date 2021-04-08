using Newtonsoft.Json;
using Sentry;
using System;
using System.IO;

namespace ContribSentry.Internals
{
    class SentryNewtonsoftJsonConverter : JsonConverter
    {
        private SentryNewtonsoftByteConverter _translator = new SentryNewtonsoftByteConverter();

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is SdkVersion sdkVersion)
            {
                writer.WriteRawValue(System.Text.Encoding.UTF8.GetString(_translator.WriteSdkDataAsValue(sdkVersion)));
            }
            else if (value is Contexts contexts)
            {
                var v = System.Text.Encoding.UTF8.GetString(_translator.WriteContextsAsValue(contexts));
                writer.WriteRawValue(v);
            }
            else if (value is Request request)
            {
                    writer.WriteRawValue(System.Text.Encoding.UTF8.GetString(_translator.WriteRequestAsValue(request)));
            }
            else if (value is User user)
            {
                writer.WriteRawValue(System.Text.Encoding.UTF8.GetString(_translator.WriteUserAsValue(user)));
            }
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}