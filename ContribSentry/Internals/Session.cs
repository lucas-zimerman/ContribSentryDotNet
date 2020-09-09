using Newtonsoft.Json;
using Sentry.Protocol;
using ContribSentry.Enums;
using ContribSentry.Extensions;
using System;

namespace ContribSentry.Internals
{
    public class Session : ISession
    {
        /** started timestamp */
        [JsonProperty("started")]
        public DateTime? Started { get; private set; }

        /** the timestamp */
        [JsonProperty("timestamp")]
        public DateTime? Timestamp { get; private set; }

        /** the number of errors on the session */
        [JsonProperty("errors ")]
        public int? ErrorCount { get; private set; }

        /** The distinctId, did */
        [JsonProperty("did")]
        public string DistinctId { get; private set; }

        /** the SessionId, sid */
        [JsonProperty("sid")]
        public Guid SessionId { get; private set; }

        /** The session init flag */
        [JsonProperty("init")]
        public bool? Init { get; private set; }

        /** The session state */
        [JsonIgnore]
        public ESessionState Status { get; set; }

        [JsonProperty("status")]
        internal string _statusJson => Status.ConvertString();

        /** The session sequence */
        [JsonIgnore]
        public long? Sequence { get; private set; }

        /** The session duration (timestamp - started) */
        [JsonProperty("duration")]
        public long? Duration { get; private set; }

        /** User Attributes. **/
        [JsonProperty("attrs")]
        public SessionAttributes Attributes { get; private set; }
        /** The session lock, ops should be atomic */
        [JsonIgnore]
        internal object _sessionLock = new object();

        public Session(
            ESessionState status,
            DateTime? started,
            DateTime? timestamp,
            int errorCount,
            string distinctId,
            Guid sessionId,
            bool? init,
            long? sequence,
            long? duration,
            string ipAddress,
            string userAgent,
            string environment,
            string release)
        {
            Status = status;
            Started = started;
            Timestamp = timestamp;
            ErrorCount = errorCount;
            DistinctId = distinctId;
            SessionId = sessionId;
            Init = init;
            Sequence = sequence;
            Duration = duration;
            Attributes = new SessionAttributes(ipAddress, userAgent, environment, release);
        }

        public Session(
            string distinctId,
            User user,
            string environment,
            string release)
        {
            Start(distinctId, user, environment, release);
        }

        public void Start(string distinctId,
            User user,
            string environment,
            string release)
        {
            Status = ESessionState.Ok;
            Started = DateTime.UtcNow;
            Timestamp = Started;
            ErrorCount = 0;
            DistinctId = distinctId;
            SessionId = Guid.NewGuid();
            Init = true;
            Sequence = null;
            Duration = null;
            Attributes = new SessionAttributes(user != null ? user.IpAddress : null,
            null, environment, release);
        }

        internal string SentryUsertoUserAgent(User user)
        {
            if (user?.Id != null)
                return user.Id;
            if (user?.Email != null)
                return user.Email;
            if (user?.Username != null)
                return user.Username;
            return null;
        }

        /// <summary>
        /// Ends a session and update its values.
        /// </summary>
        /// <param name="timestamp">the timestamp or null</param>
        public void End(DateTime? timestamp = null)
        {
            lock (_sessionLock)
            {
               // _init = null;

                // at this state it might be Crashed already, so we don't check for it.
                if (Status == ESessionState.Ok)
                {
                    Status = ESessionState.Exited;
                }

                if (timestamp != null)
                {
                    Timestamp = timestamp;

                    Duration = CalculateDurationTime(Timestamp.Value);
                    Sequence = GetSequenceTimestamp(Timestamp.Value);
                }
                else
                {
                    Timestamp = DateTime.UtcNow;
                }

            }
        }

        /// <summary>
        /// Calculates the duration time in seconds timestamp (last update) - started
        /// </summary>
        /// <param name="timestamp">the timestamp</param>
        /// <returns>duration in seconds</returns>
        private long CalculateDurationTime(DateTime timestamp)
        {
            return (long)(timestamp.TotalUtcSeconds() - Started.Value.TotalUtcSeconds());
        }

        /// <summary>
        /// Updates the current session and set its values.
        /// </summary>
        /// <param name="status">the status.</param>
        /// <param name="userAgent">the userAgent.</param>
        /// <param name="addErrorsCount">true if should increase error count or not.</param>
        /// <returns>true if the session has been updated.</returns>
        internal bool Update(ESessionState? status, string userAgent, bool addErrorsCount)
        {
            lock (_sessionLock)
            {
                bool sessionHasBeenUpdated = false;
                if (status != null)
                {
                    Status = status.Value;
                    sessionHasBeenUpdated = true;
                }

                if (userAgent != null)
                {
                    Attributes.UserAgent = userAgent;
                    sessionHasBeenUpdated = true;
                }
                if (addErrorsCount)
                {
                    ErrorCount++;
                    sessionHasBeenUpdated = true;
                }

                if (sessionHasBeenUpdated)
                {
                    Init = null;
                    Timestamp = DateTime.UtcNow;
                    if (Timestamp != null)
                    {
                        Sequence = GetSequenceTimestamp(Timestamp.Value);
                    }
                }
                return sessionHasBeenUpdated;
            }
        }


        /// <summary>
        /// Returns a logical clock.
        /// </summary>
        /// <param name="timestamp">The timestamp</param>
        /// <returns>time stamp in milliseconds UTC</returns>
        private long GetSequenceTimestamp(DateTime timestamp)
        {
            long sequence = (long)timestamp.ToUniversalTime().TotalUtcMiliseconds();
            // if device has wrong date and time and it is nearly at the beginning of the epoch time.
            // when converting GMT to UTC may give a negative value.
            if (sequence < 0)
            {
                sequence = Math.Abs(sequence);
            }
            return sequence;
        }

        public void RegisterError()
        {
            ErrorCount++;
        }

        internal Session Clone()
        {
            return new Session(
                Status,
                Started,
                Timestamp,
                ErrorCount.GetValueOrDefault(),
                DistinctId,
                SessionId,
                Init,
                Sequence,
                Duration,
                Attributes.IpAddress,
                Attributes.UserAgent,
                Attributes.Environment,
                Attributes.Release);
        }
    }
}
