using LuduStack.Application.ViewModels.User;
using LuduStack.Infra.CrossCutting.Identity.Models;

namespace LuduStack.Web.Models
{
    public class AnalyseUserViewModel
    {
        public ApplicationUser User { get; set; }

        public ProfileViewModel Profile { get; set; }
    }
}