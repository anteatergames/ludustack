using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum StorePartnershipTransactionType
    {
        [UiInfo(Display = "Deposit")]
        [Display(Name = "Deposit")]
        Deposit = 1,

        [UiInfo(Display = "Withdraw")]
        [Display(Name = "Withdraw")]
        Withdraw = 2
    }
}