using LuduStack.Application.ViewModels.User;
using LuduStack.Infra.CrossCutting.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Models
{
    public class AnalyseUserViewModel
    {
        public ApplicationUser User { get; set; }

        public ProfileViewModel Profile { get; set; }
    }
}
