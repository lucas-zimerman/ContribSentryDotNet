using ContribSentry.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContribSentry.Internals
{
    /// <summary>
    /// Used for attaching an unique Id for each Callback
    /// </summary>
    public class TracingWorkerContext : ITracingWorkerContext
    {
        public void Run(Action callback)
            => Task.Run()
    }
}