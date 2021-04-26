using LuduStack.Application.ViewModels.Jobs;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IJobPositionAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId);

        Task<OperationResultVo<JobPositionViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, JobPositionViewModel viewModel);

        OperationResultVo GetAllAvailable(Guid currentUserId);

        Task<OperationResultVo> Apply(Guid currentUserId, Guid jobPositionId, string email, string coverLetter);

        OperationResultVo GenerateNew(Guid currentUserId, JobPositionOrigin origin);

        Task<OperationResultVo> GetAllMine(Guid currentUserId);

        Task<OperationResultVo> ChangeStatus(Guid currentUserId, Guid jobPositionId, JobPositionStatus selectedStatus);

        OperationResultVo GetMyPositionsStats(Guid currentUserId);

        Task<OperationResultVo> RateApplicant(Guid currentUserId, Guid jobPositionId, Guid userId, decimal score);

        OperationResultVo GetMyApplications(Guid currentUserId);
    }
}