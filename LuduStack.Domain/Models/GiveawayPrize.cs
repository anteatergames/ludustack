using LuduStack.Domain.Core.Models;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class GiveawayPrize : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        public List<string> Secrets { get; set; }
    }
}