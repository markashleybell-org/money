using System;
using System.Collections.Generic;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class ListEntriesEntryViewModel
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public DateTime Date { get; set; }
        public string Party { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
    }
}
