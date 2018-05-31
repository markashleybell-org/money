namespace money.web.Models.Entities
{
    public static class PartyExtensions
    {
        public static Party WithUpdates(this Party party, string name) => new Party(
                id: party.ID,
                accountID: party.AccountID,
                name: name
            );
    }
}
