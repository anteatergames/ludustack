using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
