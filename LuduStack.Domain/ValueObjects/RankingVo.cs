using LuduStack.Domain.Models;

namespace LuduStack.Domain.ValueObjects
{
    public class RankingVo
    {
        public Gamification Gamification { get; set; }

        public GamificationLevel Level { get; set; }
    }
}