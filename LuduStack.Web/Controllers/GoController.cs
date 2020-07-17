using LuduStack.Application.Interfaces;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Controllers
{
    public class GoController : BaseController
    {
        private readonly IShortUrlAppService shortUrlAppService;

        public GoController(IShortUrlAppService shortUrlAppService)
        {
            this.shortUrlAppService = shortUrlAppService;
        }

        [Route("go/{token}")]
        public IActionResult Go(string token)
        {
            OperationResultVo result = shortUrlAppService.GetFullUrlByToken(token);

            if (result.Success)
            {
                OperationResultVo<string> castRestult = result as OperationResultVo<string>;

                return Redirect(castRestult.Value);
            }
            else
            {
                return RedirectToAction("index", "home", new { area = string.Empty });
            }
        }
    }
}