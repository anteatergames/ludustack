using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;

namespace LuduStack.Domain.Services
{
    public class StudyDomainService : IStudyDomainService
    {
        #region Course

        public StudyCourse GenerateNewCourse(Guid userId)
        {
            StudyCourse model = new StudyCourse
            {
                UserId = userId
            };

            return model;
        }

        #endregion Course
    }
}