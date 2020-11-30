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
                o.AddIntegration(new ContribSentrySdkIntegration());
                o.Dsn = "https://1b869b04656740518013bc2e9d5753b7@o188313.ingest.sentry.io/5458365";
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
