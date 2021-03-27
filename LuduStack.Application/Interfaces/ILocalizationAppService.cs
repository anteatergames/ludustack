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

        Task<OperationResultVo> GetAllIds(Guid currentUserId);

        Task<OperationResultVo<LocalizationViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, LocalizationViewModel viewModel);

        Task<OperationResultListVo<LocalizationViewModel>> GetAll(Guid currentUserId);

        OperationResultVo GenerateNew(Guid currentUserId);

        Task<OperationResultVo> GetByUserId(Guid currentUserId, Guid userId);

        Task<OperationResultVo> GetBasicInfoById(Guid currentUserId, Guid id);

        OperationResultVo GetMyUntranslatedGames(Guid currentUserId);

        OperationResultVo GetTranslations(Guid currentUserId, Guid projectId, SupportedLanguage language);

        OperationResultVo GetTerms(Guid currentUserId, Guid projectId);

        OperationResultVo SaveEntry(Guid currentUserId, Guid projectId, bool currentUserIsOwner, bool currentUserHelped, LocalizationEntryViewModel vm);

        Task<OperationResultVo> ReadTermsSheet(Guid currentUserId, Guid projectId, IEnumerable<KeyValuePair<int, SupportedLanguage>> columns, IFormFile termsFile);

        OperationResultVo SetTerms(Guid currentUserId, Guid projectId, IEnumerable<LocalizationTermViewModel> terms);

        OperationResultVo SaveEntries(Guid currentUserId, Guid projectId, SupportedLanguage language, IEnumerable<LocalizationEntryViewModel> entries);

        Task<OperationResultVo> GetStatsById(Guid currentUserId, Guid id);

        OperationResultVo GetPercentageByGameId(Guid currentUserId, Guid gameId);

        OperationResultVo GetXml(Guid currentUserId, Guid projectId, SupportedLanguage? language, bool fillGaps);

        OperationResultVo GetContributorsFile(Guid currentUserId, Guid projectId, ExportContributorsType type);

        OperationResultVo EntryReview(Guid currentUserId, Guid projectId, Guid entryId, bool accept);
    }
}