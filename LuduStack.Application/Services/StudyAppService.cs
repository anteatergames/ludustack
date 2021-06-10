using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Study;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Course;
using LuduStack.Domain.Messaging.Queries.Study;
using LuduStack.Domain.Messaging.Queries.UserProfile;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Domain.ValueObjects.Study;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class StudyAppService : ProfileBaseAppService, IStudyAppService
    {
        private readonly IStudyDomainService studyDomainService;
        private readonly IGamificationDomainService gamificationDomainService;

        public StudyAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon
            , IStudyDomainService studyDomainService
            , IGamificationDomainService gamificationDomainService) : base(profileBaseAppServiceCommon)
        {
            this.studyDomainService = studyDomainService;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<OperationResultVo> GetMyMentors(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> mentors = await mediator.Query<GetMentorsByUserIdQuery, IEnumerable<Guid>>(new GetMentorsByUserIdQuery(currentUserId));

                List<ProfileViewModel> finalList = new List<ProfileViewModel>();

                foreach (Guid mentorId in mentors)
                {
                    if (!finalList.Any(x => x.UserId == mentorId))
                    {
                        UserProfileEssentialVo profile = await GetCachedEssentialProfileByUserId(mentorId);

                        if (profile != null)
                        {
                            ProfileViewModel vm = mapper.Map<ProfileViewModel>(profile);

                            vm.ProfileImageUrl = UrlFormatter.ProfileImage(vm.UserId, 84);
                            vm.CoverImageUrl = UrlFormatter.ProfileCoverImage(vm.UserId, vm.Id, vm.LastUpdateDate, profile.HasCoverImage, 300);

                            finalList.Add(vm);
                        }
                    }
                }

                return new OperationResultListVo<ProfileViewModel>(finalList);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetMyStudents(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> students = await mediator.Query<GetStudentsByUserIdQuery, IEnumerable<Guid>>(new GetStudentsByUserIdQuery(currentUserId));

                List<ProfileViewModel> finalList = new List<ProfileViewModel>();

                foreach (Guid studentId in students)
                {
                    if (!finalList.Any(x => x.UserId == studentId))
                    {
                        UserProfileEssentialVo profile = await GetCachedEssentialProfileByUserId(studentId);

                        if (profile != null)
                        {
                            ProfileViewModel vm = mapper.Map<ProfileViewModel>(profile);

                            vm.ProfileImageUrl = UrlFormatter.ProfileImage(vm.UserId, 84);
                            vm.CoverImageUrl = UrlFormatter.ProfileCoverImage(vm.UserId, vm.Id, vm.LastUpdateDate, profile.HasCoverImage, 300);

                            finalList.Add(vm);
                        }
                    }
                }

                return new OperationResultListVo<ProfileViewModel>(finalList);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        #region Course

        public async Task<OperationResultVo> GetCourses(Guid currentUserId)
        {
            try
            {
                List<StudyCourseListItemVo> courses = await mediator.Query<GetCoursesQuery, List<StudyCourseListItemVo>>(new GetCoursesQuery());

                foreach (StudyCourseListItemVo course in courses)
                {
                    course.FeaturedImage = SetFeaturedImage(course.UserId, course.FeaturedImage, ImageRenderType.Small, Constants.DefaultCourseThumbnail);
                }

                return new OperationResultListVo<StudyCourseListItemVo>(courses);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetCoursesByMe(Guid currentUserId)
        {
            try
            {
                List<StudyCourseListItemVo> courses = await mediator.Query<GetCoursesByUserIdQuery, List<StudyCourseListItemVo>>(new GetCoursesByUserIdQuery(currentUserId));

                return new OperationResultListVo<StudyCourseListItemVo>(courses);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetMyCourses(Guid currentUserId)
        {
            try
            {
                StudyCoursesOfUserVo courses = await mediator.Query<GetCoursesForUserIdQuery, StudyCoursesOfUserVo>(new GetCoursesForUserIdQuery(currentUserId));

                List<StudyCourseListItemVo> finalList = new List<StudyCourseListItemVo>();

                foreach (UserCourseVo course in courses.Courses)
                {
                    if (!finalList.Any(x => x.Id == course.CourseId))
                    {
                        StudyCourseListItemVo vm = new StudyCourseListItemVo
                        {
                            Id = course.CourseId,
                            Name = course.CourseName
                        };

                        finalList.Add(vm);
                    }
                }

                return new OperationResultListVo<StudyCourseListItemVo>(finalList);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GenerateNewCourse(Guid currentUserId)
        {
            try
            {
                StudyCourse model = studyDomainService.GenerateNewCourse(currentUserId);

                CourseViewModel vm = mapper.Map<CourseViewModel>(model);

                vm.FeaturedImage = SetFeaturedImage(currentUserId, vm.FeaturedImage, ImageRenderType.Full, Constants.DefaultCourseThumbnail);

                return new OperationResultVo<CourseViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> SaveCourse(Guid currentUserId, CourseViewModel vm)
        {
            int pointsEarned = 0;

            try
            {
                StudyCourse model;

                StudyCourse existing = await mediator.Query<GetCourseByIdQuery, StudyCourse>(new GetCourseByIdQuery(vm.Id));
                if (existing != null)
                {
                    model = mapper.Map(vm, existing);
                }
                else
                {
                    model = mapper.Map<StudyCourse>(vm);
                }

                CommandResult result = await mediator.SendCommand(new SaveCourseCommand(currentUserId, model));

                if (model.Id == Guid.Empty && result.Validation.IsValid)
                {
                    pointsEarned += gamificationDomainService.ProcessAction(currentUserId, PlatformAction.CourseAdd);
                }

                return new OperationResultVo<Guid>(model.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> DeleteCourse(Guid currentUserId, Guid id)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new DeleteCourseCommand(currentUserId, id));

                if (result.Validation.IsValid)
                {
                    return new OperationResultVo(true, "That Course is gone now!");
                }
                else
                {
                    return new OperationResultVo(false, result.Validation.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetCourseById(Guid currentUserId, Guid id)
        {
            try
            {
                StudyCourse existing = await mediator.Query<GetCourseByIdQuery, StudyCourse>(new GetCourseByIdQuery(id));

                UserProfileEssentialVo profile = await mediator.Query<GetBasicUserProfileDataByUserIdQuery, UserProfileEssentialVo>(new GetBasicUserProfileDataByUserIdQuery(existing.UserId));

                CourseViewModel vm = mapper.Map<CourseViewModel>(existing);

                SetAuthorDetails(currentUserId, vm, profile);

                SetPermissions(currentUserId, vm);

                vm.FeaturedImage = SetFeaturedImage(vm.UserId, vm.FeaturedImage, ImageRenderType.Full, Constants.DefaultCourseThumbnail);

                vm.Description = ContentFormatter.FormatCFormatTextAreaBreaks(vm.Description);

                return new OperationResultVo<CourseViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetPlans(Guid currentUserId, Guid courseId)
        {
            try
            {
                List<StudyPlan> plans = await mediator.Query<GetPlansQuery, List<StudyPlan>>(new GetPlansQuery(courseId));

                List<StudyPlanViewModel> vms = mapper.Map<IEnumerable<StudyPlan>, IEnumerable<StudyPlanViewModel>>(plans).ToList();

                vms = vms.OrderBy(x => x.Order).ToList();

                return new OperationResultListVo<StudyPlanViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> SavePlans(Guid currentUserId, Guid courseId, IEnumerable<StudyPlanViewModel> plans)
        {
            try
            {
                List<StudyPlan> entities = mapper.Map<IEnumerable<StudyPlanViewModel>, IEnumerable<StudyPlan>>(plans).ToList();

                foreach (StudyPlan term in entities)
                {
                    term.UserId = currentUserId;
                }

                await mediator.SendCommand(new SavePlansCommand(currentUserId, courseId, entities));

                return new OperationResultVo(true, "Plans Updated!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> EnrollCourse(Guid currentUserId, Guid courseId)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new EnrollCourseCommand(currentUserId, courseId));

                if (!result.Validation.IsValid || !result.Success)
                {
                    return new OperationResultVo(false, "Can't enroll you.");
                }

                return new OperationResultVo(true, "You have enrolled to this course!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> LeaveCourse(Guid currentUserId, Guid courseId)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new LeaveCourseCommand(currentUserId, courseId));

                if (!result.Validation.IsValid || !result.Success)
                {
                    return new OperationResultVo(false, "Can't get you out of this course.");
                }

                Task<bool> task = unitOfWork.Commit();

                task.Wait();

                return new OperationResultVo(true, "You have left this course!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        #endregion Course

        #region Private Methods

        private void SetPermissions(Guid currentUserId, CourseViewModel vm)
        {
            vm.Permissions.CanConnect = vm.UserId != currentUserId && !vm.Members.Any(x => x.UserId == currentUserId);

            SetBasePermissions(currentUserId, vm);
        }

        #endregion Private Methods
    }
}