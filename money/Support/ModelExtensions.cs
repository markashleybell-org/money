using Microsoft.AspNetCore.Html;
using Money.Entities;

namespace Money.Support
{
    public static class ModelExtensions
    {
        public static string NumberLast4DigitsForDisplay(this IAccount account) =>
            string.IsNullOrWhiteSpace(account.NumberLast4Digits) ? default : account.Type switch {
                AccountType.CreditCard => $"●●●● {account.NumberLast4Digits}",
                AccountType.Current or AccountType.Savings => $"●●●●{account.NumberLast4Digits}",
                _ => default,
            };

        public static HtmlString NameWithDeletedStatus(this ISoftDeletableLookupData item) =>
            item.Deleted
                ? new HtmlString($"<span class=\"soft-deleted\">{item.Name}</span>")
                : new HtmlString(item.Name);

        public static HtmlString NameWithDeletedStatus(this Account account) =>
            account.Deleted
                ? new HtmlString($"<span class=\"soft-deleted\">{account.Name}</span>")
                : new HtmlString(account.Name);
    }
}
