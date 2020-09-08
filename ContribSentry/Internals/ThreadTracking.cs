using ContribSentry.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContribSentry.Internals
{
    /// <summary>
    /// Used for attaching an unique Id for each Callback
    /// </summary>
    public class ThreadTracking : ITracingContextTrackingId
    {

        private AsyncLocal<int?> _tracingIds = new AsyncLocal<int?>();

        private object _lock = new object();
        private int _value;
        public int InternalNewId()
        {
            lock (_lock)
            {
                int i = _value++;
                return i;
            }
        }
      
        public bool UnsetId(int id)
        {
            if(_tracingIds.Value != null)
            {
                _tracingIds.Value = null;
                return true;
            }
            _tracingIds.Value = null;
            return false;
        }

        public int ReserveNewId()
        {
            return InternalNewId();
        }

        public void AssociateId(int id) 
        {
            _tracingIds.Value = id;
        }

        public bool IdRegistered(int id) => _tracingIds.Value != null;

        public int? GetId() => _tracingIds.Value;

        public async Task WithIsolatedTracing(Func<Task> test, int id)
        {
            _tracingIds.Value = id;
            await test().ConfigureAwait(false);
            _tracingIds.Value = null;
        }
    }
}