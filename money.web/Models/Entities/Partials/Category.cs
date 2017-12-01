using money.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace money.web.Models.Entities
{
    public partial class Category
    {
        public Category(int accountID, string name)
        {
            AccountID = accountID;
            Name = name;
        }
    }

    public static class CategoryExtensions
    {
        public static Category WithUpdates(this Category category, string name)
        {
            return new Category(
                category.ID,
                category.AccountID,
                name,
                category.DisplayOrder
            );
        }
    }
}