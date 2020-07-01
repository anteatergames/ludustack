using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.Models
{
    public class GiveawayEntry
    {
        public GiveawayEntryType Type { get; set; }

        public int Points { get; set; }
    }
}