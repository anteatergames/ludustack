using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class GameLike : Entity
    {
        public Guid GameId { get; set; }
    }
}