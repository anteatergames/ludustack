using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamEntryViewModel : UserGeneratedBaseViewModel, IUserProfileBasic
    {
        public bool LateSubmission { get; set; }

        public bool IsTeam { get; set; }

        public Guid GameJamId { get; set; }

        public DateTime JoinDate { get; set; }

        public DateTime SubmissionDate { get; set; }

        [Required(ErrorMessage = "You must select a Game to submit!")]
        public Guid GameId { get; set; }

        public Guid? TeamId { get; set; }

        public int FinalPlace { get; set; }

        public decimal TotalScore { get; set; }

        [Display(Name = "Extra Information", Description = "Fill this if you want to give any extra information about your game to the judges or to the community.")]
        public string ExtraInformation { get; set; }

        [Display(Name = "Team Members", Description = "This is the team.")]
        public List<GameJamTeamMemberViewModel> TeamMembers { get; set; }

        public List<GameJamVoteViewModel> Votes { get; set; }

        public List<GameJamCriteriaResultViewModel> CriteriaResults { get; set; }

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

        public bool CanShowResults { get; set; }

        public bool IsOverallVote { get; set; }

        public bool ShowAllCriteria { get; set; }

        public bool CanShowFinalResults { get; set; }

        public bool IsWinner { get; set; }

        public List<ProfileViewModel> TeamMembersProfiles { get; set; }

        public DateTime SubmissionDateForDisplay { get; set; }
    }
}