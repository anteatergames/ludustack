using LuduStack.Domain.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.GameIdea
{
    public class GameIdeaViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Language")]
        public SupportedLanguage Language { get; set; }

        [Required]
        [Display(Name = "Type")]
        public GameIdeaElementType Type { get; set; }

        [Required]
        [Remote("validatedescription", "gameidea", "staff", AdditionalFields = "Id, Language, Type", HttpMethod = "POST", ErrorMessage = "Oops! That idea already exists!")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}