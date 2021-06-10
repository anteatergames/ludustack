using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Forum
{
    public class ForumIndexViewModel
    {
        public List<ForumGroupViewModel> Groups { get; set; }

        public ForumIndexViewModel()
        {
            Groups = new List<ForumGroupViewModel>();
        }
    }
}