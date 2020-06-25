using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class UserContentLike : Entity
    {
        public Guid ContentId { get; set; }
    }
}