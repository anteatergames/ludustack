using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Gamification.Controllers
{
    [Area("gamification")]
    [Route("gamification/userbadge")]
    public class UserBadgeController : SecureBaseController
    {
        private readonly IGamificationAppService gamificationAppService;

        public UserBadgeController(IGamificationAppService gamificationAppService)
        {
            this.gamificationAppService = gamificationAppService;
        }

        [Route("help")]
        public IActionResult Help()
        {
            Dictionary<string, UiInfoAttribute> badges = SetBadgeList();

            ViewData["Badges"] = badges;

            return View();
        }

        [Route("list/{id}")]
        public async Task<IActionResult> ListByUser(Guid id)
        {
            OperationResultListVo<UserBadgeViewModel> badges = await gamificationAppService.GetBadgesByUserId(id);

            return View("_List", badges.Value);
        }

        private static Dictionary<string, UiInfoAttribute> SetBadgeList()
        {
            List<KeyValuePair<string, UiInfoAttribute>> list = new List<KeyValuePair<string, UiInfoAttribute>>();

            IEnumerable<BadgeType> badges = Enum.GetValues(typeof(BadgeType)).Cast<BadgeType>();

            badges.ToList().ForEach(x =>
            {
                UiInfoAttribute uiInfo = x.GetAttributeOfType<UiInfoAttribute>();

                list.Add(new KeyValuePair<string, UiInfoAttribute>(x.ToString(), uiInfo));
            });

            Dictionary<string, UiInfoAttribute> dict = list.OrderBy(x => x.Value.Order).ToDictionary(x => x.Key, x => x.Value);

            return dict;
        }
    }
}