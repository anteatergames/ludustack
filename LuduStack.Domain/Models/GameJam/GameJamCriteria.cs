using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.Models
{
    public class GameJamCriteria
    {
        public bool Enabled { get; set; }

        public GameJamCriteriaType Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Weight { get; set; }
    }
}