using Sentry;
using System.IO;
using System.Text.Json;

namespace ContribSentry.Internals
{
    internal class SentryNewtonsoftByteConverter
    {
        internal byte[] WriteSdkDataAsValue(SdkVersion version)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false }))
                {
                    version.WriteTo(writer);
                }
                return ms.ToArray();
            }
        }

        internal byte[] WriteContextsAsValue(Contexts contexts)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false }))
                {
                    contexts.WriteTo(writer);
                }
                return ms.ToArray();
            }
        }

        internal byte[] WriteRequestAsValue(Request request)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false }))
                {
                    request.WriteTo(writer);
                }
                return ms.ToArray();
            }
        }

        internal byte[] WriteUserAsValue(User user)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false }))
                {
                    user.WriteTo(writer);
                }
                return ms.ToArray();
            }
        }

    }
}
