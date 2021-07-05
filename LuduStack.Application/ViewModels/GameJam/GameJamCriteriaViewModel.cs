using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamCriteriaViewModel
    {
        public GameJamCriteriaType Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Weight { get; set; }
    }
}