using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;

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