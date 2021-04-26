using LuduStack.Application.ViewModels.Comics;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IComicsAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultVo> GetAllIds(Guid currentUserId);

        Task<OperationResultVo<ComicStripViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, ComicStripViewModel viewModel);

        Task<OperationResultVo> GetComicsByMe(Guid currentUserId);

        OperationResultVo GenerateNew(Guid currentUserId);

        Task<OperationResultVo> GetForEdit(Guid currentUserId, Guid id);

        Task<OperationResultVo> GetForDetails(Guid currentUserId, Guid id);

        Task<OperationResultVo> Rate(Guid currentUserId, Guid id, decimal scoreDecimal);
    }
}