using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.PlatformSetting;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.PlatformSetting;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class PlatformSettingAppService : ProfileBaseAppService, IPlatformSettingAppService
    {
        public PlatformSettingAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon) : base(profileBaseAppServiceCommon)
        {
        }

        public async Task<OperationResultListVo<PlatformSettingViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                List<PlatformSettingViewModel> vms = new List<PlatformSettingViewModel>();

                IEnumerable<PlatformSetting> allModels = await mediator.Query<GetPlatformSettingQuery, IEnumerable<PlatformSetting>>(new GetPlatformSettingQuery());

                Array allSettings = Enum.GetValues(typeof(PlatformSettingElement));

                foreach (PlatformSettingElement setting in allSettings)
                {
                    Domain.Core.Attributes.UiInfoAttribute uiInfo = setting.ToUiInfo();

                    PlatformSettingViewModel vm = new PlatformSettingViewModel
                    {
                        Element = setting,
                        Type = (PlatformSettingType)uiInfo.Type,
                        Group = (PlatformSettingGroup)uiInfo.SubType,
                    };

                    PlatformSetting savedSetting = allModels.FirstOrDefault(x => x.Element == setting);

                    if (savedSetting != null)
                    {
                        vm.Id = savedSetting.Id;
                        vm.Saved = true;
                        vm.DefaultValue = uiInfo.DefaultValue;
                        vm.Value = savedSetting.Value;
                    }
                    else
                    {
                        vm.Value = uiInfo.DefaultValue;
                        vm.DefaultValue = uiInfo.DefaultValue;
                    }

                    if (vm.Type == PlatformSettingType.Boolean)
                    {
                        vm.Value = vm.Value == "0" ? "No" : "Yes";
                        vm.DefaultValue = vm.DefaultValue == "0" ? "No" : "Yes";
                    }

                    vms.Add(vm);
                }

                return new OperationResultListVo<PlatformSettingViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<PlatformSettingViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<PlatformSettingViewModel>> GetForEdit(Guid currentUserId, PlatformSettingElement element)
        {
            try
            {
                UiInfoAttribute uiInfo = element.ToUiInfo();

                PlatformSettingViewModel vm = new PlatformSettingViewModel
                {
                    Element = element,
                    Type = (PlatformSettingType)uiInfo.Type,
                    Group = (PlatformSettingGroup)uiInfo.SubType,
                };

                PlatformSetting model = await mediator.Query<GetPlatformSettingByElementQuery, PlatformSetting>(new GetPlatformSettingByElementQuery(element));

                if (model != null)
                {
                    vm.Id = model.Id;
                    vm.Value = model.Value;
                }
                else
                {
                    vm.Value = uiInfo.DefaultValue;
                }

                return new OperationResultVo<PlatformSettingViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<PlatformSettingViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<PlatformSettingViewModel>> GetByElement(Guid currentUserId, PlatformSettingElement element)
        {
            try
            {
                UiInfoAttribute uiInfo = element.ToUiInfo();

                PlatformSettingViewModel vm = new PlatformSettingViewModel
                {
                    Element = element,
                    Type = (PlatformSettingType)uiInfo.Type,
                    Group = (PlatformSettingGroup)uiInfo.SubType,
                };

                string cachedSetting = cacheService.Get<PlatformSettingElement, string>(element);

                if (!string.IsNullOrWhiteSpace(cachedSetting))
                {
                    vm.Value = cachedSetting;
                }
                else
                {
                    PlatformSetting model = await mediator.Query<GetPlatformSettingByElementQuery, PlatformSetting>(new GetPlatformSettingByElementQuery(element));

                    if (model != null)
                    {
                        vm.Id = model.Id;
                        vm.Value = model.Value;
                    }
                    else
                    {
                        vm.Value = uiInfo.DefaultValue;
                    }
                }

                return new OperationResultVo<PlatformSettingViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<PlatformSettingViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> Reset(Guid currentUserId, Guid id)
        {
            try
            {
                CommandResult<PlatformSetting> result = await mediator.SendCommand<DeletePlatformSettingCommand, PlatformSetting>(new DeletePlatformSettingCommand(currentUserId, id));

                if (!result.Validation.IsValid)
                {
                    return new OperationResultVo(result.Validation.Errors.First().ErrorMessage);
                }
                else
                {
                    UiInfoAttribute uiInfo = result.Result.Element.ToUiInfo();

                    cacheService.Set(result.Result.Element, uiInfo.DefaultValue);
                    return new OperationResultVo(true, "That Platform Setting is gone now!");
                }
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, PlatformSettingViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                PlatformSetting model = mapper.Map<PlatformSetting>(viewModel);
                string msg = "Platform Setting Saved";

                CommandResult<Guid> result = await mediator.SendCommand<SavePlatformSettingCommand, Guid>(new SavePlatformSettingCommand(currentUserId, viewModel.Element, viewModel.Value));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                pointsEarned += result.PointsEarned;
                model.Id = result.Result;

                cacheService.Set(model.Element, model.Value);

                return new OperationResultVo<Guid>(model.Id, pointsEarned, msg);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }
    }
}