using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Domain.ValueObjects
{
    public class GiveawayListItemVo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public GiveawayStatus Status { get; set; }

        public string FeaturedImage { get; set; }

        public int ParticipantCount { get; set; }
        public string StatusLocalized { get; set; }
    }
}