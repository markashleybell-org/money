using Microsoft.AspNetCore.Html;
using Money.Entities;
using Money.Models;

namespace Money.Support
{
    public static class ModelExtensions
    {
        public static string NumberLast4DigitsForDisplay(this IAccount account)
        {
            if (string.IsNullOrWhiteSpace(account.NumberLast4Digits))
            {
                return default;
            }

            switch (account.Type)
            {
                case AccountType.CreditCard:
                    return $"●●●● ●●●● ●●●● {account.NumberLast4Digits}";
                case AccountType.Current:
                case AccountType.Savings:
                    return $"●●●●{account.NumberLast4Digits}";
                default:
                    return default;
            }
        }

        public static HtmlString NameWithDeletedStatus(this ISoftDeletableLookupData item) =>
            item.Deleted
                ? new HtmlString($"<span class=\"soft-deleted\">{item.Name}</span>")
                : new HtmlString(item.Name);
    }
}
