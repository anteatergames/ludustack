using LuduStack.Web.Areas.Tools.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Areas.Tools.Controllers
{
    public class GameIdeaController : ToolsBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}