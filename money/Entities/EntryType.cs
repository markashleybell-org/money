using System;

namespace Money.Entities
{
    [Flags]
    public enum EntryType
    {
        Debit = 1,
        Credit = 2,
        Transfer = 4
    }
}
