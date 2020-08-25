using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class Notification : Entity
    {
        public NotificationType Type { get; set; }

        public Guid? OriginId { get; set; }

        public string OriginName { get; set; }

        public Guid? TargetId { get; set; }

        public string TargetName { get; set; }

        public bool IsRead { get; set; }
    }
}