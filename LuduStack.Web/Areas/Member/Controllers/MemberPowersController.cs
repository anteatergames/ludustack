using LuduStack.Web.Areas.Member.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    public class MemberPowersController : MemberBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}