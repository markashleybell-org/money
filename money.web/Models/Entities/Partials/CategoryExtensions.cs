namespace money.web.Models.Entities
{
    public static class CategoryExtensions
    {
        public static Category WithUpdates(this Category category, string name) => new Category(
                category.ID,
                category.AccountID,
                name,
                category.DisplayOrder
            );
    }
}
