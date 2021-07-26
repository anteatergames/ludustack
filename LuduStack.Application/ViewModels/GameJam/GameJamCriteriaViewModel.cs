using LuduStack.Domain.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamCriteriaViewModel
    {
        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }

        [Display(Name = "Type")]
        public GameJamCriteriaType Type { get; set; }

        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Weight")]
        [Required(ErrorMessage = "Need a weight!")]
        [RegularExpression(@"^[\d]*$", ErrorMessage = "Must be a number!")]
        public decimal Weight { get; set; }
    }
}