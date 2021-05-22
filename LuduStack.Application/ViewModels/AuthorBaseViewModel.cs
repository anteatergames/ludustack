using System;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels
{
    public class AuthorBaseViewModel
    {
        public Guid UserId { get; set; }

        [Display(Name = "Author Picture")]
        public string AuthorPicture { get; set; }

        [Display(Name = "Author Name")]
        public string AuthorName { get; set; }

        public AuthorBaseViewModel()
        {
        }

        public AuthorBaseViewModel(Guid userId)
        {
            UserId = userId;
        }
    }
}