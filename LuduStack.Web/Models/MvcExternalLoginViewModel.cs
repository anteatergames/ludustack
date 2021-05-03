using LuduStack.Infra.CrossCutting.Identity.Models.AccountViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Web.Models
{
    public class MvcExternalLoginViewModel : ExternalLoginViewModel
    {
        [Required(ErrorMessage = "The username is required")]
        [Remote("validateusername", "account", AdditionalFields = "Email", HttpMethod = "POST", ErrorMessage = "Oops! Someone already took that username!")]
        [Display(Name = "UserName")]
        [StringLength(64, ErrorMessage = "The username must have maximum 64 characters")]
        [RegularExpression("^(?=.{3,32}$)(?![-.])(?!.*[-.]{2})[a-zA-Z0-9-.]+(?<![-.])$", ErrorMessage = "Must have minimum 3 characters.</br>Must contain only letters, numbers, dashes and dots.</br>Must not contain two symbols in sequence.</br>Must not start or end with a symbol.</br>Must not contain spaces.")]
        public new string Username { get; set; }

        [Display(Name = "Profile Name")]
        public string ProfileName { get; set; }
    }
}