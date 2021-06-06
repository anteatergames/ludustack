using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class ForumGroup : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public int Order { get; set; }
    }
}