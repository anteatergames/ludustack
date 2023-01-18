using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class StorePartnershipTransaction : Entity
    {
        public StorePartnershipTransactionType Type { get; set; }

        public Guid OrderId { get; set; }

        public float Value { get; set; }
    }
}