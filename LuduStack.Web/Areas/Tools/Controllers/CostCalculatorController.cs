using LuduStack.Web.Areas.Tools.Controllers.Base;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Areas.Tools.Controllers
{
    [AllowAnonymous]
    public class CostCalculatorController : ToolsBaseController
    {
        [Route("tools/costcalculator")]
        public IActionResult Index()
        {
            var model = new CostCalculatorViewModel();

            return View(model);
        }
    }
}