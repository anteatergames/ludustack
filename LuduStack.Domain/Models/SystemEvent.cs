using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class SystemEvent : Entity
    {
        public SystemEventType Type { get; set; }
    }
}