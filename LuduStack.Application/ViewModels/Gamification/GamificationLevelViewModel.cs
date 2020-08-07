using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Gamification
{
    public class GamificationLevelViewModel : BaseViewModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Number { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Remote("validateXp", "gamificationlevel", "Staff", AdditionalFields = "Id", HttpMethod = "POST", ErrorMessage = "It must be greater than the biggest XP already defined.")]
        public int XpToAchieve { get; set; }
    }
}