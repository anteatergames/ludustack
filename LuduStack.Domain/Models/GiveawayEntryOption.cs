using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class GiveawayEntryOption : Entity
    {
        public GiveawayEntryType Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public int Points { get; set; }

        public bool IsMandatory { get; set; }
    }
}