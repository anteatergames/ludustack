using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum SystemEventType
    {
        [UiInfo(Display = "Product Sync")]
        [Display(Name = "Product Sync")]
        ProductSync = 1,
        [UiInfo(Display = "Order Sync")]
        [Display(Name = "Order Sync")]
        OrderSync = 2,
        [UiInfo(Display = "Store Partnership Sync")]
        [Display(Name = "Store Partnership Sync")]
        StorePartnershipSync = 3
    }
}