using System;

namespace LuduStack.Domain.ValueObjects
{
    public class GiveawayListItemVo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string CreateDate { get; set; }

        public string Status { get; set; }

        public string FeaturedImage { get; set; }

        public int ParticipantCount { get; set; }
    }
}