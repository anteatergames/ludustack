using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Gamification.Controllers
{
    [Area("gamification")]
    [Route("gamification/ranking")]
    public class RankingController : SecureBaseController
    {
        private readonly IGamificationAppService gamificationAppService;
        private readonly IProfileAppService profileAppService;

        public RankingController(IGamificationAppService gamificationAppService, IProfileAppService profileAppService)
        {
            this.gamificationAppService = gamificationAppService;
            this.profileAppService = profileAppService;
        }

        [Route("help")]
        public async Task<IActionResult> Help()
        {
            OperationResultListVo<GamificationLevelViewModel> objs = await gamificationAppService.GetAllLevels();

            return View(objs.Value);
        }

        public async Task<IActionResult> Index()
        {
            OperationResultListVo<RankingViewModel> serviceResult = await gamificationAppService.GetAll();

            List<RankingViewModel> objs = serviceResult.Value.ToList();

            foreach (RankingViewModel obj in objs)
            {
                Application.ViewModels.User.ProfileViewModel profile = await profileAppService.GetUserProfileWithCache(obj.UserId);

                if (profile != null)
                {
                    obj.Name = profile.Name;
                    obj.ProfileImageUrl = UrlFormatter.ProfileImage(obj.UserId);
                    obj.CoverImageUrl = UrlFormatter.ProfileCoverImage(obj.UserId, profile.Id, profile.LastUpdateDate, profile.HasCoverImage);
                }
            }

            return View(objs);
        }
    }
}