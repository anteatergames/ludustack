using LuduStack.Application.ViewModels.Study;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IStudyAppService
    {
        Task<OperationResultVo> GetMyMentors(Guid currentUserId);

        Task<OperationResultVo> GetMyStudents(Guid currentUserId);

        Task<OperationResultVo> GetCourses(Guid currentUserId);

        Task<OperationResultVo> GetCoursesByMe(Guid currentUserId);

        Task<OperationResultVo> GetMyCourses(Guid currentUserId);

        OperationResultVo GenerateNewCourse(Guid currentUserId);

        Task<OperationResultVo<Guid>> SaveCourse(Guid currentUserId, CourseViewModel vm);

        Task<OperationResultVo> DeleteCourse(Guid currentUserId, Guid id);

        Task<OperationResultVo> GetCourseById(Guid currentUserId, Guid id);

        Task<OperationResultVo> GetPlans(Guid currentUserId, Guid courseId);

        Task<OperationResultVo> SavePlans(Guid currentUserId, Guid courseId, IEnumerable<StudyPlanViewModel> plans);

        Task<OperationResultVo> EnrollCourse(Guid currentUserId, Guid courseId);

        Task<OperationResultVo> LeaveCourse(Guid currentUserId, Guid courseId);
    }
}