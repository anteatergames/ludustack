using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IProfileAppService : ICrudAppService<ProfileViewModel>, IProfileBaseAppService
    {
        Task<OperationResultListVo<ProfileViewModel>> GetAll(Guid currentUserId);

        OperationResultListVo<ProfileViewModel> GetAll(Guid currentUserId, bool noCache);

        UserProfileEssentialVo GetBasicDataByUserId(Guid userId);

        Task<ProfileViewModel> GetByUserId(Guid userId, ProfileType type);

        Task<ProfileViewModel> GetByUserId(Guid userId, ProfileType type, bool forEdit);

        Task<ProfileViewModel> Get(Guid currentUserId, Guid userId, string userHandler, ProfileType type);

        Task<ProfileViewModel> Get(Guid currentUserId, Guid userId, string userHandler, ProfileType type, bool forEdit);

        ProfileViewModel GenerateNewOne(ProfileType type);

        OperationResultVo UserFollow(Guid currentUserId, Guid userId);

        OperationResultVo UserUnfollow(Guid currentUserId, Guid userId);

        #region Connections

        OperationResultVo Connect(Guid currentUserId, Guid userId, UserConnectionType connectionType);

        OperationResultVo Disconnect(Guid currentUserId, Guid userId);

        OperationResultVo Allow(Guid currentUserId, Guid userId);

        OperationResultVo Deny(Guid currentUserId, Guid userId);

        OperationResultVo GetConnectionsByUserId(Guid userId);

        void SetProfileCache(Guid key, ProfileViewModel viewModel);

        ProfileViewModel GetUserProfileWithCache(Guid userId);

        #endregion Connections
    }
}