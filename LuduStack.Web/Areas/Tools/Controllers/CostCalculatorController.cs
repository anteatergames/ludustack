using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.CostCalculator;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Tools.Controllers.Base;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Tools.Controllers
{
    [AllowAnonymous]
    public class CostCalculatorController : ToolsBaseController
    {
        private readonly ICostCalculatorAppService costCalculatorAppService;

        public CostCalculatorController(ICostCalculatorAppService costCalculatorAppService)
        {
            this.costCalculatorAppService = costCalculatorAppService;
        }

        [Route("tools/costcalculator")]
        public IActionResult Index()
        {
            CostCalculatorViewModel model = new CostCalculatorViewModel();

            return View(model);
        }

        [Route("tools/costcalculator/getrates")]
        public async Task<IActionResult> GetRates()
        {
            OperationResultVo<CostsViewModel> rates = await costCalculatorAppService.GetRates(CurrentUserId);

            return Json(rates);
        }
    }
}