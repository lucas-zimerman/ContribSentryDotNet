using System;
using System.Threading.Tasks;

namespace ContribSentry.Interface
{
    internal interface ITracingContextTrackingId
    {
        bool UnsetId(int id);
        int ReserveNewId();
        void AssociateId(int id);
        bool IdRegistered(int id);
        int? GetId();
        Task WithIsolatedTracing(Func<Task> test, int id);
    }
}
