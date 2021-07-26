using System;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Forum
{
    public class ForumCategoryViewModel : BaseViewModel
    {
        [Required]
        [RegularExpression("^(?=.{3,32}$)(?![-.])(?!.*[-.]{2})[a-zA-Z0-9-.]+(?<![-.])$", ErrorMessage = "Invalid handler!")]
        public string Handler { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public Guid GroupId { get; set; }

        public string FeaturedImage { get; set; }

        public string Icon { get; set; }

        public int TopicCount { get; set; }

        public int PostCount { get; set; }

        public bool IsForGameJam { get; set; }
    }
}