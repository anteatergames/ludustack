using LuduStack.Application.ViewModels.Comics;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IComicsAppService : ICrudAppService<ComicStripViewModel>
    {
        Task<OperationResultVo> GetComicsByMe(Guid currentUserId);

        OperationResultVo GenerateNew(Guid currentUserId);

        Task<OperationResultVo> GetForEdit(Guid currentUserId, Guid id);

        Task<OperationResultVo> GetForDetails(Guid currentUserId, Guid id);

        OperationResultVo Rate(Guid currentUserId, Guid id, decimal scoreDecimal);
    }
}