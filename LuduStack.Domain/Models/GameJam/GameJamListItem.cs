using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Domain.Models
{
    public class GameJamListItem
    {
        public Guid Id { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid UserId { get; set; }

        public GameJamType Type { get; set; }

        public GameJamVoters Voters { get; set; }

        public string Handler { get; set; }

        public string FeaturedImage { get; set; }

        public string Name { get; set; }

        public string TimeZone { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EntryDeadline { get; set; }

        public DateTime VotingEndDate { get; set; }

        public DateTime ResultDate { get; set; }

        public string ShortDescription { get; set; }

        public SupportedLanguage Language { get; set; }
    }
}