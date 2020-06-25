using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.ValueObjects
{
    public class UserProfileEssentialVo : Entity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public bool HasCoverImage { get; set; }
    }
}