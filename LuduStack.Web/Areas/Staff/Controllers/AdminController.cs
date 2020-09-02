using LuduStack.Application.Interfaces;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Areas.Staff.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    public class AdminController : StaffBaseController
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> CheckUserInconsistencies([FromServices] IProfileAppService profileAppService)
        {
            var messages = new List<string>();
            var result = new OperationResultListVo<string>(messages);
            result.Message = "Check Missing UserNames Task";

            try
            {
                var allUsers = await GetUsersAsync();

                var profileResult = profileAppService.GetAll(CurrentUserId);

                if (!profileResult.Success)
                {
                    return View("TaskResult", profileResult);
                }

                foreach (var profile in profileResult.Value)
                {

                    var user = allUsers.FirstOrDefault(x => x.Id.Equals(profile.UserId.ToString()));
                    if (user == null) {
                        messages.Add($"profile {profile.Name} ({profile.Id}) without user {profile.UserId}");
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(profile.UserName))
                        {
                            messages.Add($"profile {profile.Name} ({profile.Id}) without UserName (should be {user.UserName})");
                        }
                    }
                }

                foreach (var user in allUsers)
                {
                    var guid = Guid.Parse(user.Id);
                    var profile = profileResult.Value.FirstOrDefault(x => x.UserId == guid);
                    if (profile == null)
                    {
                        messages.Add($"user {user.UserName} ({user.Id}) without profile");
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

        public async Task<IActionResult> CopyUserNames([FromServices]IProfileAppService profileAppService)
        {
            var messages = new List<string>();
            var result = new OperationResultListVo<string>(messages);
            result.Message = "Copy UserNames Task";

            try
            {
                var allUsers = await GetUsersAsync();

                var profileResult = profileAppService.GetAll(CurrentUserId);

                if (!profileResult.Success)
                {
                    return View("TaskResult", profileResult);
                }

                foreach (var profile in profileResult.Value)
                {
                    var user = allUsers.FirstOrDefault(x => x.Id.Equals(profile.UserId.ToString()));
                    if (user == null)
                    {
                        messages.Add($"ERROR: user for {profile.UserName} ({profile.UserId}) NOT FOUND");
                    }
                    else
                    {
                        profile.UserName = user.UserName;
                        var saveResult = profileAppService.Save(CurrentUserId, profile);
                        messages.Add($"SUCCESS: {profile.UserName} updated");
                    }
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

        public async Task<IQueryable<ApplicationUser>> GetUsersAsync()
        {
            return await Task.Run(() =>
            {
                return UserManager.Users;
            });
        }

    }
}
