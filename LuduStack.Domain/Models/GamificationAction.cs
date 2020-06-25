using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class GamificationAction : Entity
    {
        public PlatformAction Action { get; set; }

        public int ScoreValue { get; set; }
    }
}