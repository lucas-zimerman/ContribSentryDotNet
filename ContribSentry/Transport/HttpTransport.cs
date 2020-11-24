using ContribSentry.Cache;
using ContribSentry.Extensions;
using Sentry;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContribSentry.Transport
{
    public static class HttpTransport
    {
        internal static HttpClient Client = new HttpClient();

        public static async Task<bool> Send(CachedSentryData envelope)
        {
            var content = new ByteArrayContent(envelope.Data);
            content.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(envelope));
            var url = GetContentUrl(envelope);
            try
            {
                var ret = await Client.PostAsync(url, content);
                if(ret.StatusCode == HttpStatusCode.BadRequest)
                {
                    var builder = new StringBuilder();
                    builder.Append("ContribSentry Got an error while Sending ");
                    builder.Append(envelope.Type.ConvertString());
                    builder.Append("ID ");
                    builder.Append(envelope.EventId);
                    builder.Append("with status ");
                    builder.Append(ret.StatusCode);
                    builder.Append(", Server response: ");
                    var response = await ret.Content?.ReadAsStringAsync();
                    builder.Append(Regex.Replace(response, "[^A-Za-z0-9 ]", ""));

                    ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Error, builder.ToString());
                    return true; //used so we delete the problematic cache
                }
                ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Sent {0} ID {1} with status {2}", args: new object[] { envelope.Type, envelope.EventId, ret.StatusCode });
                return ret.StatusCode == HttpStatusCode.OK;
            }
            catch(Exception ex)
            {
                ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Failed to Send {0} ID {1}", ex , new object[] { envelope.Type, envelope.EventId });
                return false;
            }
        }

        private static string GetContentType(CachedSentryData data)
            => data.Type == Enums.ESentryType.Event ? "application/json" : "application/x-sentry-envelope";

        private static string GetContentUrl(CachedSentryData data)
            => data.Type == Enums.ESentryType.Event ? ContribSentrySdk.Options.ParsedDsn.Value.GetEventUrl() :
            ContribSentrySdk.Options.ParsedDsn.Value.GetEnvelopeUrl();
    }
}