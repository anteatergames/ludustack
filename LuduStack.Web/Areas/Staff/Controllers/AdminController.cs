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
            result.Message = "Check User Inconsistencies Task";

            try
            {
                var allUsers = await GetUsersAsync();

                var profileResult = profileAppService.GetAll(CurrentUserId, true);

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
                }

                foreach (var user in allUsers)
                {
                    var guid = Guid.Parse(user.Id);
                    var profile = profileResult.Value.FirstOrDefault(x => x.UserId == guid);
                    if (profile == null)
                    {
                        messages.Add($"user {user.UserName} ({user.Id}) without profile");
                    }

                    if (user.CreateDate == DateTime.MinValue)
                    {
                        messages.Add($"user {user.UserName} without create date");
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

        public async Task<IActionResult> FixUserInconcistencies([FromServices]IProfileAppService profileAppService)
        {
            var messages = new List<string>();
            var result = new OperationResultListVo<string>(messages);
            result.Message = "Update Handlers Task";

            try
            {
                var allUsers = await GetUsersAsync();

                var profileResult = profileAppService.GetAll(CurrentUserId, true);

                if (!profileResult.Success)
                {
                    return View("TaskResult", profileResult);
                }

                var usersWithoutDate = allUsers.Where(x => x.CreateDate == DateTime.MinValue);
                foreach (var user in usersWithoutDate)
                {
                    var profile = profileResult.Value.FirstOrDefault(x => x.UserId.ToString().Equals(user.Id));
                    if (profile != null)
                    {
                        user.CreateDate = profile.CreateDate;
                        await UserManager.UpdateAsync(user);
                    }
                }

                foreach (var profile in profileResult.Value)
                {
                    var user = allUsers.FirstOrDefault(x => x.Id.Equals(profile.UserId.ToString()));
                    if (user == null)
                    {
                        messages.Add($"ERROR: user for {profile.Handler} ({profile.UserId}) NOT FOUND");
                    }
                    else
                    {
                        var handler = user.UserName.ToLower();
                        if (string.IsNullOrWhiteSpace(profile.Handler) || !profile.Handler.Equals(handler))
                        {
                            profile.Handler = handler;
                            var saveResult = profileAppService.Save(CurrentUserId, profile);
                            messages.Add($"SUCCESS: {profile.Name} handler updated to \"{handler}\"");
                        }
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
