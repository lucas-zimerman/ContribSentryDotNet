namespace ContribSentry.Internals
{
    internal interface ISessionContainer
    {
        Session GetCurrent();

        void CreateNewSession(Session session);

        void Clear();
    }
}
