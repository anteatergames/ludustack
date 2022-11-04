using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class Order : Entity
    {
        public string Number { get; set; }

        public string StoreOrderNumber { get; set; }

        public float Discount { get; set; }

        public float FreightValue { get; set; }

        public float TotalProductsValue { get; set; }

        public float TotalOrderValue { get; set; }

        public List<OrderProduct> Items { get; set; }

        public OrderOrigin Origin { get; set; }

        public OrderSituation Situation { get; set; }

        public Order()
        {
            Items = new List<OrderProduct>();
        }
    }
}