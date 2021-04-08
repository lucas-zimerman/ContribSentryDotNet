using ContribSentry.Extensibility;
using Sentry;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ContribSentry.Internals
{
    internal class CacheFileWorker
    {
        private CancellationTokenSource _tokenSource {get;set;}

        internal CacheFileWorker()
        {
            _tokenSource = new CancellationTokenSource();
        }

        internal bool StartWorker() 
        {
            if (!Directory.Exists(ContribSentrySdk.Options.CacheDirPath))
            {
                try
                {
                    Directory.CreateDirectory(ContribSentrySdk.Options.CacheDirPath);
                }
                catch (Exception ex)
                {
                    ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Failed createfolder on {ContribSentrySdk.Options.CacheDirPath} {ex.Message}");
                    ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Disabling Event and Envelope Cache");
                    ContribSentrySdk.EventCache = DisabledDiskCache.Instance;
                    ContribSentrySdk.EnvelopeCache = DisabledEnvelopeCache.Instance;
                    return false;
                }
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Checking for Stored Envelope Cache");
                    var list = ContribSentrySdk.EnvelopeCache.Iterator();
                    ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Preparing to send Envelope Cache Data");
                    foreach (var item in list)
                    {
                        var sent = await ContribSentrySdk.EndConsumer.CaptureCachedEnvelope(item);
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

                    ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Checking for Stored Event Cache");
                    list = ContribSentrySdk.EventCache.Iterator();
                    ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Preparing to send Envelope Cache Data");
                    foreach (var item in list)
                    {
                        var sent = ContribSentrySdk.EndConsumer.CaptureCachedEnvelope(item).Result;
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

                    ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry FileWorker is now sleeping");
                    await Task.Delay(120000);
                }
            }, _tokenSource.Token);
            return true;
        }

        ~CacheFileWorker()
        {
            ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Closing FileWorker");
            _tokenSource.Cancel();
        }

    }
}
