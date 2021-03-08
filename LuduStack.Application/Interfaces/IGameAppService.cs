using LuduStack.Application.ViewModels.Game;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IGameAppService : ICrudAppService<GameViewModel>
    {
        Task<OperationResultVo<GameViewModel>> GetById(Guid currentUserId, Guid id, bool forEdit);

        OperationResultVo<GameViewModel> CreateNew(Guid currentUserId);

        IEnumerable<GameListItemViewModel> GetLatest(Guid currentUserId, int count, Guid userId, Guid? teamId, GameGenre genre);

        Task<IEnumerable<SelectListItemVo>> GetByUser(Guid userId);

        OperationResultVo GameFollow(Guid currentUserId, Guid gameId);

        OperationResultVo GameUnfollow(Guid currentUserId, Guid gameId);

        OperationResultVo GameLike(Guid currentUserId, Guid gameId);

        OperationResultVo GameUnlike(Guid currentUserId, Guid gameId);
    }
}