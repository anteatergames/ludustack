using LuduStack.Application.ViewModels.PlatformSetting;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IPlatformSettingAppService
    {
        Task<OperationResultListVo<PlatformSettingViewModel>> GetAll(Guid currentUserId);

        Task<OperationResultVo<PlatformSettingViewModel>> GetForEdit(Guid currentUserId, PlatformSettingElement element);

        Task<OperationResultVo<PlatformSettingViewModel>> GetByElement(Guid currentUserId, PlatformSettingElement element);

        Task<OperationResultVo> Reset(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, PlatformSettingViewModel viewModel);
    }
}