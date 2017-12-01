using money.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

    public static class PartyExtensions
    {
        public static Party WithUpdates(this Party party, string name)
        {
            return new Party(
                id: party.ID,
                accountID: party.AccountID,
                name: name
            );
        }
    }
}