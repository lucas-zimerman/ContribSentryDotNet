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
            System.Console.WriteLine("Estou 3");
            return Session;
        }

        public void Clear()
        {
            Session = null;
        }
    }
}
