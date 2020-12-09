using ContribSentry;
using Sentry;
using System;
using System.Threading.Tasks;

namespace Performance.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            SentrySdk.Init(o =>
            {
                o.AddIntegration(new ContribSentrySdkIntegration(new ContribSentryOptions() {RegisterTracingBreadcrumb = false }));
                o.Dsn = "https://80aed643f81249d4bed3e30687b310ab@o447951.ingest.sentry.io/5428537";
                o.Debug = true;
            });
            var operation1 = Task.Run(() =>
            {
                ContribSentrySdk.StartTransaction("Test Init", (transaction) =>
                {
                    var task = new Task[]
                    {
                    Level1(),
                    Level2(),
                    Level3()
                    };
                    Task.WaitAll(task);
                });
            });

            var operation2 = Task.Run(() =>
            {
                ContribSentrySdk.StartTransaction("Sync", (transaction) =>
                {
                    var task = new Task[]
                    {
                    Sync()
                    };
                    Task.WaitAll(task);
                });
            });
            Task.WaitAll(new Task[] { operation1, operation2 });
            System.Console.WriteLine("Hello World!");
            System.Console.ReadKey();
        }

        public static async Task Level1()
            => await ContribSentrySdk.StartChild("Test Init Level1", func: async (span) =>
                {
                    //Do stuff
                    await Task.Delay(350);
                    await SubTaskLevel();
                });

        public static async Task Sync()
            => await ContribSentrySdk.StartChild("Test Sync", func: async (span) =>
            {
                //Do stuff
                await Task.Delay(350);
                await SubTaskLevel();
                await SubTaskLevel();
                await SubTaskLevel();
                await SubTaskLevel();
                await SubTaskLevel();
            });

        public static async Task Level2()
            => await ContribSentrySdk.StartChild("Test Init Level2", func: async (span) =>
             {
                //Do stuff
                await Task.Delay(250);
                 await SubTaskLevel();
             });

        public static async Task Level3()
            => await ContribSentrySdk.StartChild("Test Init Level3", func: async (span) =>
            {
                //Do stuff
                await Task.Delay(330);
                await SubTaskLevel();
            });

        public static async Task SubTaskLevel()
            => await ContribSentrySdk.StartChild("A request", func: async (span) =>
            {
                //Do stuff
                await Task.Delay(50);
                await SubSubTaskLevel();

                ContribSentrySdk.Sleep();
                await Task.Delay(5000);
                ContribSentrySdk.Resume();
                
                await SubSubTaskLevel();
                await SubSubTaskLevel();
            });


        public static async Task SubSubTaskLevel()
            => await ContribSentrySdk.StartChild("A request", func: async (span) =>
            {
                //Do stuff
                await Task.Delay(50);
            });
    }
}
