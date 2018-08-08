using money.web.Models.Entities;

namespace money.web.Abstract
{
    public interface IPersistentSessionManager
    {
        PersistentSession CreatePersistentSession(int userID);

        PersistentSession GetPersistentSession(PersistentSession session);

        PersistentSession UpdatePersistentSession(PersistentSession session);

        void DestroyPersistentSession(int userID, string seriesIdentifier);
    }
}
