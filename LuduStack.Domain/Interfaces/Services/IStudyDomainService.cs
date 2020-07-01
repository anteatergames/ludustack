using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Domain.ValueObjects.Study;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IStudyDomainService
    {
        #region General

        IEnumerable<Guid> GetMentorsByUserId(Guid userId);

        IEnumerable<Guid> GetStudentsByUserId(Guid userId);

        #endregion General

        #region Course

        List<StudyCourseListItemVo> GetCourses();

        List<StudyCourseListItemVo> GetCoursesByUserId(Guid userId);

        StudyCoursesOfUserVo GetCoursesForUserId(Guid userId);

        StudyCourse GenerateNewCourse(Guid userId);

        StudyCourse GetCourseById(Guid id);

        void AddCourse(StudyCourse model);

        void UpdateCourse(StudyCourse model);

        IEnumerable<StudyPlan> GetPlans(Guid courseId);

        Task<bool> SavePlans(Guid courseId, List<StudyPlan> plans);

        void RemoveCourse(Guid id);

        Task<bool> EnrollCourse(Guid userId, Guid courseId);

        Task<bool> LeaveCourse(Guid userId, Guid courseId);

        #endregion Course
    }
}