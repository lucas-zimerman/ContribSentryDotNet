using ContribSentry.Cache;
using Sentry;
using System.Collections.Generic;

namespace ContribSentry.Interface
{
    /// <summary>
    /// Implementations of this interface are used as a kind of persistent storage for events that wait<br/>
    /// to be sent to the Sentry server.<br/>
    ///<br/>
    /// <p>Note that this interface doesn't handle the situation of resending the stored events after a<br/>
    /// crash.While that is surely one of the main usecases for the persistent storage of events, the<br/>
    /// re-initialization is out of scope of the event transport logic.
    /// </summary>
    public interface IEventCache
    {
        /// <summary>
        /// Stores the event so that it can be sent later.
        /// </summary>
        /// <param name="event">the event to store</param>
        void Store(SentryEvent @event);

        /// <summary>
        /// Discards the event from the storage. This means that the event has been successfully sent. Note<br/>
        /// that this MUST NOT fail on events that haven't been stored before (i.e. this method is called<br/>
        /// even for events that has been sent on the first attempt).
        /// </summary>
        /// <param name="event">the event to discard from storage</param>
        void Discard(SentryEvent @event);
        void Discard(CachedSentryData @event);

        List<CachedSentryData> Iterator();
    }
}
