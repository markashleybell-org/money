using money.Entities;

namespace money.Support
{
    public interface IPersistentSessionManager
    {
        PersistentSession CreatePersistentSession(int userID);

        PersistentSession GetPersistentSession(PersistentSession session);

        PersistentSession UpdatePersistentSession(PersistentSession session);

        void DestroyPersistentSession(int userID, string seriesIdentifier);
    }
}
