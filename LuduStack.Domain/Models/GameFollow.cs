using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class GameFollow : Entity
    {
        public Guid GameId { get; set; }
    }
}