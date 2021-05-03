using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Controllers.Base;
using LuduStack.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    public class ProfileController : SecureBaseController
    {
        private readonly IProfileAppService profileAppService;
        private readonly IGamificationAppService gamificationAppService;

        public ProfileController(IProfileAppService profileAppService, IGamificationAppService gamificationAppService) : base()
        {
            this.profileAppService = profileAppService;
            this.gamificationAppService = gamificationAppService;
        }

        [HttpGet]
        [Route("/u/{userHandler?}")]
        [Route("profile/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, string userHandler, bool refreshImages)
        {
            try
            {
                ProfileViewModel vm = await profileAppService.Get(CurrentUserId, id, userHandler, ProfileType.Personal);

                if (vm == null)
                {
                    ProfileViewModel profile = profileAppService.GenerateNewOne(ProfileType.Personal);

                    ApplicationUser user = await UserManager.FindByIdAsync(id.ToString());

                    if (user != null)
                    {
                        profile.UserId = id;
                        await profileAppService.Save(CurrentUserId, profile);
                    }
                    else
                    {
                        return RedirectToAction("index", "home", new { area = string.Empty, msg = SharedLocalizer["User not found!"] });
                    }

                    vm = profile;
                }

                await gamificationAppService.FillProfileGamificationDetails(CurrentUserId, vm);

                if (CurrentUserId != Guid.Empty)
                {
                    ApplicationUser user = await UserManager.FindByIdAsync(CurrentUserId.ToString());
                    bool userIsAdmin = await UserManager.IsInRoleAsync(user, Roles.Administrator.ToString());
                    vm.Permissions.IsAdmin = userIsAdmin;
                    vm.Permissions.CanEdit = vm.UserId == CurrentUserId;
                    vm.Permissions.CanFollow = vm.UserId != CurrentUserId;
                    vm.Permissions.CanConnect = vm.UserId != CurrentUserId;
                }

                ViewData["ConnecionTypes"] = EnumExtensions.ToJson(UserConnectionType.Mentor);

                SetImagesToRefresh(vm, refreshImages);

                return View(vm);
            }
            catch
            {
                return RedirectToAction("index", "home", new { area = string.Empty, msg = SharedLocalizer["Unable to get the user!"] });
            }
        }

        [Route("profile/edit/{userId:guid}")]
        public async Task<IActionResult> Edit(Guid userId)
        {
            ProfileViewModel vm = await profileAppService.GetByUserId(userId, ProfileType.Personal, true);

            OperationResultVo countriesResult = profileAppService.GetCountries(CurrentUserId);
            if (countriesResult.Success)
            {
                OperationResultListVo<SelectListItemVo> castResultCountries = countriesResult as OperationResultListVo<SelectListItemVo>;

                IEnumerable<SelectListItemVo> countries = castResultCountries.Value;

                List<SelectListItem> countriesDropDown = countries.ToSelectList();
                if (!string.IsNullOrWhiteSpace(vm.Country))
                {
                    countriesDropDown.ForEach(x => x.Selected = x.Value.Equals(vm.Country));
                }
                ViewBag.Countries = countriesDropDown;
            }
            else
            {
                ViewBag.Countries = new List<SelectListItem>();
            }

            SetImagesToRefresh(vm, true);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProfileViewModel vm, IFormFile avatar)
        {
            try
            {
                OperationResultVo<Guid> saveResult = await profileAppService.Save(CurrentUserId, vm);
                if (!saveResult.Success)
                {
                    return Json(new OperationResultVo(false, saveResult.Message));
                }
                else
                {
                    string url = Url.Action("Details", "Profile", new { area = string.Empty, userHandler = vm.Handler, refreshImages = true });

                    return Json(new OperationResultRedirectVo(url));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        private static void SetImagesToRefresh(ProfileViewModel vm, bool refresh)
        {
            if (refresh)
            {
                vm.ProfileImageUrl = UrlFormatter.ReplaceCloudVersion(vm.ProfileImageUrl);
                vm.CoverImageUrl = UrlFormatter.ReplaceCloudVersion(vm.CoverImageUrl);
            }
        }
    }
}