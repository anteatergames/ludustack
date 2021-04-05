using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Brainstorm;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using LuduStack.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    public class BrainstormController : SecureBaseController
    {
        private readonly IBrainstormAppService brainstormAppService;

        public BrainstormController(IBrainstormAppService brainstormAppService)
        {
            this.brainstormAppService = brainstormAppService;
        }

        [Route("brainstorm/{id:guid}")]
        [Route("brainstorm")]
        public async Task<IActionResult> Index(Guid? id)
        {
            BrainstormSessionViewModel currentSession;

            OperationResultListVo<BrainstormSessionViewModel> sessions = await brainstormAppService.GetSessions(CurrentUserId);

            ViewData["Sessions"] = sessions.Value;

            if (id.HasValue && id.Value != Guid.Empty && sessions.Value.Any(x => x.Id == id))
            {
                currentSession = sessions.Value.FirstOrDefault(x => x.Id == id.Value);
            }
            else
            {
                currentSession = sessions.Value.FirstOrDefault(x => x.Type == BrainstormSessionType.Main);
            }

            return View(currentSession);
        }

        [Route("brainstorm/{sessionId:guid}/newidea")]
        [Route("brainstorm/newidea")]
        public async Task<IActionResult> NewIdea(Guid sessionId)
        {
            OperationResultVo<BrainstormSessionViewModel> sessionResult;

            if (sessionId == Guid.Empty)
            {
                sessionResult = brainstormAppService.GetMainSession();
            }
            else
            {
                sessionResult = await brainstormAppService.GetSession(sessionId);
            }

            if (sessionResult.Success)
            {
                sessionId = sessionResult.Value.Id;

                ViewData["Session"] = sessionResult.Value;
            }

            BrainstormIdeaViewModel vm = new BrainstormIdeaViewModel
            {
                SessionId = sessionId
            };

            if (Request.IsAjaxRequest())
            {
                return PartialView("_CreateEdit", vm);
            }
            else
            {
                return View("_CreateEdit", vm);
            }
        }

        public PartialViewResult NewSession()
        {
            BrainstormSessionViewModel vm = new BrainstormSessionViewModel
            {
                Type = BrainstormSessionType.Generic
            };

            return PartialView("_CreateEditSession", vm);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            OperationResultVo<BrainstormIdeaViewModel> op = await brainstormAppService.GetById(CurrentUserId, id);

            BrainstormIdeaViewModel vm = op.Value;

            await SetAuthorDetails(vm);

            return View("_Details", vm);
        }

        [Route("brainstorm/list/{sessionId:guid}")]
        [Route("brainstorm/list")]
        public PartialViewResult List(Guid sessionId)
        {
            OperationResultListVo<BrainstormIdeaViewModel> serviceResult = brainstormAppService.GetAllBySessionId(CurrentUserId, sessionId);

            IEnumerable<BrainstormIdeaViewModel> items = serviceResult.Value;

            return PartialView("_List", items);
        }

        [HttpPost]
        public IActionResult Save(BrainstormIdeaViewModel vm)
        {
            try
            {
                bool isNew = vm.Id == Guid.Empty;

                vm.UserId = CurrentUserId;

                brainstormAppService.Save(CurrentUserId, vm);

                string url = Url.Action("Index", "Brainstorm", new { area = string.Empty, id = vm.SessionId.ToString() });

                if (isNew && EnvName.Equals(ConstantHelper.ProductionEnvironmentName))
                {
                    NotificationSender.SendTeamNotificationAsync($"New idea posted: {vm.Title}");
                }

                return Json(new OperationResultRedirectVo(url));
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSession(BrainstormSessionViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await brainstormAppService.SaveSession(CurrentUserId, vm);

                if (!saveResult.Success)
                {
                    return Json(new OperationResultVo(saveResult.Message));
                }
                else
                {
                    string url = Url.Action("Index", "Brainstorm", new { area = string.Empty, id = saveResult.Value.ToString() });

                    if (isNew && EnvName.Equals(ConstantHelper.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync($"New brainstorm session created: {vm.Title}");
                    }

                    return Json(new OperationResultRedirectVo(url));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [HttpPost]
        public IActionResult Vote(BrainstormVoteViewModel vm)
        {
            try
            {
                brainstormAppService.Vote(CurrentUserId, vm.VotingItemId, vm.VoteValue);

                string url = Url.Action("Index", "Brainstorm", new { area = string.Empty });

                return Json(new OperationResultRedirectVo(url));
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [HttpPost("brainstorm/comment")]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            OperationResultVo response;

            await SetAuthorDetails(vm);

            response = brainstormAppService.Comment(vm);

            return Json(response);
        }

        [HttpPost("brainstorm/changestatus/{ideaId:guid}")]
        public IActionResult ChangeStatus(Guid ideaId, BrainstormIdeaStatus selectedStatus)
        {
            try
            {
                brainstormAppService.ChangeStatus(CurrentUserId, ideaId, selectedStatus);

                string url = Url.Action("Index", "Brainstorm", new { area = string.Empty });

                return Json(new OperationResultRedirectVo(url));
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }
    }
}