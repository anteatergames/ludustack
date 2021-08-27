using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamViewModel : UserGeneratedBaseViewModel
    {
        public string ForumCategoryHandler { get; set; }

        public string DiscordUrl { get; set; }

        public GameJamType Type { get; set; }

        [Display(Name = "Participation", Description = "How people can participate on this Game Jam?")]
        public GameJamParticipationType ParticipationType { get; set; }

        [Display(Name = "Voters", Description = "Who can cast votes on entries for this Game Jam?")]
        public GameJamVoters Voters { get; set; }

        [Display(Name = "Language", Description = "What is the main language of the Game Jam?")]
        public SupportedLanguage Language { get; set; }

        [Display(Name = "Is Draft")]
        public bool IsDraft { get; set; }

        [Display(Name = "Unlisted", Description = "This prevents this Game Jam to be listed on the platform. Only people with a direct link can see it.")]
        public bool Unlisted { get; set; }

        [Display(Name = "Community can vote", Description = "Any user from the platform can vote on entries for this Game Jam.")]
        public bool CommunityCanVote { get; set; }

        [Display(Name = "Hide Submissions", Description = "All submissions to this Game Jam would not be visible during the submission phase.")]
        public bool HideSubmissions { get; set; }

        [Display(Name = "Hide Realtime Results", Description = "The results will only be visible after the voting phase.")]
        public bool HideRealtimeResults { get; set; }

        [Display(Name = "Allow late join", Description = "Allows users to join this Game Jam after it starts.")]
        public bool AllowLateJoin { get; set; }

        [Display(Name = "Hide Main Theme", Description = "The Main Theme will be hidden during the warmup phase.")]
        public bool HideMainTheme { get; set; }

        [Display(Name = "Full Width Banner", Description = "Renders the top banner with full width.")]
        public bool FullWidthBanner { get; set; }

        [Required(ErrorMessage = "You must choose a unique handler!")]
        [Display(Name = "Handler", Description = "Define a handler for your Game Jam. The handler will be used in the URL like ludustack.com/jam/mycoolhandler. You can only use letters, numbers, dashes and dots. No spaces allowed and all will be saved as lowercase.")]
        [Remote("validatehandler", "gamejam", "community", AdditionalFields = "Id", HttpMethod = "POST", ErrorMessage = "Oops! That handler is already in use!")]
        [RegularExpression("^(?=.{3,32}$)(?![-.])(?!.*[-.]{2})[a-zA-Z0-9-.]+(?<![-.])$", ErrorMessage = "Invalid handler!")]
        public string Handler { get; set; }

        [RegularExpression("^(?=.{3,32}$)(?![-.])(?!.*[-.]{2})[a-zA-Z0-9-.]+(?<![-.])$", ErrorMessage = "Invalid hashtag!")]
        public string HashTag { get; set; }

        [Display(Name = "Winners", Description = "If this Game Jam has winners, define the amount of winners here.")]
        public int Winners { get; set; }

        [Display(Name = "Banner Image", Description = "This image will be shown at the top of your Game Jam page. We recomend you to pick a transparent PNG image.")]
        public string BannerImage { get; set; }

        [Display(Name = "Featured Image", Description = "This image will be used when someone shares the Game Jam to social networks.")]
        public string FeaturedImage { get; set; }

        [Display(Name = "Background Image", Description = "This will be used as background of your Game Jam page.")]
        public string BackgroundImage { get; set; }

        [Display(Name = "Background Color", Description = "Select a background color for the Game Jam page.")]
        public string BackgroundColor { get; set; }

        [Required(ErrorMessage = "A Game Jam MUST have a name!")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Give your Game Jam a short description.")]
        [Display(Name = "Short Description", Description = "Give your Game Jam a killer short description. How would you pitch your Game Jam to an investor?")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Please, provide a nice description of your Game Jam.")]
        [Display(Name = "Description", Description = "This is your place to shine. Give you Game Jam a nice description to set all the details and rules so users feel confortable in joining and your Game Jam will be a success!")]
        public string Description { get; set; }

        [Display(Name = "Main Theme")]
        public string MainTheme { get; set; }

        [Display(Name = "Time Zone")]
        public string TimeZone { get; set; }

        public int TimeZoneDifference
        {
            get
            {
                int timeZoneDifference = 0;

                if (!string.IsNullOrWhiteSpace(TimeZone))
                {
                    int.TryParse(TimeZone, out timeZoneDifference);
                }

                return timeZoneDifference;
            }
        }

        [Required(ErrorMessage = "This is needed")]
        [Display(Name = "Start Date", Description = "Here you set when this Game Jam starts.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "This is needed")]
        [Display(Name = "Deadline", Description = "Here you set the limit date for the participants to submit their entries to this Game Jam. After this, the if the participant still wants to submit and entry, it should be requested a Late Submission permit.")]
        public DateTime EntryDeadline { get; set; }

        [Required(ErrorMessage = "This is needed")]
        [Display(Name = "Voting End", Description = "This is the limit date for all voters to cast their votes. After this date, no one can vote anymore unless the Voting Date is pushing forward.")]
        public DateTime VotingEndDate { get; set; }

        [Required(ErrorMessage = "This is needed")]
        [Display(Name = "Results Date", Description = "The date where the results should be made public.")]
        public DateTime ResultDate { get; set; }

        [Display(Name = "Judges", Description = "These are the selected Judges.")]
        public List<GameJamJudgeViewModel> Judges { get; set; }

        public List<GameJamSponsorViewModel> Sponsors { get; set; }

        [Display(Name = "Criteria", Description = "Select the criteria to be used on the voting phase. If no criteria is selected a Overall criteria will be used.")]
        public List<GameJamCriteriaViewModel> Criteria { get; set; }

        public List<ProfileViewModel> JudgesProfiles { get; set; }

        // extra properties

        public bool RemoveFeaturedImage { get; set; }

        public bool RemoveBannerImage { get; set; }

        public bool RemoveBackgroundImage { get; set; }

        public string AuthorHandler { get; set; }

        public int SecondsToCountDown { get; set; }

        public int JoinCount { get; set; }

        public GameJamPhase CurrentPhase { get; set; }

        public int PhaseNumber { get; set; }

        public string CountDownMessage { get; set; }

        public bool CurrentUserJoined { get; set; }

        public bool ShowMainTheme { get; set; }

        public string CantJoinMessage { get; set; }

        public List<GameJamHighlightsVo> Highlights { get; set; }

        public bool ShowSubmissions { get; set; }

        public bool ShowJudges { get; set; }

        public bool ShowCriteria { get; set; }

        public bool HasWinners => Winners > 0;

        public bool ShowFinalResults { get; set; }

        public DateTime CreateDateToDisplay { get; set; }

        public DateTime StartDateToDisplay { get; set; }

        public DateTime EntryDeadlineToDisplay { get; set; }

        public DateTime VotingEndDateToDisplay { get; set; }

        public DateTime ResultDateToDisplay { get; set; }
        public string DurationText { get; set; }
    }
}