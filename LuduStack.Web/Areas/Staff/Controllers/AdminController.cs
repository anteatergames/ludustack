using LuduStack.Application.Requests.User;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Areas.Staff.Controllers.Base;
using LuduStack.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    public class AdminController : StaffBaseController
    {
        private readonly IMediator mediator;

        public AdminController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task<ViewResult> Index()
        {
            return Task.FromResult(View());
        }

        public async Task<IActionResult> CheckUserInconsistencies()
        {
            List<string> messages = new List<string>();
            OperationResultListVo<string> result = new OperationResultListVo<string>(messages)
            {
                Message = "Check User Inconsistencies Task"
            };

            try
            {
                OperationResultListVo<ProfileViewModel> profileResult = await ProfileAppService.GetAll(CurrentUserId, true);

                if (!profileResult.Success)
                {
                    result.Message = "Fail to load user profiles.";
                }

                IQueryable<ApplicationUser> allUsers = await GetUsersAsync();

                foreach (ApplicationUser user in allUsers)
                {
                    AnalyseUser(messages, profileResult, user);
                }

                if (profileResult.Success)
                {
                    foreach (ProfileViewModel profile in profileResult.Value)
                    {
                        AnalyseProfile(messages, allUsers, profile);
                    } 
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                messages.Add(ex.Message);
            }

            result.Value = messages.OrderBy(x => x);

            return View("TaskResult", result);
        }

        public async Task<IActionResult> FixUserInconcistencies()
        {
            List<string> messages = new List<string>();
            OperationResultListVo<string> result = new OperationResultListVo<string>(messages)
            {
                Message = "Update Handlers Task"
            };

            try
            {
                OperationResultListVo<ProfileViewModel> profileResult = await ProfileAppService.GetAll(CurrentUserId, true);

                if (!profileResult.Success)
                {
                    result.Message = "Fail to load user profiles.";
                }

                IQueryable<ApplicationUser> allUsers = await GetUsersAsync();

                IQueryable<ApplicationUser> usersWithoutDate = allUsers.Where(x => x.CreateDate == DateTime.MinValue);
                foreach (ApplicationUser user in usersWithoutDate)
                {
                    await FixUserInconsistencies(profileResult, user);
                }

                foreach (ProfileViewModel profile in profileResult.Value)
                {
                    await FixProfileInconsistencies(messages, allUsers, profile);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                messages.Add("ERROR: " + ex.Message);
            }

            result.Value = messages.OrderBy(x => x);

            return View("TaskResult", result);
        }

        [Route("staff/admin/analyseuser/{userId:guid}")]
        public async Task<IActionResult> AnalyseUser(Guid userId)
        {
            AnalyseUserViewModel model = new AnalyseUserViewModel();

            ApplicationUser user = await UserManager.FindByIdAsync(userId.ToString());
            ProfileViewModel profile = await ProfileAppService.GetByUserId(userId, ProfileType.Personal);

            IList<string> roles = await UserManager.GetRolesAsync(user);

            user.Roles = roles.ToList();

            model.User = user;
            model.Profile = profile;

            return View(model);
        }

        [HttpDelete("staff/admin/deleteuser/{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            OperationResultVo result = await mediator.Send(new DeleteUserFromPlatformRequest(CurrentUserId, userId));

            if (result.Success)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(userId.ToString());
                IdentityResult identityResult = await UserManager.DeleteAsync(user);

                Console.WriteLine(identityResult.Succeeded);
            }

            return Json(result);
        }

        public async Task<IQueryable<ApplicationUser>> GetUsersAsync()
        {
            return await Task.Run(() =>
            {
                return UserManager.Users;
            });
        }

        private static void AnalyseUser(List<string> messages, OperationResultListVo<ProfileViewModel> profileResult, ApplicationUser user)
        {
            Guid guid = Guid.Parse(user.Id);
            ProfileViewModel profile = profileResult.Value.FirstOrDefault(x => x.UserId == guid);
            if (profile == null)
            {
                messages.Add($"user {user.UserName} ({user.Id}) without profile");
            }

            if (user.CreateDate == DateTime.MinValue)
            {
                messages.Add($"user {user.UserName} without create date");
            }
        }

        private static void AnalyseProfile(List<string> messages, IQueryable<ApplicationUser> allUsers, ProfileViewModel profile)
        {
            ApplicationUser user = allUsers.FirstOrDefault(x => x.Id.Equals(profile.UserId.ToString()));
            if (user == null)
            {
                messages.Add($"profile {profile.Name} ({profile.Id}) without user {profile.UserId}");
            }
            else
            {
                AnalyseHandler(messages, profile, user);
            }
        }

        private static void AnalyseHandler(List<string> messages, ProfileViewModel profile, ApplicationUser user)
        {
            if (string.IsNullOrWhiteSpace(profile.Handler))
            {
                messages.Add($"profile {profile.Name} ({profile.Id}) without handler (should be {user.UserName.ToLower()})");
            }
            else
            {
                if (!profile.Handler.Equals(profile.Handler.ToLower()))
                {
                    messages.Add($"profile {profile.Name} ({profile.Id}) handler ({profile.Handler}) not lowercase");
                }
            }
        }

        private async Task FixUserInconsistencies(OperationResultListVo<ProfileViewModel> profileResult, ApplicationUser user)
        {
            ProfileViewModel profile = profileResult.Value.FirstOrDefault(x => x.UserId.ToString().Equals(user.Id));
            if (profile != null)
            {
                user.CreateDate = profile.CreateDate;
                await UserManager.UpdateAsync(user);
            }
        }

        private async Task FixProfileInconsistencies(List<string> messages, IQueryable<ApplicationUser> allUsers, ProfileViewModel profile)
        {
            ApplicationUser user = allUsers.FirstOrDefault(x => x.Id.Equals(profile.UserId.ToString()));
            if (user == null)
            {
                messages.Add($"ERROR: user for {profile.Handler} ({profile.UserId}) NOT FOUND");
            }
            else
            {
                string handler = user.UserName.ToLower();
                if (string.IsNullOrWhiteSpace(profile.Handler) || !profile.Handler.Equals(handler))
                {
                    profile.Handler = handler;
                    await ProfileAppService.Save(CurrentUserId, profile);
                    messages.Add($"SUCCESS: {profile.Name} handler updated to \"{handler}\"");
                }
            }
        }
    }
}