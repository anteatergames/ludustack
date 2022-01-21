using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.GameIdea;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.GameIdea;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class GameIdeaAppService : BaseAppService, IGameIdeaAppService
    {
        public GameIdeaAppService(IBaseAppServiceCommon baseAppServiceCommon) : base(baseAppServiceCommon)
        {
        }

        public virtual async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountGameIdeaQuery, int>(new CountGameIdeaQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultVo<GameIdeaListViewModel>> Get(Guid currentUserId, SupportedLanguage? language)
        {
            GameIdeaListViewModel model = new GameIdeaListViewModel();
            model.Language = language.HasValue ? language.Value : SupportedLanguage.English;

            try
            {
                IEnumerable<GameIdea> allModels = await mediator.Query<GetGameIdeaQuery, IEnumerable<GameIdea>>(new GetGameIdeaQuery(language));

                IEnumerable<GameIdeaViewModel> vms = mapper.Map<IEnumerable<GameIdea>, IEnumerable<GameIdeaViewModel>>(allModels);

                IEnumerable<IGrouping<GameIdeaElementType, GameIdeaViewModel>> vmsByElementType = vms.GroupBy(x => x.Type);

                model.Elements = vmsByElementType.ToDictionary(x => x.Key, x => x.ToList());

                List<GameIdeaElementType> allElementTypes = Enum.GetValues(typeof(GameIdeaElementType)).Cast<GameIdeaElementType>().ToList();
                foreach (GameIdeaElementType elementType in allElementTypes)
                {
                    if (!model.Elements.Any(x => x.Key == elementType))
                    {
                        model.Elements.Add(elementType, new List<GameIdeaViewModel>());
                    }
                }

                model.Elements = model.Elements.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value.OrderBy(y => y.Description).ToList());

                return new OperationResultVo<GameIdeaListViewModel>(model);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<GameIdeaListViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<GameIdeaViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                GameIdea model = await mediator.Query<GetGameIdeaByIdQuery, GameIdea>(new GetGameIdeaByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<GameIdeaViewModel>("Entity not found!");
                }

                GameIdeaViewModel vm = mapper.Map<GameIdeaViewModel>(model);

                return new OperationResultVo<GameIdeaViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<GameIdeaViewModel>(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteGameIdeaCommand(currentUserId, id));

                return new OperationResultVo(true, "That Game Idea is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public virtual async Task<OperationResultVo<Guid>> Save(Guid currentUserId, GameIdeaViewModel viewModel)
        {
            try
            {
                OperationResultVo duplicationCheck = await ValidateDescription(currentUserId, viewModel.Description, viewModel.Id, viewModel.Language, viewModel.Type);

                if (!duplicationCheck.Success)
                {
                    string message = "Cmon, that already exists!";
                    return new OperationResultVo<Guid>(viewModel.Id, false, message);
                }

                GameIdea model;

                GameIdea existing = await mediator.Query<GetGameIdeaByIdQuery, GameIdea>(new GetGameIdeaByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<GameIdea>(viewModel);
                }

                CommandResult result = await mediator.SendCommand(new SaveGameIdeaCommand(model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id, 0, "Game Idea saved!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> ValidateDescription(Guid currentUserId, string description, Guid id, SupportedLanguage language, GameIdeaElementType type)
        {
            try
            {
                bool valid = await mediator.Query<CheckGameIdeaDescriptionQuery, bool>(new CheckGameIdeaDescriptionQuery(description, id, language, type));

                return new OperationResultVo(valid);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }
    }
}