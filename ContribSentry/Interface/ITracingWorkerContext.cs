using System;

namespace ContribSentry.Interface
{
    internal interface ITracingWorkerContext
    {
        void Run(Action callback);
    }
}
