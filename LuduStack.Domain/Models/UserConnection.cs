using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class UserConnection : Entity
    {
        public Guid TargetUserId { get; set; }

        public UserConnectionType ConnectionType { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }
}