using LuduStack.Web.Areas.Tools.Controllers.Base;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Areas.Tools.Controllers
{
    [AllowAnonymous]
    public class RateCalculatorController : ToolsBaseController
    {
        [Route("tools/ratecalculator")]
        public IActionResult Index()
        {
            RateCalculatorViewModel model = new RateCalculatorViewModel();

            return View(model);
        }
    }
}