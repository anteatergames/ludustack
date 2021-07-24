using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.Models
{
    public class GameJamCriteriaResult
    {
        public GameJamCriteriaType Criteria { get; set; }

        public decimal Score { get; set; }

        public int FinalPosition { get; set; }
    }
}