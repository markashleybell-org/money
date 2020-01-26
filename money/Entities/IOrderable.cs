namespace Money.Entities
{
    public interface IOrderable<T>
         where T : IEntity
    {
        int DisplayOrder { get; }
    }
}
