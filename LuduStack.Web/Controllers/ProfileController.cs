using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Controllers.Base;
using LuduStack.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    [Authorize]
    public class ProfileController : SecureBaseController
    {
        private readonly IProfileAppService profileAppService;
        private readonly IGamificationAppService gamificationAppService;

        public ProfileController(IProfileAppService profileAppService, IGamificationAppService gamificationAppService)
        {
            this.profileAppService = profileAppService;
            this.gamificationAppService = gamificationAppService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/u/{userHandler?}")]
        [Route("profile/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, string userHandler, bool refreshImages)
        {
            try
            {
                ProfileViewModel viewModel = await profileAppService.Get(CurrentUserId, id, userHandler, ProfileType.Personal);

                if (viewModel == null)
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

                    viewModel = profile;
                }

                await gamificationAppService.FillProfileGamificationDetails(CurrentUserId, viewModel);

                SetPermissions(viewModel);

                ViewData["ConnecionTypes"] = EnumExtensions.ToJson(UserConnectionType.Mentor);

                SetImagesToRefresh(viewModel, refreshImages);

                return View(viewModel);
            }
            catch
            {
                return RedirectToAction("index", "home", new { area = string.Empty, msg = SharedLocalizer["Unable to get the user!"] });
            }
        }

        [Route("profile/edit/{userId:guid}")]
        public async Task<IActionResult> Edit(Guid userId)
        {
            if (!CurrentUserIsAdmin && userId != CurrentUserId)
            {
                return RedirectToAction("index", "home", new { msg = SharedLocalizer["You cannot edit someone else's profile!"] });
            }

            ProfileViewModel viewModel = await profileAppService.GetByUserId(userId, ProfileType.Personal, true);

            OperationResultVo countriesResult = profileAppService.GetCountries(CurrentUserId);
            if (countriesResult.Success)
            {
                OperationResultListVo<SelectListItemVo> castResultCountries = countriesResult as OperationResultListVo<SelectListItemVo>;

                IEnumerable<SelectListItemVo> countries = castResultCountries.Value;

                List<SelectListItem> countriesDropDown = countries.ToSelectList();
                if (!string.IsNullOrWhiteSpace(viewModel.Country))
                {
                    countriesDropDown.ForEach(x => x.Selected = x.Value.Equals(viewModel.Country));
                }
                ViewData["Countries"] = countriesDropDown;
            }
            else
            {
                ViewData["Countries"] = new List<SelectListItem>();
            }

            SetImagesToRefresh(viewModel, true);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProfileViewModel viewModel, IFormFile avatar)
        {
            if (!CurrentUserIsAdmin && viewModel.UserId != CurrentUserId)
            {
                return RedirectToAction("index", "home", new { msg = SharedLocalizer["You cannot edit someone else's profile!"] });
            }

            try
            {
                OperationResultVo<Guid> saveResult = await profileAppService.Save(CurrentUserId, viewModel);
                if (!saveResult.Success)
                {
                    return Json(new OperationResultVo(false, saveResult.Message));
                }
                else
                {
                    string url = Url.Action("details", "profile", new { area = string.Empty, userHandler = viewModel.Handler, refreshImages = true });

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

        private void SetPermissions(ProfileViewModel viewModel)
        {
            if (CurrentUserId != Guid.Empty)
            {
                viewModel.Permissions.IsAdmin = CurrentUserIsAdmin;
                viewModel.Permissions.CanEdit = viewModel.UserId == CurrentUserId;
                viewModel.Permissions.CanFollow = viewModel.UserId != CurrentUserId;
                viewModel.Permissions.CanConnect = viewModel.UserId != CurrentUserId;
            }
        }
    }
}