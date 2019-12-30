using System;

namespace Money.Entities
{
    [Flags]
    public enum EntryType
    {
        Unknown = 0,
        Debit = 1,
        Credit = 2,
        Transfer = 4
    }
}
