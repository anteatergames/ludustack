using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IStudyCourseRepository : IRepository<StudyCourse>
    {
        Task<List<StudyCourseListItemVo>> GetCourses();

        Task<List<StudyCourseListItemVo>> GetCoursesByUserId(Guid userId);

        Task<List<StudyPlan>> GetPlans(Guid courseId);

        Task<bool> AddPlan(Guid courseId, StudyPlan plan);

        Task<bool> UpdatePlan(Guid courseId, StudyPlan plan);

        Task<bool> RemovePlan(Guid courseId, Guid planId);

        bool CheckStudentEnrolled(Guid courseId, Guid userId);

        Task AddStudent(Guid courseId, CourseMember student);

        Task UpdateStudent(Guid courseId, CourseMember student);

        Task RemoveStudent(Guid courseId, Guid userId);
    }
}