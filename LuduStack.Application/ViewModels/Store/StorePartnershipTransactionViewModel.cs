using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Application.ViewModels.Store
{
    public class StorePartnershipTransactionViewModel : BaseViewModel
    {
        public StorePartnershipTransactionType Type { get; set; }

        public string TypeText { get; set; }

        public Guid OrderId { get; set; }

        public float Value { get; set; }
    }
}