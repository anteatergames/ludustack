using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class Notification : Entity
    {
        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }
    }
}