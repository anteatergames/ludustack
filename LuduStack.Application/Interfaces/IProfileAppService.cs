﻿using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IProfileAppService : IProfileBaseAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId);

        Task<OperationResultVo<ProfileViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, ProfileViewModel viewModel);

        Task<OperationResultListVo<ProfileViewModel>> GetAll(Guid currentUserId);

        Task<OperationResultListVo<ProfileViewModel>> GetAll(Guid currentUserId, bool noCache);

        Task<ProfileViewModel> GetByUserId(Guid userId, ProfileType type);

        Task<ProfileViewModel> GetByUserId(Guid userId, ProfileType type, bool forEdit);

        Task<ProfileViewModel> Get(Guid currentUserId, Guid userId, string userHandler, ProfileType type);

        Task<ProfileViewModel> Get(Guid currentUserId, Guid userId, string userHandler, ProfileType type, bool forEdit);

        ProfileViewModel GenerateNewOne(ProfileType type);

        OperationResultVo Search(string term);

        OperationResultVo UserFollow(Guid currentUserId, Guid userId);

        OperationResultVo UserUnfollow(Guid currentUserId, Guid userId);

        #region Connections

        OperationResultVo Connect(Guid currentUserId, Guid userId, UserConnectionType connectionType);

        OperationResultVo Disconnect(Guid currentUserId, Guid userId);

        OperationResultVo Allow(Guid currentUserId, Guid userId);

        OperationResultVo Deny(Guid currentUserId, Guid userId);

        OperationResultVo GetConnectionsByUserId(Guid userId);

        void SetProfileCache(Guid key, ProfileViewModel viewModel);

        Task<ProfileViewModel> GetUserProfileWithCache(Guid userId);

        #endregion Connections
    }
}