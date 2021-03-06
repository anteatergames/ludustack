﻿using LuduStack.Application.ViewModels.GameJam;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IGameJamAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<IEnumerable<SelectListItemVo>> GetSelectList(Guid userId);

        Task<OperationResultListVo<GameJamViewModel>> GetAll(Guid currentUserId, bool currentUserIsAdmin);

        Task<OperationResultListVo<GameJamViewModel>> GetByUserId(Guid userId, bool currentUserIsAdmin);

        Task<OperationResultVo<GameJamViewModel>> GetForDetails(Guid currentUserId, bool currentUserIsAdmin, Guid id, string handler);

        Task<OperationResultVo<GameJamViewModel>> GetForEdit(Guid currentUserId, bool currentUserIsAdmin, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, GameJamViewModel viewModel);

        Task<OperationResultVo<GameJamViewModel>> GenerateNew(Guid currentUserId);

        Task<OperationResultVo> ValidateHandler(Guid currentUserId, string handler, Guid id);

        Task<OperationResultVo> Join(Guid currentUserId, Guid jamId);

        Task<OperationResultVo<GameJamEntryViewModel>> GetEntry(Guid currentUserId, bool currentUserIsAdmin, string jamHandler);
        Task<OperationResultVo> SubmitGame(Guid currentUserId, string jamHandler, Guid gameId);
    }
}