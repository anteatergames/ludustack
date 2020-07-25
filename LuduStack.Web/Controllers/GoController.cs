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
                OperationResultVo<ShortUrl> castRestult = result as OperationResultVo<ShortUrl>;

                var url = castRestult.Value?.OriginalUrl;

                url = PrepareUrl(url, referral);

                switch (castRestult.Value.DestinationType)
                {
                    case Domain.Core.Enums.ShortUrlDestinationType.Giveaway:
                        ViewData["Title"] = SharedLocalizer["Giveaways"];
                        ViewData["Description"] = SharedLocalizer["Win Prizes!"];
                        //ViewData["OgImage"] = ;
                        break;
                    case Domain.Core.Enums.ShortUrlDestinationType.Undefined:
                    default:
                        break;
                }

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