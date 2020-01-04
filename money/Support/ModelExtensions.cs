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
                case AccountType.Current:
                case AccountType.Savings:
                    return $"[●●{account.NumberLast4Digits}]";
                default:
                    return default;
            }
        }

        public static HtmlString SpanForNumberLast4Digits(this AccountViewModel account)
        {
            var displayString = account.NumberLast4DigitsForDisplay();

            return !string.IsNullOrWhiteSpace(displayString)
                ? new HtmlString($"<span class=\"last-4\">{displayString}</span>")
                : new HtmlString(string.Empty);
        }

        public static HtmlString NameWithDeletedStatus(this ISoftDeletableLookupData item) =>
            item.Deleted
                ? new HtmlString($"<span class=\"soft-deleted\">{item.Name}</span>")
                : new HtmlString(item.Name);
    }
}
