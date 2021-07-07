using LuduStack.Application.ViewModels.Game;
using LuduStack.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamEntryViewModel : UserGeneratedBaseViewModel, IUserProfileBasic
    {
        public bool LateSubmission { get; set; }

        public bool IsTeam { get; set; }

        public Guid GameJamId { get; set; }

        public DateTime JoinDate { get; set; }

        public DateTime SubmissionDate { get; set; }

        public Guid GameId { get; set; }

        public Guid? TeamId { get; set; }

        public int FinalPlace { get; set; }

        public List<GameJamVoteViewModel> Votes { get; set; }

        public int SecondsToCountDown { get; set; }

        public string Title { get; set; }

        public string FeaturedImage { get; set; }

        public GameJamViewModel GameJam { get; set; }

        public GameViewModel Game { get; set; }

        public bool Submitted => SubmissionDate != default;

        public string JamHandler { get; set; }

        #region IUserProfileBasic

        public string Handler { get; set; }

        public string ProfileImageUrl { get; set; }

        public string CoverImageUrl { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string Location { get; set; }

        #endregion IUserProfileBasic

        public decimal TotalScore { get; set; }

        public bool CanShowResults { get; set; }

        public bool IsOverallVote { get; set; }

        public bool ShowAllCriteria { get; set; }
    }
}