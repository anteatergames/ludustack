using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface ILocalizationRepository : IRepository<Localization>
    {
        Localization GetBasicInfoById(Guid id);

        int CountTerms(Func<LocalizationTerm, bool> where);

        IQueryable<LocalizationTerm> GetTerms(Guid translationProjectId);

        Task<bool> AddTerm(Guid translationProjectId, LocalizationTerm term);

        Task<bool> RemoveTerm(Guid translationProjectId, Guid termId);

        Task<bool> UpdateTerm(Guid translationProjectId, LocalizationTerm term);

        int CountEntries(Func<LocalizationEntry, bool> where);

        IQueryable<LocalizationEntry> GetEntries(Guid translationProjectId);

        IQueryable<LocalizationEntry> GetEntries(Guid projectId, LocalizationLanguage language);

        IQueryable<LocalizationEntry> GetEntries(Guid projectId, LocalizationLanguage language, Guid termId);

        Task<bool> AddEntry(Guid translationProjectId, LocalizationEntry entry);

        IEnumerable<Guid> GetTranslatedGamesByUserId(Guid userId);

        Task<bool> RemoveEntry(Guid translationProjectId, Guid entryId);

        void UpdateEntry(Guid translationProjectId, LocalizationEntry entry);

        LocalizationEntry GetEntry(Guid projectId, Guid entryId);

        LocalizationStatsVo GetStatsByGameId(Guid gameId);
    }
}