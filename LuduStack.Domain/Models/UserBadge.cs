using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class UserBadge : Entity
    {
        public BadgeType Badge { get; set; }

        public List<Guid> References { get; set; }

        public UserBadge()
        {
            References = new List<Guid>();
        }
    }
}