using LuduStack.Domain.Models;
using System;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IStudyDomainService
    {
        #region Course

        StudyCourse GenerateNewCourse(Guid userId);

        #endregion Course
    }
}