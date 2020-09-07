using Sentry.Protocol;

namespace ContribSentry.Interface
{
    public interface IContribSentrySessionService
    {
        /// <summary>
        /// Initialize the Session Service.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="endConsumer">The internal Class used for sending Sessions to Sentry.</param>
        void Init(ContribSentryOptions options, IEndConsumerService endConsumer);
        void Close();

        /// <summary>
        /// Start a new Session.<br/>
        /// Note: This function shouldn't be called by the developer.<br/>
        /// developer must call StartSession from ContribSentrySdk.
        /// </summary>
        /// <param name="user">The Sentry user Object</param>
        /// <param name="distinctId">The distinctId.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="release">The release</param>
        void StartSession(User user, string distinctId, string environment, string release);

        /// <summary>
        /// End a new Session.<br/>
        /// Note: This function shouldn't be called by the developer.<br/>
        /// developer must call EndSession from ContribSentrySdk.
        /// </summary>
        /// <param name="user">The Sentry user Object</param>
        /// <param name="distinctId">The distinctId.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="release">The release</param>
        void EndSession();

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns></returns>
        ISession GetCurrent();
    }
}
