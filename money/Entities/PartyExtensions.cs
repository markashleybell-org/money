namespace money.Entities
{
    public static class PartyExtensions
    {
        public static Party WithUpdates(
            this Party party,
            string name)
            => new Party(
                id: party.ID,
                accountId: party.AccountID,
                name: name
            );
    }
}
