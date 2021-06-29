﻿using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class GameJam : Entity
    {
        public GameJamType Type { get; set; }

        public GameJamVoters Voters { get; set; }

        public bool IsDraft { get; set; }

        public bool Unlisted { get; set; }

        public bool CommunityCanVote { get; set; }

        public bool HideSubmissions { get; set; }

        public bool HideResults { get; set; }

        public bool AllowLateJoin { get; set; }

        public string Handler { get; set; }

        public string HashTag { get; set; }

        public string BannerImage { get; set; }

        public string FeaturedImage { get; set; }

        public string BackgroundImage { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public string MainTheme { get; set; }

        public string PrizeDescription { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EntryDeadline { get; set; }

        public DateTime VotingEndDate { get; set; }

        public DateTime ResultDate { get; set; }

        public List<Guid> Judges { get; set; }

        public List<GameJamSponsor> Sponsors { get; set; }

        public List<GameJamCriteria> Criteria { get; set; }
    }
}