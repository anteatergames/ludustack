using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class StorePartnership : Entity
    {
        public float FundsTotal { get; set; }

        public float FundsAvailable { get; set; }

        public float FundsRequested { get; set; }

        public float FundsWithdrawn { get; set; }

        public Guid PartnerUserId { get; set; }

        public List<StorePartnershipTransaction> Transactions { get; set; }

        public StorePartnership()
        {
            Transactions = new List<StorePartnershipTransaction>();
        }
    }
}