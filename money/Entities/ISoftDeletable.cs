namespace Money.Entities
{
    public interface ISoftDeletable<T>
        where T : IEntity
    {
        bool Deleted { get; }

        T ForDeletion();

        T ForUndeletion();
    }
}
