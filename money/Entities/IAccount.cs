namespace Money.Entities
{
    public interface IAccount
    {
        AccountType Type { get; }

        string NumberLast4Digits { get; }
    }
}
