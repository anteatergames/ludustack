using LuduStack.Application.ViewModels.Game;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IGameAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, GameViewModel viewModel);

        Task<OperationResultVo<GameViewModel>> GetById(Guid currentUserId, Guid id);

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