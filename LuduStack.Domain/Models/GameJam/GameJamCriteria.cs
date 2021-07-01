using System;

namespace LuduStack.Domain.Models
{
    public class GameJamCriteria
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Weight { get; set; }
    }
}