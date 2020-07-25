using LuduStack.Application.Interfaces;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LuduStack.Web.Controllers
{
    public class GoController : BaseController
    {
        private readonly IShortUrlAppService shortUrlAppService;

        public GoController(IShortUrlAppService shortUrlAppService)
        {
            this.shortUrlAppService = shortUrlAppService;
        }

        [Route("go/{token}/{referral?}")]
        public IActionResult Go(string token, string referral)
        {
            OperationResultVo result = shortUrlAppService.GetFullUrlByToken(token);

            if (result.Success)
            {
                OperationResultVo<string> castRestult = result as OperationResultVo<string>;

                var url = castRestult.Value;

                url = PrepareUrl(url, referral);

                return Redirect(url);
            }
            else
            {
                return RedirectToAction("index", "home", new { area = string.Empty });
            }
        }

        private string PrepareUrl(string url, string referral)
        {
            if (string.IsNullOrWhiteSpace(referral))
            {
                return url;
            }

            if (url.Contains("?"))
            {
                url += "&r=";
            }
            else
            {
                url += "?r=";
            }

            url += referral;

            return url;
        }
    }
}