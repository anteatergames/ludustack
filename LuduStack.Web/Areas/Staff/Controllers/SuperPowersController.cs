using LuduStack.Web.Areas.Staff.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    public class SuperPowersController : StaffBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}