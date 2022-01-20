using LuduStack.Application.ViewModels.GameIdea;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IGameIdeaAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultVo<GameIdeaListViewModel>> Get(Guid currentUserId, SupportedLanguage? language);

        Task<OperationResultVo<GameIdeaViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, GameIdeaViewModel viewModel);

        Task<OperationResultVo> ValidateDescription(Guid currentUserId, string description, Guid id, SupportedLanguage language, GameIdeaElementType type);
    }
}