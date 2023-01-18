using LuduStack.Domain.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Store
{
    public class OrderViewModel : BaseViewModel
    {
        public string Number { get; set; }

        public string StoreOrderNumber { get; set; }


        [Display(Name = "Discount")]
        public float Discount { get; set; }

        [Display(Name = "Freight Value")]
        public float FreightValue { get; set; }

        [Required]
        [Display(Name = "Total Products Value")]
        public float TotalProductsValue { get; set; }

        [Required]
        [Display(Name = "Total Order Value")]
        public float TotalOrderValue { get; set; }

        public OrderSituation Situation { get; set; }

        public List<OrderProductViewModel> Items { get; set; }

        public OrderViewModel()
        {
            Items = new List<OrderProductViewModel>();
        }
    }
}