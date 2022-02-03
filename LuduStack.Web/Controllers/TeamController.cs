using LuduStack.Application;
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
    [Authorize]
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

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("list")]
        public async Task<IActionResult> List()
        {
            OperationResultListVo<TeamViewModel> serviceResult = await teamAppService.GetAll(CurrentUserId, CurrentUserIsAdmin);

            List<TeamViewModel> model = serviceResult.Value.ToList();

            foreach (var team in model)
            {
                if (team.Name.StartsWith("Team "))
                {
                    team.Name = team.Name.Replace("Team ", string.Format("{0} ", SharedLocalizer["Team"]));
                }
            }

            return PartialView("_List", model);
        }

        [AllowAnonymous]
        [Route("list/user/{userId:guid}")]
        public async Task<IActionResult> ListByUser(Guid userId)
        {
            OperationResultListVo<TeamViewModel> serviceResult = await teamAppService.GetByUserId(userId, CurrentUserIsAdmin);

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

        [AllowAnonymous]
        [Route("{teamId:guid}")]
        public async Task<IActionResult> Details(Guid teamId, Guid notificationclicked)
        {
            await notificationAppService.MarkAsRead(notificationclicked);

            OperationResultVo<TeamViewModel> serviceResult = await teamAppService.GetById(CurrentUserId, CurrentUserIsAdmin, teamId);

            if (!serviceResult.Success)
            {
                return RedirectToAction("index", "team", new { area = string.Empty, msg = SharedLocalizer["Team not found!"] });
            }

            TeamViewModel model = serviceResult.Value;

            if (!model.Name.ToLower().Contains("team"))
            {
                model.Name = SharedLocalizer["{0} Team", model.Name];
            }

            return View(model);
        }

        [Route("new")]
        public async Task<IActionResult> New()
        {
            OperationResultVo<TeamViewModel> service = (OperationResultVo<TeamViewModel>)await teamAppService.GenerateNewTeam(CurrentUserId);

            return PartialView("_CreateEdit", service.Value);
        }

        [Route("edit/{teamId:guid}")]
        public async Task<IActionResult> Edit(Guid teamId)
        {
            OperationResultVo<TeamViewModel> service = await teamAppService.GetById(CurrentUserId, CurrentUserIsAdmin, teamId);

            TeamViewModel viewModel = service.Value;

            if (!CurrentUserIsAdmin && viewModel.UserId != CurrentUserId)
            {
                return RedirectToAction("details", "team", new { teamId, msg = SharedLocalizer["You cannot edit someone else's team!"] });
            }

            viewModel.RecruitingBefore = viewModel.Recruiting;

            return PartialView("_CreateEdit", viewModel);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save(TeamViewModel viewModel)
        {
            if (!CurrentUserIsAdmin && viewModel.UserId != CurrentUserId)
            {
                return RedirectToAction("details", "team", new { viewModel.Id, msg = SharedLocalizer["You cannot edit someone else's team!"] });
            }

            try
            {
                bool isNew = viewModel.Id == Guid.Empty;
                viewModel.UserId = CurrentUserId;

                IEnumerable<Guid> oldMembers = viewModel.Members.Where(x => x.Id != Guid.Empty).Select(x => x.Id);

                OperationResultVo<Guid> saveResult = await teamAppService.Save(CurrentUserId, viewModel);

                if (!saveResult.Success)
                {
                    return Json(new OperationResultVo(saveResult.Message));
                }
                else
                {
                    viewModel.Id = saveResult.Value;

                    string url = Url.Action("Index", "Team", new { area = string.Empty, id = saveResult.Value, pointsEarned = saveResult.PointsEarned });

                    Notify(viewModel, oldMembers);

                    bool recruiting = !viewModel.RecruitingBefore && viewModel.Recruiting;
                    await GenerateTeamPost(viewModel, isNew, recruiting);

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync($"New team Created: {viewModel.Name}");
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

        private async Task GenerateTeamPost(TeamViewModel vm, bool newTeam, bool recruiting)
        {
            if ((newTeam && vm.Members.Count > 1) || recruiting)
            {
                UserContentViewModel newContent = new UserContentViewModel
                {
                    AuthorName = GetSessionValue(SessionValues.FullName),
                    UserId = CurrentUserId,
                    UserContentType = UserContentType.TeamCreation,
                    Content = string.Format("{0}|{1}|{2}|{3}|{4}", vm.Id, vm.Name, vm.Motto, vm.Members.Count, recruiting)
                };

                await userContentAppService.Save(CurrentUserId, newContent);
            }
        }
    }
}