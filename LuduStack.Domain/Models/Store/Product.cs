using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class Product : Entity
    {
        public string Code { get; set; }

        public string ParentCode { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public float Price { get; set; }

        public string Currency { get; set; }

        public string StoreHandler { get; set; }

        public ProductOrigin Origin { get; set; }

        public List<ProductVariant> Variants { get; set; }

        public List<StorePartner> Owners { get; set; }

        public Product()
        {
            Variants = new List<ProductVariant>();
        }
    }
}