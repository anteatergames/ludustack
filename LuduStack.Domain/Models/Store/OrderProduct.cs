﻿using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class OrderProduct
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public float Quantity { get; set; }

        public float UnitValue { get; set; }

        public float ItemDiscount { get; set; }
    }
}