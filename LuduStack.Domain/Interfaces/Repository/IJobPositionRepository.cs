using LuduStack.Domain.Models;
using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IJobPositionRepository : IRepository<JobPosition>
    {
        Task<bool> AddApplicant(Guid jobPositionId, JobApplicant applicant);
    }
}