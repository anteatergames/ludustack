using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects
{
    public class LocalizationStatsVo
    {
        public Guid LocalizationId { get; set; }

        public int TermCount { get; set; }

        public List<LocalizationEntry> Entries { get; set; }

        public double LocalizationPercentage { get; internal set; }
    }
}