using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces.Models;

namespace LuduStack.Domain.ValueObjects
{
    public class UserProfileEssentialVo : Entity
    {
        public string Handler { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool HasCoverImage { get; set; }
    }
}