using LuduStack.Application.ViewModels.Comics;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface IComicsAppService : ICrudAppService<ComicStripViewModel>
    {
        OperationResultVo GetComicsByMe(Guid currentUserId);
        OperationResultVo GenerateNew(Guid currentUserId);
        OperationResultVo GetForEdit(Guid currentUserId, Guid id);
        OperationResultVo GetForDetails(Guid currentUserId, Guid id);
        OperationResultVo Rate(Guid currentUserId, Guid id, decimal scoreDecimal);
    }
}
