namespace Money.Support
{
    public interface ISoftDeletableLookupData
    {
        string Name { get; }

        bool Deleted { get; }
    }
}
