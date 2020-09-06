namespace ContribSentry.Internals
{
    internal class SessionContainerGlobal : ISessionContainer
    {
        internal Session Session;

        public void CreateNewSession(Session session)
        {
            Session = session;
        }

        public Session GetCurrent()
        {
            return Session;
        }

        public void Clear()
        {
            Session = null;
        }
    }
}
