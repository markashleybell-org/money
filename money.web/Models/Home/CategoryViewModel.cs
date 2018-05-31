using System;

namespace money.web.Models
{
    public class CategoryViewModel
    {
        public int ID { get; set; }

        public int AccountID { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }

        public decimal Spent { get; set; }

        public decimal Remaining { get; set; }

        public int PercentageSpent =>
            (Spent == 0 || Amount == 0) ? 0 : (int)Math.Round((100 * Spent) / Amount);
    }
}
