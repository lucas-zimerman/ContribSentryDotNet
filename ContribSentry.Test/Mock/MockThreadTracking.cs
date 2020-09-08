using ContribSentry.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContribSentry.Test.Mock
{
    public class MockThreadTracking : ITracingContextTrackingId
    {
        public bool Created => true;

        public void AssociateId(int id) { }
        public int? GetId() => null;
        public bool IdRegistered(int id) => true;

        public int ReserveNewId() => 0;

        public bool UnsetId(int id) => true;

        public Task WithIsolatedTracing(Func<Task> test, int id) => new Task(null);
    }
}
