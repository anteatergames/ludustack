using LuduStack.Domain.Core.Enums;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamCriteriaResultViewModel
    {
        public GameJamCriteriaType Criteria { get; set; }

        public decimal Score { get; set; }

        public int FinalPosition { get; set; }
    }
}