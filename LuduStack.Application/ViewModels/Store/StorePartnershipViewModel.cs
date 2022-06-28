using LuduStack.Application.ViewModels.User;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Store
{
    public class StorePartnershipViewModel : BaseViewModel
    {
        public float FundsTotal { get; set; }

        public float FundsAvailable { get; set; }

        public float FundsRequested { get; set; }

        public float FundsWithdrawn { get; set; }

        public Guid PartnerUserId { get; set; }

        public List<StorePartnershipTransactionViewModel> Transactions { get; set; }

        public ProfileViewModel PartnerProfile { get; set; }

        public StorePartnershipViewModel()
        {
            Transactions = new List<StorePartnershipTransactionViewModel>();
        }
    }
}