using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using LuduStack.Web.Enums;
using LuduStack.Web.Extensions;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    [Route("user")]
    public class UserController : SecureBaseController
    {
        private readonly IProfileAppService profileAppService;
        private readonly INotificationAppService notificationAppService;

        public UserController(IProfileAppService profileAppService
            , INotificationAppService notificationAppService) : base()
        {
            this.profileAppService = profileAppService;
            this.notificationAppService = notificationAppService;
        }

        public IActionResult Index()
        {
            ProfileViewModel model = profileAppService.GenerateNewOne(ProfileType.Personal);

            return View(model);
        }

        [Route("edit")]
        public IActionResult Edit()
        {
            return View();
        }

        [Route("list")]
        public async Task<IActionResult> List()
        {
            OperationResultListVo<ProfileViewModel> serviceResult = await profileAppService.GetAllEssential(CurrentUserId, false);

            List<ProfileViewModel> profiles = serviceResult.Value.OrderByDescending(x => x.CreateDate).ToList();

            return View("_UserList", profiles);
        }

        [Route("search")]
        public IActionResult Search(string term)
        {
            Select2SearchResultViewModel vm = new Select2SearchResultViewModel();

            OperationResultVo serviceResult = profileAppService.Search(term);

            if (serviceResult.Success)
            {
                IEnumerable<ProfileSearchViewModel> searchResults = ((OperationResultListVo<ProfileSearchViewModel>)serviceResult).Value;

                foreach (ProfileSearchViewModel item in searchResults)
                {
                    Select2SearchResultItemViewModel s2obj = new Select2SearchResultItemViewModel
                    {
                        Id = item.UserId.ToString(),
                        Text = item.Name
                    };

                    vm.Results.Add(s2obj);
                }

                return Json(vm);
            }
            else
            {
                return Json(serviceResult);
            }
        }

        #region User Follow/Unfollow

        [HttpPost]
        [Route("follow")]
        public IActionResult FollowUser(Guid userId)
        {
            OperationResultVo response = profileAppService.UserFollow(CurrentUserId, userId);

            string fullName = GetSessionValue(SessionValues.FullName);

            notificationAppService.Notify(CurrentUserId, fullName, userId, NotificationType.FollowYou, CurrentUserId);

            return Json(response);
        }

        [HttpPost]
        [Route("unfollow")]
        public IActionResult UnFollowUser(Guid userId)
        {
            OperationResultVo response = profileAppService.UserUnfollow(CurrentUserId, userId);

            return Json(response);
        }

        #endregion User Follow/Unfollow

        #region User Connection

        [HttpGet]
        [Route("connections/{userId:guid}")]
        public IActionResult Connections(Guid userId)
        {
            OperationResultListVo<UserConnectionViewModel> connections = (OperationResultListVo<UserConnectionViewModel>)profileAppService.GetConnectionsByUserId(userId);

            List<UserConnectionViewModel> model;

            if (connections.Success)
            {
                model = connections.Value.ToList();
            }
            else
            {
                model = new List<UserConnectionViewModel>();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("~/Views/Shared/_Connections.cshtml", model);
            }

            return View("~/Views/Shared/_Connections.cshtml", model);
        }

        [HttpPost]
        [Route("connect")]
        public IActionResult ConnectToUser(Guid userId, UserConnectionType connectionType)
        {
            OperationResultVo response = profileAppService.Connect(CurrentUserId, userId, connectionType);

            string fullName = GetSessionValue(SessionValues.FullName);

            notificationAppService.Notify(CurrentUserId, fullName, userId, NotificationType.ConnectionRequest, CurrentUserId);

            return Json(response);
        }

        [HttpPost]
        [Route("disconnect")]
        public IActionResult DisconnectUser(Guid userId)
        {
            OperationResultVo response = profileAppService.Disconnect(CurrentUserId, userId);

            return Json(response);
        }

        [HttpPost]
        [Route("allowconnection")]
        public IActionResult AllowUser(Guid userId)
        {
            OperationResultVo response = profileAppService.Allow(CurrentUserId, userId);

            return Json(response);
        }

        [HttpPost]
        [Route("denyconnection")]
        public IActionResult DenyUser(Guid userId)
        {
            OperationResultVo response = profileAppService.Deny(CurrentUserId, userId);

            return Json(response);
        }

        #endregion User Connection
    }
}