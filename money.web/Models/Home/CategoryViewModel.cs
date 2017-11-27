using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}