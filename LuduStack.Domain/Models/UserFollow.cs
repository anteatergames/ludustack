using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class UserFollow : Entity
    {
        public Guid? FollowUserId { get; set; }
    }
}