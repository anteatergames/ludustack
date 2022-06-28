using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Store
{
    public class ProductViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "SKU")]
        public string Code { get; set; }

        public string ParentCode { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public float Price { get; set; }

        public string Currency { get; set; }

        public string StoreHandler { get; set; }

        public ProductOrigin Origin { get; set; }

        public List<ProductVariantViewModel> Variants { get; set; }

        [Display(Name = "Product Owners", Description = "These are the owners of this product.")]
        public List<StorePartnerViewModel> Owners { get; set; }

        public List<ProfileViewModel> OwnersProfiles { get; set; }
        public string StoreUrl { get; internal set; }

        public ProductViewModel()
        {
            Variants = new List<ProductVariantViewModel>();
        }
    }
}