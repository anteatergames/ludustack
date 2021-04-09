using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.UserPreferences;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.UserPreferences;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class UserPreferencesAppService : BaseAppService, IUserPreferencesAppService
    {
        public UserPreferencesAppService(IBaseAppServiceCommon baseAppServiceCommon) : base(baseAppServiceCommon)
        {
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, UserPreferencesViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                UserPreferences model;

                if (viewModel.Id == viewModel.UserId)
                {
                    viewModel.Id = Guid.Empty;
                }

                UserPreferences existing = await mediator.Query<GetUserPreferencesByIdQuery, UserPreferences>(new GetUserPreferencesByIdQuery(viewModel.Id));

                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<UserPreferences>(viewModel);
                }

                CommandResult result = await mediator.SendCommand(new SaveUserPreferencesCommand(currentUserId, model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                pointsEarned += result.PointsEarned;

                return new OperationResultVo<Guid>(model.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<UserPreferencesViewModel> GetByUserId(Guid userId)
        {
            IEnumerable<UserPreferences> list = await mediator.Query<GetUserPreferencesByUserIdQuery, IEnumerable<UserPreferences>>(new GetUserPreferencesByUserIdQuery(userId));

            UserPreferences model = list.FirstOrDefault();

            if (model == null)
            {
                model = new UserPreferences
                {
                    UserId = userId,
                    UiLanguage = SupportedLanguage.English
                };
            }

            UserPreferencesViewModel vm = mapper.Map<UserPreferencesViewModel>(model);

            return vm;
        }
    }
}