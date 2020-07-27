using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Domain.Models
{
    public class GiveawayEntry
    {
        public DateTime Date { get; set; }

        public GiveawayEntryType Type { get; set; }

        public int Points { get; set; }
    }
}