using LuduStack.Application.ViewModels.Localization;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface ILocalizationAppService : IPermissionControl<LocalizationViewModel>
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId);

        Task<OperationResultVo<LocalizationViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, LocalizationViewModel viewModel);

        Task<OperationResultListVo<LocalizationViewModel>> GetAll(Guid currentUserId);

        OperationResultVo GenerateNew(Guid currentUserId);

        Task<OperationResultVo> GetByUserId(Guid currentUserId, Guid userId);

        Task<OperationResultVo> GetBasicInfoById(Guid currentUserId, Guid id);

        Task<OperationResultVo> GetMyUntranslatedGames(Guid currentUserId);

        Task<OperationResultVo> GetTranslations(Guid currentUserId, Guid projectId, LocalizationLanguage language);

        Task<OperationResultVo> GetTerms(Guid currentUserId, Guid projectId);

        Task<OperationResultVo> SaveEntry(Guid currentUserId, Guid projectId, bool currentUserIsOwner, bool currentUserHelped, LocalizationEntryViewModel vm);

        Task<OperationResultVo> ReadTermsSheet(Guid currentUserId, Guid projectId, IEnumerable<KeyValuePair<int, LocalizationLanguage>> columns, IFormFile termsFile);

        OperationResultVo SetTerms(Guid currentUserId, Guid projectId, IEnumerable<LocalizationTermViewModel> terms);

        OperationResultVo SaveEntries(Guid currentUserId, Guid projectId, LocalizationLanguage language, IEnumerable<LocalizationEntryViewModel> entries);

        Task<OperationResultVo> GetStatsById(Guid currentUserId, Guid id);

        OperationResultVo GetPercentageByGameId(Guid currentUserId, Guid gameId);

        OperationResultVo GetXml(Guid currentUserId, Guid projectId, LocalizationLanguage? language, bool fillGaps);

        Task<OperationResultVo> GetContributorsFile(Guid currentUserId, Guid projectId, ExportContributorsType type);

        OperationResultVo EntryReview(Guid currentUserId, Guid projectId, Guid entryId, bool accept);
    }
}