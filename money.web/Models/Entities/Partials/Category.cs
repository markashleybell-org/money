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
}
