using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class ForumCategory : Entity
    {
        public string Handler { get; set; }

        public string FeaturedImage { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }
    }
}