using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.Localization;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Game;
using LuduStack.Domain.Messaging.Queries.Localization;
using LuduStack.Domain.Messaging.Queries.UserProfile;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class LocalizationAppService : ProfileBaseAppService, ILocalizationAppService
    {
        private readonly ILocalizationDomainService translationDomainService;
        private readonly IGamificationDomainService gamificationDomainService;

        public LocalizationAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon
            , ILocalizationDomainService translationDomainService
            , IGamificationDomainService gamificationDomainService) : base(profileBaseAppServiceCommon)
        {
            this.translationDomainService = translationDomainService;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountLocalizationQuery, int>(new CountLocalizationQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<LocalizationViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<Localization> allModels = await mediator.Query<GetLocalizationQuery, IEnumerable<Localization>>(new GetLocalizationQuery());

                List<LocalizationViewModel> vms = mapper.Map<IEnumerable<Localization>, IEnumerable<LocalizationViewModel>>(allModels).ToList();

                foreach (LocalizationViewModel item in vms)
                {
                    item.TermCount = item.Terms.Count;

                    await SetGameViewModel(item.Game.Id, item);
                    item.Game.Title = string.Empty;

                    SetPermissions(currentUserId, item);

                    int totalTermCount = item.Terms.Count;
                    int distinctEntriesCount = item.Entries.Select(x => new { x.TermId, x.Language }).Distinct().Count();
                    int languageCount = item.Entries.Select(x => x.Language).Distinct().Count();

                    item.TranslationPercentage = translationDomainService.CalculatePercentage(totalTermCount, distinctEntriesCount, languageCount);
                }

                vms = vms.OrderByDescending(x => x.CreateDate).ToList();

                return new OperationResultListVo<LocalizationViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<LocalizationViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = await mediator.Query<GetLocalizationIdsQuery, IEnumerable<Guid>>(new GetLocalizationIdsQuery());

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetByUserId(Guid currentUserId, Guid userId)
        {
            try
            {
                IEnumerable<Localization> allModels = await mediator.Query<GetLocalizationByUserIdQuery, IEnumerable<Localization>>(new GetLocalizationByUserIdQuery(userId));

                List<LocalizationViewModel> vms = mapper.Map<IEnumerable<Localization>, IEnumerable<LocalizationViewModel>>(allModels).ToList();

                foreach (LocalizationViewModel item in vms)
                {
                    item.TermCount = item.Terms.Count;

                    GameViewModel game = await GetGameWithCache(item.Game.Id);
                    item.Game.Title = game.Title;

                    SetPermissions(userId, item);
                }

                vms = vms.OrderByDescending(x => x.CreateDate).ToList();

                return new OperationResultListVo<LocalizationViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<LocalizationViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<LocalizationViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                Localization model = await mediator.Query<GetLocalizationByIdQuery, Localization>(new GetLocalizationByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<LocalizationViewModel>("Translation Project not found!");
                }

                model.Terms = model.Terms.Where(x => !string.IsNullOrWhiteSpace(x.Key)).ToList();

                UserProfileEssentialVo profile = await mediator.Query<GetBasicUserProfileDataByUserIdQuery, UserProfileEssentialVo>(new GetBasicUserProfileDataByUserIdQuery(model.UserId));

                LocalizationViewModel vm = mapper.Map<LocalizationViewModel>(model);

                await SetGameViewModel(model.GameId, vm);

                SetAuthorDetails(currentUserId, vm, profile);

                SetPermissions(currentUserId, vm);

                vm.CurrentUserHelped = model.Entries.Any(x => x.UserId == currentUserId);
                vm.CurrentUserIsOwner = model.UserId == currentUserId;

                return new OperationResultVo<LocalizationViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<LocalizationViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetBasicInfoById(Guid currentUserId, Guid id)
        {
            try
            {
                Localization model = translationDomainService.GetBasicInfoById(id);

                if (model == null)
                {
                    return new OperationResultVo<LocalizationViewModel>("Translation Project not found!");
                }

                LocalizationViewModel vm = mapper.Map<LocalizationViewModel>(model);

                await SetGameViewModel(model.GameId, vm);

                SetPermissions(currentUserId, vm);

                return new OperationResultVo<LocalizationViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<LocalizationViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteLocalizationCommand(currentUserId, id));

                return new OperationResultVo(true, "That Translation Project is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, LocalizationViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                Localization model;

                Localization existing = await mediator.Query<GetLocalizationByIdQuery, Localization>(new GetLocalizationByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<Localization>(viewModel);
                }

                foreach (LocalizationTerm term in model.Terms)
                {
                    if (term.UserId == Guid.Empty)
                    {
                        term.UserId = currentUserId;
                    }
                }

                CommandResult result = await mediator.SendCommand(new SaveLocalizationCommand(currentUserId, model));

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

        public async Task<OperationResultVo> GetMyUntranslatedGames(Guid currentUserId)
        {
            try
            {
                IEnumerable<Game> myGames = await mediator.Query<GetGameByUserIdQuery, IEnumerable<Game>>(new GetGameByUserIdQuery(currentUserId));

                IEnumerable<Guid> myTranslatedGames = translationDomainService.GetTranslatedGamesByUserId(currentUserId);

                IEnumerable<Game> availableGames = myGames.Where(x => !myTranslatedGames.Contains(x.Id));

                List<SelectListItemVo> vms = mapper.Map<IEnumerable<Game>, IEnumerable<SelectListItemVo>>(availableGames).ToList();

                return new OperationResultListVo<SelectListItemVo>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetTranslations(Guid currentUserId, Guid projectId, LocalizationLanguage language)
        {
            try
            {
                IEnumerable<LocalizationEntry> entries = translationDomainService.GetEntries(projectId, language);

                IEnumerable<Guid> userIdList = entries.Select(x => x.UserId);

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(userIdList));

                List<LocalizationEntryViewModel> vms = mapper.Map<IEnumerable<LocalizationEntry>, IEnumerable<LocalizationEntryViewModel>>(entries).ToList();

                foreach (LocalizationEntryViewModel entry in vms)
                {
                    UserProfileEssentialVo profile = userProfiles.FirstOrDefault(x => x.UserId == entry.UserId);
                    entry.UserHandler = profile.Handler;
                    entry.AuthorName = profile.Name;
                    entry.AuthorPicture = UrlFormatter.ProfileImage(entry.UserId);
                }

                return new OperationResultListVo<LocalizationEntryViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> SaveEntry(Guid currentUserId, Guid projectId, bool currentUserIsOwner, bool currentUserHelped, LocalizationEntryViewModel vm)
        {
            int pointsEarned = 0;

            try
            {
                LocalizationEntry entry = mapper.Map<LocalizationEntry>(vm);

                DomainActionPerformed actionPerformed = translationDomainService.AddEntry(projectId, entry);

                if (actionPerformed == DomainActionPerformed.None)
                {
                    return new OperationResultVo("Another user already sent that translation!");
                }

                if (!currentUserIsOwner && actionPerformed == DomainActionPerformed.Create)
                {
                    pointsEarned += gamificationDomainService.ProcessAction(currentUserId, PlatformAction.LocalizationHelp);
                }

                await unitOfWork.Commit();
                vm.Id = entry.Id;

                if (!currentUserHelped && !currentUserIsOwner)
                {
                    await mediator.SendCommand(new SaveUserBadgeCommand(currentUserId, BadgeType.Babel, projectId));
                }

                UserProfileEssentialVo profile = await GetCachedEssentialProfileByUserId(entry.UserId);
                vm.AuthorName = profile.Name;

                return new OperationResultVo<LocalizationEntryViewModel>(vm, pointsEarned, "Translation saved!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo SaveEntries(Guid currentUserId, Guid projectId, LocalizationLanguage language, IEnumerable<LocalizationEntryViewModel> entries)
        {
            try
            {
                List<LocalizationEntryViewModel> entriesUpdated = entries.ToList();

                foreach (LocalizationEntryViewModel item in entriesUpdated)
                {
                    item.UserId = currentUserId;
                    item.Language = language;
                }

                IEnumerable<LocalizationEntry> vms = mapper.Map<IEnumerable<LocalizationEntryViewModel>, IEnumerable<LocalizationEntry>>(entries);

                translationDomainService.SaveEntries(projectId, vms);

                unitOfWork.Commit();

                return new OperationResultVo(true, "Translations saved!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetTerms(Guid currentUserId, Guid projectId)
        {
            try
            {
                Localization basicData = translationDomainService.GetBasicInfoById(projectId);

                IEnumerable<LocalizationTerm> entries = translationDomainService.GetTerms(projectId);

                List<LocalizationTermViewModel> vms = mapper.Map<IEnumerable<LocalizationTerm>, IEnumerable<LocalizationTermViewModel>>(entries).ToList();

                foreach (LocalizationTermViewModel entry in vms)
                {
                    UserProfileEssentialVo profile = await GetCachedEssentialProfileByUserId(entry.UserId);
                    entry.AuthorName = profile.Name;
                    entry.AuthorPicture = UrlFormatter.ProfileImage(entry.UserId);
                }

                LocalizationViewModel projectVm = new LocalizationViewModel
                {
                    Id = projectId,
                    PrimaryLanguage = basicData.PrimaryLanguage,
                    Terms = vms
                };

                return new OperationResultVo<LocalizationViewModel>(projectVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo SetTerms(Guid currentUserId, Guid projectId, IEnumerable<LocalizationTermViewModel> terms)
        {
            try
            {
                List<LocalizationTerm> vms = mapper.Map<IEnumerable<LocalizationTermViewModel>, IEnumerable<LocalizationTerm>>(terms).ToList();

                foreach (LocalizationTerm term in vms)
                {
                    term.UserId = currentUserId;
                }

                translationDomainService.SetTerms(projectId, vms);

                unitOfWork.Commit();

                return new OperationResultVo(true, "Terms Updated!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetStatsById(Guid currentUserId, Guid id)
        {
            try
            {
                Localization model = await mediator.Query<GetLocalizationByIdQuery, Localization>(new GetLocalizationByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo("Translation Project not found!");
                }

                List<Guid> userIdList = model.Entries.GroupBy(x => x.UserId).Select(x => x.Key).ToList();
                userIdList.Add(model.UserId);

                IEnumerable<UserProfileEssentialVo> profiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(userIdList));

                TranslationStatsViewModel vm = mapper.Map<TranslationStatsViewModel>(model);

                vm.TermCount = model.Terms.Count;

                IEnumerable<IGrouping<LocalizationLanguage, LocalizationEntry>> languages = model.Entries.GroupBy(x => x.Language);

                foreach (IGrouping<LocalizationLanguage, LocalizationEntry> language in languages)
                {
                    TranslationStatsLanguageViewModel languageEntry = new TranslationStatsLanguageViewModel
                    {
                        Language = language.Key,
                        EntryCount = language.Select(x => x.TermId).Distinct().Count(),
                        Percentage = translationDomainService.CalculatePercentage(vm.TermCount, language.Count(), 1)
                    };

                    vm.Languages.Add(languageEntry);
                }

                SetAuthorDetails(currentUserId, vm, profiles);

                SetPercentage(model, vm);

                await SetContributors(model, vm, profiles);

                await SetGameViewModel(model.GameId, vm);

                SetPermissions(currentUserId, vm);

                return new OperationResultVo<TranslationStatsViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetPercentageByGameId(Guid currentUserId, Guid gameId)
        {
            try
            {
                LocalizationStatsVo model = translationDomainService.GetPercentageByGameId(gameId);

                if (model == null)
                {
                    return new OperationResultVo("Localization Project not found!");
                }

                return new OperationResultVo<LocalizationStatsVo>(model);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetXml(Guid currentUserId, Guid projectId, LocalizationLanguage? language, bool fillGaps)
        {
            try
            {
                if (language.HasValue)
                {
                    Task<InMemoryFileVo> task = translationDomainService.GetXmlById(projectId, language.Value, fillGaps);

                    task.Wait();

                    return new OperationResultVo<InMemoryFileVo>(task.Result);
                }
                else
                {
                    List<InMemoryFileVo> list = new List<InMemoryFileVo>();

                    Task<List<InMemoryFileVo>> task = translationDomainService.GetXmlById(projectId, fillGaps);

                    task.Wait();

                    foreach (InMemoryFileVo item in task.Result)
                    {
                        list.Add(item);
                    }

                    return new OperationResultVo<List<InMemoryFileVo>>(list);
                }
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetContributorsFile(Guid currentUserId, Guid projectId, ExportContributorsType type)
        {
            try
            {
                Task<List<Guid>> task = translationDomainService.GetContributors(projectId, type);

                task.Wait();

                List<Guid> contributorsIds = task.Result;

                List<KeyValuePair<Guid, string>> dict = new List<KeyValuePair<Guid, string>>();

                foreach (Guid contributorId in contributorsIds)
                {
                    UserProfileEssentialVo profile = await GetCachedEssentialProfileByUserId(contributorId);
                    dict.Add(new KeyValuePair<Guid, string>(contributorId, profile.Name));
                }

                return new OperationResultVo<List<KeyValuePair<Guid, string>>>(dict);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> ReadTermsSheet(Guid currentUserId, Guid projectId, IEnumerable<KeyValuePair<int, LocalizationLanguage>> columns, IFormFile termsFile)
        {
            try
            {
                List<LocalizationTerm> loadedTerms = new List<LocalizationTerm>();
                List<LocalizationEntry> loadedEntries = new List<LocalizationEntry>();

                if (termsFile != null && termsFile.Length > 0)
                {
                    DataTable dataTable = await LoadExcel(termsFile);

                    FillTerms(dataTable, loadedTerms);

                    Localization model = await mediator.Query<GetLocalizationByIdQuery, Localization>(new GetLocalizationByIdQuery(projectId));

                    if (model != null)
                    {
                        bool termsUpdated = false;
                        foreach (LocalizationTerm loadedTerm in loadedTerms)
                        {
                            termsUpdated = AddOrUpdateTerm(currentUserId, model, loadedTerm);
                        }

                        if (termsUpdated)
                        {
                            CommandResult result = await mediator.SendCommand(new SaveLocalizationCommand(currentUserId, model));

                            if (!result.Validation.IsValid)
                            {
                                string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                                return new OperationResultVo(false, message);
                            }

                            FillEntries(dataTable, model.Terms, loadedEntries, columns);

                            await UpdateEntries(currentUserId, loadedEntries, model);
                        }
                    }
                }

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private async Task UpdateEntries(Guid currentUserId, List<LocalizationEntry> loadedEntries, Localization model)
        {
            bool entriesUpdated = false;
            foreach (LocalizationEntry loadedEntry in loadedEntries)
            {
                entriesUpdated = AddOrUpdateEntry(currentUserId, model, loadedEntry);
            }

            if (entriesUpdated)
            {
                await mediator.SendCommand(new SaveLocalizationCommand(currentUserId, model));
            }
        }

        public OperationResultVo GenerateNew(Guid currentUserId)
        {
            try
            {
                Localization model = translationDomainService.GenerateNewProject(currentUserId);

                LocalizationViewModel newVm = mapper.Map<LocalizationViewModel>(model);

                return new OperationResultVo<LocalizationViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo EntryReview(Guid currentUserId, Guid projectId, Guid entryId, bool accept)
        {
            try
            {
                if (accept)
                {
                    translationDomainService.AcceptEntry(projectId, entryId);
                }
                else
                {
                    translationDomainService.RejectEntry(projectId, entryId);
                }

                unitOfWork.Commit();

                return new OperationResultVo(true, "Translation Updated!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public void SetPermissions(Guid currentUserId, LocalizationViewModel vm)
        {
            SetBasePermissions(currentUserId, vm);
        }

        private Task SetContributors(Localization model, TranslationStatsViewModel vm, IEnumerable<UserProfileEssentialVo> userProfiles)
        {
            IEnumerable<IGrouping<Guid, LocalizationEntry>> contributors = model.Entries.GroupBy(x => x.UserId);

            foreach (IGrouping<Guid, LocalizationEntry> contributorGroup in contributors)
            {
                ContributorViewModel contributor = new ContributorViewModel();
                UserProfileEssentialVo profile = userProfiles.FirstOrDefault(x => x.UserId == contributorGroup.Key);
                contributor.UserId = contributorGroup.Key;
                contributor.UserHandler = profile.Handler;
                contributor.AuthorName = profile.Name;
                contributor.AuthorPicture = UrlFormatter.ProfileImage(contributorGroup.Key);
                contributor.EntryCount = contributorGroup.Count();
                vm.Contributors.Add(contributor);
            }

            vm.Contributors = vm.Contributors.OrderByDescending(x => x.EntryCount).ToList();

            return Task.CompletedTask;
        }

        private void SetPercentage(Localization model, TranslationStatsViewModel vm)
        {
            int totalTermCount = model.Terms.Count;
            int distinctEntriesCount = model.Entries.Select(x => new { x.TermId, x.Language }).Distinct().Count();
            int languageCount = model.Entries.Select(x => x.Language).Distinct().Count();

            vm.TranslationPercentage = translationDomainService.CalculatePercentage(totalTermCount, distinctEntriesCount, languageCount);
        }

        private static string SetFeaturedImage(Guid userId, string thumbnailUrl, ImageRenderType imageType)
        {
            if (string.IsNullOrWhiteSpace(thumbnailUrl) || Constants.DefaultGameThumbnail.NoExtension().Contains(thumbnailUrl.NoExtension()))
            {
                return Constants.DefaultGameThumbnail;
            }
            else
            {
                switch (imageType)
                {
                    case ImageRenderType.LowQuality:
                        return UrlFormatter.Image(userId, ImageType.GameThumbnail, thumbnailUrl, 278, 10);

                    case ImageRenderType.Responsive:
                        return UrlFormatter.Image(userId, ImageType.GameThumbnail, thumbnailUrl, true, 0, 0);

                    case ImageRenderType.Full:
                    default:
                        return UrlFormatter.Image(userId, ImageType.GameThumbnail, thumbnailUrl, 278);
                }
            }
        }

        private async Task SetGameViewModel(Guid gameId, LocalizationViewModel vm)
        {
            GameViewModel game = await GetGameWithCache(gameId);
            if (game != null)
            {
                vm.Game.Title = game.Title;

                vm.Game.ThumbnailUrl = SetFeaturedImage(game.UserId, game?.ThumbnailUrl, ImageRenderType.Full);
                vm.Game.ThumbnailResponsive = SetFeaturedImage(game.UserId, game?.ThumbnailUrl, ImageRenderType.Responsive);
                vm.Game.ThumbnailLquip = SetFeaturedImage(game.UserId, game?.ThumbnailUrl, ImageRenderType.LowQuality);
            }
        }

        private async Task<DataTable> LoadExcel(IFormFile termsFile)
        {
            DataTable dtTable = new DataTable();
            ISheet sheet;

            using (MemoryStream stream = new MemoryStream())
            {
                await termsFile.CopyToAsync(stream);
                stream.Position = 0;

                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                sheet = xssWorkbook.GetSheetAt(0);

                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                AddColumns(dtTable, headerRow, cellCount);

                FillDataTable(dtTable, sheet, cellCount);
            }

            return dtTable;
        }

        private static void AddColumns(DataTable dtTable, IRow headerRow, int cellCount)
        {
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                if (cell == null || string.IsNullOrWhiteSpace(cell.ToString()))
                {
                    continue;
                }

                {
                    dtTable.Columns.Add(cell.ToString());
                }
            }
        }

        private static void FillDataTable(DataTable dtTable, ISheet sheet, int cellCount)
        {
            List<string> rowList = new List<string>();

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                ICell firstCell = row.GetCell(0);
                ICell secondCell = row.GetCell(1);

                if (row == null || row.Cells.All(d => d.CellType == CellType.Blank))
                {
                    continue;
                }

                if (firstCell != null && secondCell != null && !string.IsNullOrWhiteSpace(firstCell.ToString()) && !string.IsNullOrWhiteSpace(secondCell.ToString()))
                {
                    FillRows(dtTable, cellCount, rowList, row);

                    rowList.Clear();
                }
            }
        }

        private static void FillRows(DataTable dtTable, int cellCount, List<string> rowList, IRow row)
        {
            for (int j = row.FirstCellNum; j < cellCount; j++)
            {
                if (row.GetCell(j) != null && !string.IsNullOrEmpty(row.GetCell(j).ToString()) && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                {
                    rowList.Add(row.GetCell(j).ToString());
                }
            }

            if (rowList.Count > 0)
            {
                dtTable.Rows.Add(rowList.ToArray());
            }
        }

        private static void FillTerms(DataTable dataTable, List<LocalizationTerm> terms)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                object term = row.ItemArray[0];
                object termValue = row.ItemArray[1];

                if (term == null || string.IsNullOrWhiteSpace(term.ToString()) || termValue == null || string.IsNullOrWhiteSpace(termValue.ToString()))
                {
                    continue;
                }

                bool termExists = terms.Any(x => x.Key.Equals(term));
                if (!termExists)
                {
                    LocalizationTerm newTerm = new LocalizationTerm
                    {
                        Key = term.ToString().Trim().Replace("\n", "_"),
                        Value = termValue.ToString().Trim()
                    };

                    newTerm.Key = SanitizeKey(newTerm.Key);

                    terms.Add(newTerm);
                }
            }
        }

        private static string SanitizeKey(string key)
        {
            if (key.EndsWith("\n") || key.StartsWith("\n"))
            {
                key = key.Replace("\n", string.Empty);
            }
            if (key.EndsWith("_") || key.StartsWith("_"))
            {
                key = key.Replace("_", string.Empty);
            }
            if (key.EndsWith(".") || key.StartsWith("."))
            {
                key = key.Replace(".", string.Empty);
            }

            key = key.Replace("\n", "_");
            key = key.Replace("__", "_");
            key = key.Replace("__", "_");
            key = key.Replace("__", "_");

            return key;
        }

        private static void FillEntries(DataTable dataTable, List<LocalizationTerm> terms, List<LocalizationEntry> entries, IEnumerable<KeyValuePair<int, LocalizationLanguage>> columns)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                object term = row.ItemArray[0];

                foreach (KeyValuePair<int, LocalizationLanguage> col in columns)
                {
                    int colNumber = col.Key - 1;

                    if (col.Key > row.ItemArray.Length)
                    {
                        break;
                    }

                    object translation = row.ItemArray[colNumber];

                    if (term == null || string.IsNullOrWhiteSpace(term.ToString()) || translation == null || string.IsNullOrWhiteSpace(translation.ToString()))
                    {
                        continue;
                    }

                    LocalizationTerm existingTerm = terms.FirstOrDefault(x => SanitizeKey(x.Key).Equals(SanitizeKey(term.ToString())));
                    if (existingTerm != null)
                    {
                        LocalizationEntry newEntry = new LocalizationEntry
                        {
                            TermId = existingTerm.Id,
                            Value = translation.ToString().Trim(),
                            Language = col.Value
                        };

                        entries.Add(newEntry);
                    }
                }
            }
        }

        private static bool AddOrUpdateTerm(Guid currentUserId, Localization model, LocalizationTerm loadedTerm)
        {
            bool termsUpdated;
            if (loadedTerm.UserId == Guid.Empty)
            {
                loadedTerm.UserId = currentUserId;
            }

            LocalizationTerm modelTerm = model.Terms.FirstOrDefault(x => x.Key.Equals(loadedTerm.Key.Replace("\n", string.Empty)));
            if (modelTerm == null)
            {
                model.Terms.Add(loadedTerm);
                termsUpdated = true;
            }
            else
            {
                loadedTerm.Id = modelTerm.Id;
                loadedTerm.CreateDate = modelTerm.CreateDate;
                loadedTerm.UserId = modelTerm.UserId;
                modelTerm.Value = loadedTerm.Value;
                termsUpdated = true;
            }

            return termsUpdated;
        }

        private static bool AddOrUpdateEntry(Guid currentUserId, Localization model, LocalizationEntry loadedEntry)
        {
            bool entriesUpdated;
            if (loadedEntry.UserId == Guid.Empty)
            {
                loadedEntry.UserId = currentUserId;
            }

            LocalizationEntry modelEntry = model.Entries.FirstOrDefault(x => x.TermId == loadedEntry.TermId && x.UserId == currentUserId && x.Language == loadedEntry.Language);
            if (modelEntry == null)
            {
                model.Entries.Add(loadedEntry);
                entriesUpdated = true;
            }
            else
            {
                loadedEntry.Id = modelEntry.Id;
                loadedEntry.CreateDate = modelEntry.CreateDate;
                loadedEntry.UserId = modelEntry.UserId;
                modelEntry.Value = loadedEntry.Value;
                entriesUpdated = true;
            }

            return entriesUpdated;
        }
    }
}