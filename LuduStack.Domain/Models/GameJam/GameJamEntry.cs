using LuduStack.Domain.Core.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class GameJamEntry : Entity
    {
        public bool LateSubmission { get; set; }

        public bool IsTeam { get; set; }

        public Guid GameJamId { get; set; }

        public DateTime JoinDate { get; set; }

        public DateTime SubmissionDate { get; set; }

        public Guid? GameId { get; set; }

        public Guid? TeamId { get; set; }

        public int FinalPlace { get; set; }

        public decimal TotalScore { get; set; }

        public string ExtraInformation { get; set; }

        public List<GameJamTeamMember> TeamMembers { get; set; }

        public List<GameJamVote> Votes { get; set; }

        public List<GameJamCriteriaResult> CriteriaResults { get; set; }
    }
}