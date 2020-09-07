using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContribSentry.Internals
{
    /// <summary>
    /// Used for attaching an unique Id for each Callback
    /// </summary>
    public class ThreadTracking
    {

        private AsyncLocal<int?> _tracingIds = new AsyncLocal<int?>();

        internal static object @lock = new object();
        internal static int _value;
        public static int InternalNewId()
        {
            lock (@lock)
            {
                int i = _value++;
                return i;
            }
        }

        public async Task StartCallbackTrackingIdAsync(Func<Task> test, int? unsafeId = null)
        {
            if(_tracingIds.Value != null)
            {
                return;
            }

            _tracingIds.Value = unsafeId ?? InternalNewId();
            await test().ConfigureAwait(false);
            _tracingIds.Value = null;
        }

        public bool Created => _tracingIds.Value != null;

        public int StartUnsafeTrackingId()
        {
            return InternalNewId();
        }

        public int? Id => _tracingIds.Value;

    }
}