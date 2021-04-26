using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IJobPositionDomainService
    {
        void AddApplicant(Guid userId, Guid jobPositionId, string email, string coverLetter);

        IEnumerable<JobPosition> GetAllAvailable();

        JobPosition GenerateNewJobPosition(Guid currentUserId, JobPositionOrigin origin);

        Dictionary<JobPositionStatus, int> GetPositionsStats(Guid userId);

        List<JobPositionApplicationVo> GetApplicationsByUserId(Guid userId);
    }
}