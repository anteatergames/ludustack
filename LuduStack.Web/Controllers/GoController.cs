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
        public IActionResult Go(string token, string source)
        {
            OperationResultVo result = shortUrlAppService.GetFullUrlByToken(token);

            if (result.Success)
            {
                OperationResultVo<string> castRestult = result as OperationResultVo<string>;

                var url = castRestult.Value;

                url = PrepareUrl(url, source);

                return Redirect(url);
            }
            else
            {
                return RedirectToAction("index", "home", new { area = string.Empty });
            }
        }

        private string PrepareUrl(string url, string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return url;
            }

            if (url.Contains("?"))
            {
                url += "&source=";
            }
            else
            {
                url += "?source=";
            }

            url += source;

            return url;
        }
    }
}