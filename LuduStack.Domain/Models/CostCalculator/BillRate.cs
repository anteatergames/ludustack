using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class BillRate : Entity
    {
        public BillRateType BillRateType { get; set; }

        public ArtStyle? ArtStyle { get; set; }

        public SoundStyle? SoundStyle { get; set; }

        public GameElement GameElement { get; set; }

        public decimal HourPrice { get; set; }

        public int HourQuantity { get; set; }
    }
}