using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class Gamification : Entity
    {
        public int CurrentLevelNumber { get; set; }

        public int XpTotal { get; set; }

        public int XpCurrentLevel { get; set; }

        public int XpToNextLevel { get; set; }

        public bool ExcludeFromRanking { get; set; }
    }
}