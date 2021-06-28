using LuduStack.Domain.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamViewModel : BaseViewModel
    {
        public GameJamType Type { get; set; }

        public GameJamVoters Voters { get; set; }

        public bool IsDraft { get; set; }

        public bool Unlisted { get; set; }

        public bool CommunityCanVote { get; set; }

        public bool HideSubmissions { get; set; }

        public bool HideResults { get; set; }

        [Required]
        [Remote("validatehandler", "gamejam", "community", AdditionalFields = "Email", HttpMethod = "POST", ErrorMessage = "Oops! That handler is already in use!")]
        [RegularExpression("^(?=.{3,32}$)(?![-.])(?!.*[-.]{2})[a-zA-Z0-9-.]+(?<![-.])$", ErrorMessage = "Invalid handler!")]
        public string Handler { get; set; }

        [RegularExpression("^(?=.{3,32}$)(?![-.])(?!.*[-.]{2})[a-zA-Z0-9-.]+(?<![-.])$", ErrorMessage = "Invalid handler!")]
        public string HashTag { get; set; }

        public string BannerImage { get; set; }

        public string FeaturedImage { get; set; }

        public string BackgroundImage { get; set; }

        [Required]
        public string Name { get; set; }

        public string ShortDescription { get; set; }

        [Required]
        public string Description { get; set; }

        public string MainTheme { get; set; }

        public string PrizeDescription { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EntryDeadline { get; set; }

        public DateTime VotingEndDate { get; set; }

        public DateTime ResultDate { get; set; }

        public List<Guid> Judges { get; set; }

        public List<GameJamSponsorViewModel> Sponsors { get; set; }

        public List<GameJamCriteriaViewModel> Criteria { get; set; }

        public string AuthorName { get; set; }

        public string AuthorHandler { get; set; }

        public int SecondsToCountDown { get; set; }

        public int JoinCount { get; set; }
    }
}
