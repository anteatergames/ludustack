using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class TranslationDomainService : ILocalizationDomainService
    {
        protected readonly ILocalizationRepository localizationRepository;

        public TranslationDomainService(ILocalizationRepository localizationRepository)
        {
            this.localizationRepository = localizationRepository;
        }

        public Localization GenerateNewProject(Guid userId)
        {
            Localization model = new Localization
            {
                UserId = userId
            };

            return model;
        }

        public IEnumerable<Guid> GetTranslatedGamesByUserId(Guid userId)
        {
            IEnumerable<Guid> gameIds = localizationRepository.GetTranslatedGamesByUserId(userId);

            return gameIds;
        }

        public IEnumerable<LocalizationEntry> GetEntries(Guid projectId, LocalizationLanguage language)
        {
            List<LocalizationEntry> entries = localizationRepository.GetEntries(projectId, language).ToList();

            return entries;
        }

        public DomainActionPerformed AddEntry(Guid projectId, LocalizationEntry entry)
        {
            IQueryable<LocalizationEntry> existing = localizationRepository.GetEntries(projectId, entry.Language, entry.TermId);
            bool oneIsMine = existing.Any(x => x.UserId == entry.UserId);

            if (oneIsMine)
            {
                entry.Id = existing.First(x => x.UserId == entry.UserId).Id;
                localizationRepository.UpdateEntry(projectId, entry);

                return DomainActionPerformed.Update;
            }
            else
            {
                entry.Value = entry.Value.Trim();

                bool existsWithSameValue = existing.Any(x => x.Value.Equals(entry.Value));
                if (!existing.Any() || !existsWithSameValue)
                {
                    localizationRepository.AddEntry(projectId, entry);

                    return DomainActionPerformed.Create;
                }
            }

            return DomainActionPerformed.None;
        }

        public void SaveEntries(Guid projectId, IEnumerable<LocalizationEntry> entries)
        {
            List<LocalizationEntry> existingEntrys = localizationRepository.GetEntries(projectId).ToList();

            foreach (LocalizationEntry entry in entries)
            {
                LocalizationEntry existing = existingEntrys.FirstOrDefault(x => x.TermId == entry.TermId && x.UserId == entry.UserId && x.Language == entry.Language);
                if (existing == null)
                {
                    entry.CreateDate = DateTime.Now;
                    localizationRepository.AddEntry(projectId, entry);
                }
                else
                {
                    existing.Value = entry.Value;
                    existing.LastUpdateDate = DateTime.Now;

                    localizationRepository.UpdateEntry(projectId, existing);
                }
            }
        }

        public IEnumerable<LocalizationTerm> GetTerms(Guid projectId)
        {
            List<LocalizationTerm> terms = localizationRepository.GetTerms(projectId).ToList();

            return terms;
        }

        public LocalizationStatsVo GetPercentageByGameId(Guid gameId)
        {
            LocalizationStatsVo model = localizationRepository.GetStatsByGameId(gameId);

            if (model == null)
            {
                return null;
            }

            int distinctEntriesCount = model.Entries.Select(x => new { x.TermId, x.Language }).Distinct().Count();
            int languageCount = model.Entries.Select(x => x.Language).Distinct().Count();

            double percentage = CalculatePercentage(model.TermCount, distinctEntriesCount, languageCount);
            model.LocalizationPercentage = percentage;

            return model;
        }

        public void SetTerms(Guid projectId, IEnumerable<LocalizationTerm> terms)
        {
            List<LocalizationTerm> existingTerms = localizationRepository.GetTerms(projectId).ToList();

            foreach (LocalizationTerm term in terms)
            {
                LocalizationTerm existing = existingTerms.FirstOrDefault(x => x.Id == term.Id);
                if (existing == null)
                {
                    localizationRepository.AddTerm(projectId, term);
                }
                else
                {
                    existing.Key = term.Key;
                    existing.Value = term.Value;
                    existing.Obs = term.Obs;
                    existing.LastUpdateDate = DateTime.Now;

                    localizationRepository.UpdateTerm(projectId, existing);
                }
            }

            IEnumerable<LocalizationTerm> deleteTerms = existingTerms.Where(x => !terms.Contains(x));

            if (deleteTerms.Any())
            {
                List<LocalizationEntry> existingEntries = localizationRepository.GetEntries(projectId).ToList();
                foreach (LocalizationTerm term in deleteTerms)
                {
                    IEnumerable<LocalizationEntry> entries = existingEntries.Where(x => x.TermId == term.Id);
                    localizationRepository.RemoveTerm(projectId, term.Id);

                    foreach (LocalizationEntry entry in entries)
                    {
                        localizationRepository.RemoveEntry(projectId, entry.Id);
                    }
                }
            }
        }

        public Localization GetBasicInfoById(Guid id)
        {
            Localization obj = localizationRepository.GetBasicInfoById(id);

            return obj;
        }

        public void AcceptEntry(Guid projectId, Guid entryId)
        {
            LocalizationEntry entry = localizationRepository.GetEntry(projectId, entryId);
            if (entry != null)
            {
                entry.Accepted = true;
                localizationRepository.UpdateEntry(projectId, entry);
            }
        }

        public void RejectEntry(Guid projectId, Guid entryId)
        {
            LocalizationEntry entry = localizationRepository.GetEntry(projectId, entryId);
            if (entry != null)
            {
                entry.Accepted = false;
                localizationRepository.UpdateEntry(projectId, entry);
            }
        }

        public async Task<List<InMemoryFileVo>> GetXmlById(Guid projectId, bool fillGaps)
        {
            List<InMemoryFileVo> xmlTexts = new List<InMemoryFileVo>();

            Localization project = await localizationRepository.GetById(projectId);

            List<LocalizationLanguage> languages = project.Entries.Select(x => x.Language).Distinct().ToList();
            languages.Add(project.PrimaryLanguage);

            foreach (LocalizationLanguage language in languages)
            {
                string xmlText = GenerateLanguageXml(project, language, fillGaps);

                xmlTexts.Add(new InMemoryFileVo
                {
                    FileName = string.Format("{0}.xml", language.ToString().ToLower()),
                    Contents = Encoding.UTF8.GetBytes(xmlText)
                });
            }

            return xmlTexts;
        }

        public async Task<InMemoryFileVo> GetXmlById(Guid projectId, LocalizationLanguage language, bool fillGaps)
        {
            Localization project = await localizationRepository.GetById(projectId);

            string xmlText = GenerateLanguageXml(project, language, fillGaps);

            return new InMemoryFileVo
            {
                FileName = string.Format("{0}.xml", language.ToString().ToLower()),
                Contents = Encoding.UTF8.GetBytes(xmlText)
            };
        }

        public Task<List<Guid>> GetContributors(Guid projectId, ExportContributorsType type)
        {
            List<Guid> contributorsIds = localizationRepository.GetEntries(projectId).Select(x => x.UserId).Distinct().ToList();

            return Task.FromResult(contributorsIds);
        }

        public double CalculatePercentage(int totalTerms, int translatedCount, int languageCount)
        {
            int totalTranslationsTarget = languageCount * totalTerms;

            double percentage = (100 * translatedCount) / (double)(totalTranslationsTarget == 0 ? 1 : totalTranslationsTarget);

            return percentage > 100 ? 100 : percentage;
        }

        private static string GenerateLanguageXml(Localization project, LocalizationLanguage language, bool fillGaps)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<resources>");
            sb.AppendLine();

            sb.AppendLine(string.Format("<string id=\"lang_name\">{0}</string>", language.ToDisplayName()));
            sb.AppendLine(string.Format("<string id=\"lang_index\">{0}</string>", (int)language));
            sb.AppendLine();

            for (int i = 0; i < project.Terms.Count; i++)
            {
                string langValue = null;
                LocalizationTerm term = project.Terms.ElementAt(i);
                IOrderedEnumerable<LocalizationEntry> entries = project.Entries.Where(x => x.TermId == term.Id && x.Language == language).OrderBy(x => x.Accepted);

                if (entries.Any())
                {
                    LocalizationEntry lastAccepted = entries.LastOrDefault(x => !x.Accepted.HasValue || x.Accepted == true);
                    if (lastAccepted != null)
                    {
                        langValue = lastAccepted.Value;
                    }
                }
                else
                {
                    if (fillGaps)
                    {
                        langValue = term.Value;
                    }
                }

                if (!string.IsNullOrWhiteSpace(langValue))
                {
                    sb.AppendLine(string.Format("<string id=\"{0}\">{1}</string>", term.Key, langValue));
                }
            }

            sb.AppendLine();
            sb.AppendLine("</resources>");

            return sb.ToString();
        }
    }
}