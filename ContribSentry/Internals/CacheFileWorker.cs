using ContribSentry.Extensibility;
using Sentry;
using Sentry.Protocol;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ContribSentry.Internals
{
    internal class CacheFileWorker
    {
        private CancellationTokenSource _tokenSource {get;set;}
        private ContribSentryOptions _options;

        internal CacheFileWorker(ContribSentryOptions options)
        {
            _tokenSource = new CancellationTokenSource();
            _options = options;
        }

        internal bool StartWorker() 
        {
            if (!Directory.Exists(_options.CacheDirPath))
            {
                try
                {
                    Directory.CreateDirectory(_options.CacheDirPath);
                }
                catch (Exception ex)
                {
                    _options.DiagnosticLogger?.Log(SentryLevel.Error, "ContribSentry Failed createfolder on {0}", ex, new object[1] { ContribSentrySdk.Options.CacheDirPath });
                    _options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Disabling Event and Envelope Cache");
                    ContribSentrySdk.EventCache = DisabledDiskCache.Instance;
                    ContribSentrySdk.EnvelopeCache = DisabledEnvelopeCache.Instance;
                    return false;
                }
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    _options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Checking for Stored Envelope Cache");
                    var list = ContribSentrySdk.EnvelopeCache.Iterator();
                    _options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Preparing to send Envelope Cache Data");
                    foreach (var item in list)
                    {
                        var sent = await ContribSentrySdk.Transport.CaptureCachedEnvelope(item);
                        await Task.Delay(1000);
                        if (sent)
                        {
                            ContribSentrySdk.EnvelopeCache.Discard(item);
                        }
                        else
                        {
                            break;
                        }
                    }

                    _options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Checking for Stored Event Cache");
                    list = ContribSentrySdk.EventCache.Iterator();
                    _options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Preparing to send Envelope Cache Data");
                    foreach (var item in list)
                    {
                        var sent = ContribSentrySdk.Transport.CaptureCachedEnvelope(item).Result;
                        await Task.Delay(1000);
                        if (sent)
                        {
                            ContribSentrySdk.EventCache.Discard(item);
                        }
                        else
                        {
                            break;
                        }
                    }

                    _options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry FileWorker is now sleeping");
                    await Task.Delay(120000);
                }
            }, _tokenSource.Token);
            return true;
        }

        ~CacheFileWorker()
        {
            _options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Closing FileWorker");
            _tokenSource.Cancel();
        }

    }
}
