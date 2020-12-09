using System;
using System.Threading;
using ContribSentry;
using Sentry;

namespace DotNetCoreConsoleSession
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var sentryOptions = new SentryOptions()
            {
                Environment = "development",
                Dsn = "https://1b869b04656740518013bc2e9d5753b7@o188313.ingest.sentry.io/5458365",
                Debug = true,
                Release = $"ContribSentrySamples X"
            };
            var contribOptions = new ContribSentryOptions() 
            { 
                GlobalSessionMode = true,
                DistinctId = "1234"
            };
            sentryOptions.AddIntegration(new ContribSentrySdkIntegration(contribOptions));
            SentrySdk.Init(sentryOptions);

            Console.WriteLine("Logging Healthy Session");
            RegisterHealthySession();
            Thread.Sleep(1000);

            Console.WriteLine("Logging Errored Session");
            RegisterErroedSession();
            Thread.Sleep(1000);

            Console.WriteLine("Logging Crashed Session");
            RegisterCrashedSession();
        }

        static void RegisterHealthySession()
        {
            ContribSentrySdk.StartSession();
            var r = new Random();
            Thread.Sleep(r.Next(1000, 3000));
            ContribSentrySdk.EndSession();
        }

        static void RegisterErroedSession()
        {
            ContribSentrySdk.StartSession();
            var r = new Random();
            SentrySdk.CaptureException(new Exception("Moq Error"));
            Thread.Sleep(r.Next(1000, 3000));
            ContribSentrySdk.EndSession();
        }

        static void RegisterCrashedSession()
        {
            ContribSentrySdk.StartSession();
            var i = 0;
            var j = 1 / i;
            //Internally it'll register a crashed session since the app crashed.
        }
    }
}
