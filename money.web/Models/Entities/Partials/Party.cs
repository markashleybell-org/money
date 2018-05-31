namespace money.web.Models.Entities
{
    public partial class Party
    {
        public Party(int accountID, string name)
        {
            AccountID = accountID;
            Name = name;
        }
    }
}
