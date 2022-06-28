using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Application.ViewModels.Store
{
    public class OrderProductViewModel
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public float Quantity { get; set; }

        public float UnitValue { get; set; }

        public float ItemDiscount { get; set; }
    }
}
