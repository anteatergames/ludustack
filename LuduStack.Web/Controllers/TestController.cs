using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Controllers
{
    public class TestController : SecureBaseController
    {
        public IActionResult DiscordWidget()
        {
            return View();
        }
    }
}