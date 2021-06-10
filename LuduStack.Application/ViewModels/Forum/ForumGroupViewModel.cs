using LuduStack.Domain.ValueObjects;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Forum
{
    public class ForumGroupViewModel : BaseViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string Icon { get; set; }

        [Range(1, 999)]
        public int Order { get; set; }

        public string Slug { get; set; }

        public List<ForumCategoryListItemVo> Categories { get; set; }

        public ForumGroupViewModel()
        {
            Categories = new List<ForumCategoryListItemVo>();
        }
    }
}