using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Study;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Learn.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Learn.Controllers
{
    public class CourseController : LearnBaseController
    {
        private readonly IStudyAppService studyAppService;

        public CourseController(IStudyAppService studyAppService)
        {
            this.studyAppService = studyAppService;
        }

        [Route("learn/course/listbyme")]
        public async Task<PartialViewResult> ListByMe()
        {
            List<StudyCourseListItemVo> model;

            OperationResultVo serviceResult = await studyAppService.GetCoursesByMe(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<StudyCourseListItemVo> castResult = serviceResult as OperationResultListVo<StudyCourseListItemVo>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<StudyCourseListItemVo>();
            }

            ViewData["ListDescription"] = SharedLocalizer["My Courses"].ToString();

            return PartialView("_ListCourses", model);
        }

        [Route("learn/course/listmine")]
        public async Task<PartialViewResult> ListMine()
        {
            List<StudyCourseListItemVo> model;

            OperationResultVo serviceResult = await studyAppService.GetMyCourses(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<StudyCourseListItemVo> castResult = serviceResult as OperationResultListVo<StudyCourseListItemVo>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<StudyCourseListItemVo>();
            }

            ViewData["ListDescription"] = SharedLocalizer["My Courses"].ToString();

            return PartialView("_ListCourses", model);
        }

        [Route("learn/course/list")]
        public async Task<PartialViewResult> List(string backUrl)
        {
            List<StudyCourseListItemVo> model;

            OperationResultVo serviceResult = await studyAppService.GetCourses(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<StudyCourseListItemVo> castResult = serviceResult as OperationResultListVo<StudyCourseListItemVo>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<StudyCourseListItemVo>();
            }

            ViewData["ListDescription"] = SharedLocalizer["Courses"].ToString();

            SetBackUrl(backUrl);

            return PartialView("_ListCoursesCards", model);
        }

        [Route("learn/course/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, string backUrl)
        {
            CourseViewModel vm;

            OperationResultVo serviceResult = await studyAppService.GetCourseById(CurrentUserId, id);

            if (serviceResult.Success)
            {
                OperationResultVo<CourseViewModel> castResult = serviceResult as OperationResultVo<CourseViewModel>;

                vm = castResult.Value;
            }
            else
            {
                vm = new CourseViewModel();
            }

            FormatToShow(vm);

            SetBackUrl(backUrl);

            return View("CourseDetailsWrapper", vm);
        }

        [Route("learn/course/add")]
        public ViewResult AddCourse()
        {
            CourseViewModel model;

            OperationResultVo serviceResult = studyAppService.GenerateNewCourse(CurrentUserId);

            OperationResultVo<CourseViewModel> castResult = serviceResult as OperationResultVo<CourseViewModel>;

            model = castResult.Value;

            return View("CourseCreateEditWrapper", model);
        }

        [Route("learn/course/edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            CourseViewModel model;

            OperationResultVo serviceResult = await studyAppService.GetCourseById(CurrentUserId, id);

            OperationResultVo<CourseViewModel> castResult = serviceResult as OperationResultVo<CourseViewModel>;

            model = castResult.Value;

            return View("CourseCreateEditWrapper", model);
        }

        [Route("learn/course/save")]
        public async Task<IActionResult> SaveCourse(CourseViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await studyAppService.SaveCourse(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("details", "course", new { area = "learn", id = saveResult.Value });

                    if (isNew)
                    {
                        url = Url.Action("edit", "course", new { area = "learn", id = saveResult.Value, pointsEarned = saveResult.PointsEarned });

                        if (EnvName.Equals(Constants.ProductionEnvironmentName))
                        {
                            await NotificationSender.SendTeamNotificationAsync("New Course created!");
                        }
                    }

                    return Json(new OperationResultRedirectVo<Guid>(saveResult, url));
                }
                else
                {
                    return Json(new OperationResultVo(false));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpDelete("learn/course/delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, bool redirect)
        {
            try
            {
                OperationResultVo deleteResult = await studyAppService.DeleteCourse(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (redirect)
                    {
                        string url = Url.Action("index", "study", new { area = "learn", msg = deleteResult.Message });
                        deleteResult.Message = null;

                        return Json(new OperationResultRedirectVo(deleteResult, url));
                    }

                    return Json(deleteResult);
                }
                else
                {
                    return Json(deleteResult);
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Route("learn/course/{courseId:guid}/listplans")]
        public async Task<PartialViewResult> ListPlans(Guid courseId)
        {
            ViewData["ListDescription"] = SharedLocalizer["Study Plans"].ToString();

            List<StudyCourseListItemVo> model = new List<StudyCourseListItemVo>();

            try
            {
                OperationResultVo result = await studyAppService.GetPlans(CurrentUserId, courseId);

                if (result.Success)
                {
                    List<StudyPlanViewModel> castResult = (result as OperationResultListVo<StudyPlanViewModel>).Value.ToList();

                    FormatToShow(castResult);

                    return PartialView("_ListPlans", castResult);
                }
                else
                {
                    return PartialView("_ListPlans", model);
                }
            }
            catch (Exception)
            {
                return PartialView("_ListPlans", model);
            }
        }

        [Route("learn/course/{courseId:guid}/edit/plans/")]
        public async Task<PartialViewResult> ListPlansForEdit(Guid courseId)
        {
            ViewData["ListDescription"] = SharedLocalizer["Study Plans"].ToString();

            List<StudyCourseListItemVo> model = new List<StudyCourseListItemVo>();

            try
            {
                OperationResultVo result = await studyAppService.GetPlans(CurrentUserId, courseId);

                if (result.Success)
                {
                    OperationResultListVo<StudyPlanViewModel> castResult = result as OperationResultListVo<StudyPlanViewModel>;

                    return PartialView("_ListPlansForEdit", castResult.Value);
                }
                else
                {
                    return PartialView("_ListPlansForEdit", model);
                }
            }
            catch (Exception)
            {
                return PartialView("_ListPlansForEdit", model);
            }
        }

        [Authorize]
        [HttpPost("study/course/{courseId:guid}/saveplans/")]
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        [RequestSizeLimit(int.MaxValue)]
        public async Task<IActionResult> SavePlans(Guid courseId, IEnumerable<StudyPlanViewModel> plans)
        {
            try
            {
                OperationResultVo result = await studyAppService.SavePlans(CurrentUserId, courseId, plans);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("study/course/{courseId:guid}/enroll/")]
        public async Task<IActionResult> Enroll(Guid courseId)
        {
            try
            {
                OperationResultVo result = await studyAppService.EnrollCourse(CurrentUserId, courseId);

                string url = Url.Action("details", "course", new { area = "learn", id = courseId });

                return Json(new OperationResultRedirectVo(result, url));
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("study/course/{courseId:guid}/leave/")]
        public async Task<IActionResult> Leave(Guid courseId)
        {
            try
            {
                OperationResultVo result = await studyAppService.LeaveCourse(CurrentUserId, courseId);

                string url = Url.Action("details", "course", new { area = "learn", id = courseId });

                return Json(new OperationResultRedirectVo(result, url));
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        private void FormatToShow(CourseViewModel model)
        {
            model.Description = string.IsNullOrWhiteSpace(model.Description) ? SharedLocalizer["No Description to show."] : model.Description.Replace("\n", "<br />");
        }

        private void FormatToShow(List<StudyPlanViewModel> castResult)
        {
            foreach (StudyPlanViewModel plan in castResult)
            {
                plan.Description = string.IsNullOrWhiteSpace(plan.Description) ? SharedLocalizer["No Description to show."] : plan.Description.Replace("\n", "<br />");
            }
        }

        private void SetBackUrl(string backUrl)
        {
            ViewData["BackUrl"] = string.IsNullOrWhiteSpace(backUrl) ? Url.Action("index", "study", new { area = "learn" }) : backUrl;
        }
    }
}