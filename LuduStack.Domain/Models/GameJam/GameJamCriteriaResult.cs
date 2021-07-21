using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Domain.Models
{
    public class GameJamCriteriaResult
    {
        public GameJamCriteriaType Criteria { get; set; }

        public decimal Score { get; set; }

        public int FinalPosition { get; set; }
    }
}
