﻿using LuduStack.Application.ViewModels.Study;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IStudyAppService
    {
        OperationResultVo GetMyMentors(Guid currentUserId);

        OperationResultVo GetMyStudents(Guid currentUserId);

        OperationResultVo GetCourses(Guid currentUserId);

        OperationResultVo GetCoursesByMe(Guid currentUserId);

        OperationResultVo GetMyCourses(Guid currentUserId);

        OperationResultVo GenerateNewCourse(Guid currentUserId);

        Task<OperationResultVo<Guid>> SaveCourse(Guid currentUserId, CourseViewModel vm);

        Task<OperationResultVo> DeleteCourse(Guid currentUserId, Guid id);

        Task<OperationResultVo> GetCourseById(Guid currentUserId, Guid id);

        OperationResultVo GetPlans(Guid currentUserId, Guid courseId);

        OperationResultVo SavePlans(Guid currentUserId, Guid courseId, IEnumerable<StudyPlanViewModel> plans);

        OperationResultVo EnrollCourse(Guid currentUserId, Guid courseId);

        OperationResultVo LeaveCourse(Guid currentUserId, Guid courseId);
    }
}