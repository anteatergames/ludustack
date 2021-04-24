using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.Team;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using LuduStack.Web.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    [Route("team")]
    public class TeamController : SecureBaseController
    {
        private readonly ITeamAppService teamAppService;
        private readonly INotificationAppService notificationAppService;
        private readonly IUserContentAppService userContentAppService;

        public TeamController(ITeamAppService teamAppService
            , INotificationAppService notificationAppService
            , IUserContentAppService userContentAppService)
        {
            this.teamAppService = teamAppService;
            this.notificationAppService = notificationAppService;
            this.userContentAppService = userContentAppService;
        }

        public IActionResult Index(int? pointsEarned)
        {
            SetGamificationMessage(pointsEarned);

            return View();
        }

        [Route("list")]
        public async Task<IActionResult> List()
        {
            OperationResultListVo<TeamViewModel> serviceResult = await teamAppService.GetAll(CurrentUserId);

            List<TeamViewModel> model = serviceResult.Value.ToList();

            return PartialView("_List", model);
        }

        [Route("list/user/{userId:guid}")]
        public IActionResult ListByUser(Guid userId)
        {
            OperationResultListVo<TeamViewModel> serviceResult = (OperationResultListVo<TeamViewModel>)teamAppService.GetByUserId(userId);

            List<TeamViewModel> model = serviceResult.Value.ToList();

            foreach (TeamViewModel team in model)
            {
                team.Permissions.CanEdit = team.Permissions.CanDelete = false;
            }

            return PartialView("_List", model);
        }

        [Route("list/mine")]
        public IActionResult ListMyTeams()
        {
            OperationResultVo serviceResult = teamAppService.GetSelectListByUserId(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<SelectListItemVo> castResult = serviceResult as OperationResultListVo<SelectListItemVo>;

                List<SelectListItemVo> model = castResult.Value.ToList();

                return PartialView("_ListMine", model);
            }
            else
            {
                return null;
            }
        }

        [Route("{teamId:guid}")]
        public async Task<IActionResult> Details(Guid teamId, int? pointsEarned, Guid notificationclicked)
        {
            await notificationAppService.MarkAsRead(notificationclicked);

            OperationResultVo<TeamViewModel> serviceResult = await teamAppService.GetById(CurrentUserId, teamId);

            if (!serviceResult.Success)
            {
                return RedirectToAction("index", "team", new { area = string.Empty, msg = SharedLocalizer["Team not found!"] });
            }

            TeamViewModel model = serviceResult.Value;

            SetGamificationMessage(pointsEarned);

            return View(model);
        }

        [Authorize]
        [Route("edit/{teamId:guid}")]
        public async Task<IActionResult> Edit(Guid teamId)
        {
            OperationResultVo<TeamViewModel> service = await teamAppService.GetById(CurrentUserId, teamId);

            TeamViewModel model = service.Value;

            model.RecruitingBefore = model.Recruiting;

            return PartialView("_CreateEdit", model);
        }

        [Authorize]
        [Route("new")]
        public async Task<IActionResult> New()
        {
            OperationResultVo<TeamViewModel> service = (OperationResultVo<TeamViewModel>)await teamAppService.GenerateNewTeam(CurrentUserId);

            return PartialView("_CreateEdit", service.Value);
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<IActionResult> Save(TeamViewModel vm)
        {
            try
            {
                bool isNew = vm.Id == Guid.Empty;
                vm.UserId = CurrentUserId;

                IEnumerable<Guid> oldMembers = vm.Members.Where(x => x.Id != Guid.Empty).Select(x => x.Id);

                OperationResultVo<Guid> saveResult = await teamAppService.Save(CurrentUserId, vm);

                if (!saveResult.Success)
                {
                    return Json(new OperationResultVo(saveResult.Message));
                }
                else
                {
                    vm.Id = saveResult.Value;

                    string url = Url.Action("Index", "Team", new { area = string.Empty, id = saveResult.Value, pointsEarned = saveResult.PointsEarned });

                    Notify(vm, oldMembers);

                    bool recruiting = !vm.RecruitingBefore && vm.Recruiting;
                    GenerateTeamPost(vm, isNew, recruiting);

                    if (isNew && EnvName.Equals(ConstantHelper.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync($"New team Created: {vm.Name}");
                    }

                    return Json(new OperationResultRedirectVo(saveResult, url));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Route("{teamId:guid}/invitation/accept")]
        public async Task<IActionResult> AcceptInvitation(Guid teamId, string quote)
        {
            OperationResultVo serviceResult = await teamAppService.AcceptInvite(teamId, CurrentUserId, quote);

            return Json(serviceResult);
        }

        [Route("{teamId:guid}/invitation/reject")]
        public IActionResult RejectInvitation(Guid teamId)
        {
            OperationResultVo serviceResult = teamAppService.RejectInvite(teamId, CurrentUserId);

            return Json(serviceResult);
        }

        [HttpDelete("{teamId:guid}")]
        public async Task<IActionResult> DeleteTeam(Guid teamId)
        {
            OperationResultVo serviceResult = await teamAppService.Remove(CurrentUserId, teamId);

            return Json(serviceResult);
        }

        [HttpDelete("{teamId:guid}/{userId:guid}")]
        public IActionResult RemoveMember(Guid teamId, Guid userId)
        {
            OperationResultVo serviceResult = teamAppService.RemoveMember(CurrentUserId, teamId, userId);

            return Json(serviceResult);
        }

        [HttpPost("CandidateApply")]
        public async Task<IActionResult> CandidateApply(TeamMemberViewModel vm)
        {
            OperationResultVo serviceResult = await teamAppService.CandidateApply(CurrentUserId, vm);

            string url = Url.Action("Details", "Team", new { area = string.Empty, teamId = vm.TeamId, pointsEarned = serviceResult.PointsEarned });

            return Json(new OperationResultRedirectVo(serviceResult, url));
        }

        [HttpPost("AcceptCandidate")]
        public IActionResult AcceptCandidate(Guid teamId, Guid userId)
        {
            OperationResultVo serviceResult = teamAppService.AcceptCandidate(CurrentUserId, teamId, userId);

            string url = Url.Action("Details", "Team", new { area = string.Empty, teamId });

            return Json(new OperationResultRedirectVo(serviceResult, url));
        }

        [HttpPost("RejectCandidate")]
        public IActionResult RejectCandidate(Guid teamId, Guid userId)
        {
            OperationResultVo serviceResult = teamAppService.RejectCandidate(CurrentUserId, teamId, userId);

            string url = Url.Action("Details", "Team", new { area = string.Empty, teamId });

            return Json(new OperationResultRedirectVo(serviceResult, url));
        }

        private void Notify(TeamViewModel vm, IEnumerable<Guid> oldMembers)
        {
            string fullName = GetSessionValue(SessionValues.FullName);

            foreach (TeamMemberViewModel member in vm.Members.Where(x => !x.Leader))
            {
                if (!oldMembers.Contains(member.Id))
                {
                    notificationAppService.Notify(CurrentUserId, fullName, member.UserId, NotificationType.TeamInvitation, vm.Id);
                }
            }
        }

        private void GenerateTeamPost(TeamViewModel vm, bool newTeam, bool recruiting)
        {
            if ((newTeam && vm.Members.Count > 1) || recruiting)
            {
                UserContentViewModel newContent = new UserContentViewModel
                {
                    AuthorName = GetSessionValue(SessionValues.FullName),
                    UserId = CurrentUserId,
                    UserContentType = UserContentType.TeamCreation,
                    Content = String.Format("{0}|{1}|{2}|{3}|{4}", vm.Id, vm.Name, vm.Motto, vm.Members.Count, recruiting)
                };

                userContentAppService.Save(CurrentUserId, newContent);
            }
        }
    }
}