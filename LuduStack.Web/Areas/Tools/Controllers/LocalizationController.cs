﻿using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Localization;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Tools.Controllers.Base;
using LuduStack.Web.Extensions;
using LuduStack.Web.Extensions.ViewModelExtensions;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Tools.Controllers
{
    public class LocalizationController : ToolsBaseController
    {
        private readonly ILocalizationAppService translationAppService;

        public LocalizationController(ILocalizationAppService translationAppService)
        {
            this.translationAppService = translationAppService;
        }

        public async Task<IActionResult> Index()
        {
            OperationResultVo gamesResult = await translationAppService.GetMyUntranslatedGames(CurrentUserId);
            if (gamesResult.Success)
            {
                OperationResultListVo<SelectListItemVo> castResultGames = gamesResult as OperationResultListVo<SelectListItemVo>;

                IEnumerable<SelectListItemVo> games = castResultGames.Value;
                ViewData["CanRequest"] = games.Any();
            }
            else
            {
                ViewData["CanRequest"] = false;
            }

            return View();
        }

        [Route("tools/localization/list")]
        public async Task<PartialViewResult> List()
        {
            IEnumerable<LocalizationViewModel> model;
            OperationResultVo serviceResult;

            serviceResult = await translationAppService.GetAll(CurrentUserId);

            if (serviceResult != null && serviceResult.Success)
            {
                OperationResultListVo<LocalizationViewModel> castResult = serviceResult as OperationResultListVo<LocalizationViewModel>;

                model = castResult.Value;
            }
            else
            {
                model = new List<LocalizationViewModel>();
            }

            foreach (LocalizationViewModel item in model)
            {
                SetLocalization(item);
            }

            ViewData["ListDescription"] = SharedLocalizer["These are translation projects available to help."].ToString();

            return PartialView("_List", model);
        }

        [Route("tools/localization/listmine")]
        public async Task<PartialViewResult> ListMine()
        {
            IEnumerable<LocalizationViewModel> model;
            OperationResultVo serviceResult;

            serviceResult = await translationAppService.GetByUserId(CurrentUserId, CurrentUserId);

            if (serviceResult != null && serviceResult.Success)
            {
                OperationResultListVo<LocalizationViewModel> castResult = serviceResult as OperationResultListVo<LocalizationViewModel>;

                model = castResult.Value;
            }
            else
            {
                model = new List<LocalizationViewModel>();
            }

            foreach (LocalizationViewModel item in model)
            {
                SetLocalization(item);
            }

            ViewData["ListDescription"] = SharedLocalizer["These are translation projects available to help."].ToString();

            return PartialView("_ListByUser", model);
        }

        [Route("tools/localization/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            OperationResultVo result = await translationAppService.GetStatsById(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<TranslationStatsViewModel> castRestult = result as OperationResultVo<TranslationStatsViewModel>;

                TranslationStatsViewModel viewModel = castRestult.Value;

                viewModel.Url = Url.Action("details", "localization", new { area = "tools", id = viewModel.Id }, (string)ViewData["protocol"], (string)ViewData["host"]);

                SetLocalization(viewModel);

                return View("Details", viewModel);
            }
            else
            {
                return null;
            }
        }

        [Authorize]
        [Route("tools/localization/translate/{id:guid}/{language?}")]
        public async Task<IActionResult> Translate(Guid id, SupportedLanguage language)
        {
            OperationResultVo result = await translationAppService.GetById(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<LocalizationViewModel> castRestult = result as OperationResultVo<LocalizationViewModel>;

                LocalizationViewModel model = castRestult.Value;

                SetLocalization(model);

                ViewData["language"] = language.ToString();

                return View("TranslateWrapper", model);
            }
            else
            {
                return null;
            }
        }

        [Route("tools/localization/review/{id:guid}/{language?}")]
        public async Task<IActionResult> Review(Guid id, SupportedLanguage language)
        {
            OperationResultVo result = await translationAppService.GetById(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<LocalizationViewModel> castRestult = result as OperationResultVo<LocalizationViewModel>;

                LocalizationViewModel model = castRestult.Value;

                SetLocalization(model);

                ViewData["language"] = language.ToString();

                return View("ReviewWrapper", model);
            }
            else
            {
                return null;
            }
        }

        [Route("tools/localization/export/{id:guid}")]
        public async Task<IActionResult> Export(Guid id)
        {
            OperationResultVo result = await translationAppService.GetStatsById(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<TranslationStatsViewModel> castRestult = result as OperationResultVo<TranslationStatsViewModel>;

                TranslationStatsViewModel model = castRestult.Value;

                SetLocalization(model);

                return View("Export", model);
            }
            else
            {
                return null;
            }
        }

        [Route("tools/localization/exportxml/{projectId:guid}")]
        public IActionResult ExportXml(Guid projectId, LocalizationLanguage? language, bool fillGaps)
        {
            OperationResultVo result = translationAppService.GetXml(CurrentUserId, projectId, language, fillGaps);

            if (result.Success)
            {
                if (language.HasValue)
                {
                    OperationResultVo<InMemoryFileVo> castRestult = result as OperationResultVo<InMemoryFileVo>;

                    InMemoryFileVo model = castRestult.Value;

                    return File(model.Contents, "application/xml", model.FileName);
                }
                else
                {
                    OperationResultVo<List<InMemoryFileVo>> castRestult = result as OperationResultVo<List<InMemoryFileVo>>;

                    List<InMemoryFileVo> list = castRestult.Value;

                    byte[] zip = GetZipArchive(list);

                    return File(zip, "application/xml", string.Format("{0}.zip", projectId));
                }
            }
            else
            {
                return null;
            }
        }

        [Route("tools/localization/exportcontributors/{projectId:guid}")]
        public async Task<IActionResult> ExportContributors(Guid projectId, ExportContributorsType type)
        {
            OperationResultVo result = await translationAppService.GetContributorsFile(CurrentUserId, projectId, type);

            if (result.Success)
            {
                OperationResultVo<List<KeyValuePair<Guid, string>>> castRestult = result as OperationResultVo<List<KeyValuePair<Guid, string>>>;

                List<KeyValuePair<Guid, string>> model = castRestult.Value;

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("UserId;User Name;Profile URL");

                foreach (KeyValuePair<Guid, string> item in model)
                {
                    string url = Url.Action("details", "profile", new { area = string.Empty, id = item.Key }, "https", Request.Host.Value);
                    string newLine = string.Format("{0};{1};{2}", item.Key, item.Value, url);
                    sb.AppendLine(newLine);
                }

                InMemoryFileVo file = new InMemoryFileVo
                {
                    FileName = string.Format("contributors_{0}.csv", type.ToString().ToLower()),
                    Contents = Encoding.UTF8.GetBytes(sb.ToString())
                };

                return File(file.Contents, "text/csv", file.FileName);
            }
            else
            {
                return null;
            }
        }

        [Authorize]
        [Route("tools/localization/new/")]
        public async Task<IActionResult> New()
        {
            OperationResultVo serviceResult = translationAppService.GenerateNew(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultVo<LocalizationViewModel> castResult = serviceResult as OperationResultVo<LocalizationViewModel>;

                LocalizationViewModel model = castResult.Value;

                SetLocalization(model, true);

                OperationResultVo gamesResult = await translationAppService.GetMyUntranslatedGames(CurrentUserId);
                if (gamesResult.Success)
                {
                    OperationResultListVo<SelectListItemVo> castResultGames = gamesResult as OperationResultListVo<SelectListItemVo>;

                    IEnumerable<SelectListItemVo> games = castResultGames.Value;

                    List<SelectListItem> gamesDropDown = games.ToSelectList();
                    ViewData["UserGames"] = gamesDropDown;
                }
                else
                {
                    ViewData["UserGames"] = new List<SelectListItem>();
                }

                return View("CreateEditWrapper", model);
            }
            else
            {
                return View("CreateEditWrapper", new LocalizationViewModel());
            }
        }

        [Authorize]
        [Route("tools/localization/edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            OperationResultVo serviceResult = await translationAppService.GetById(CurrentUserId, id);

            if (serviceResult.Success)
            {
                OperationResultVo<LocalizationViewModel> castResult = serviceResult as OperationResultVo<LocalizationViewModel>;

                LocalizationViewModel model = castResult.Value;

                SetLocalization(model, true);

                return View("CreateEditWrapper", model);
            }
            else
            {
                return null;
            }
        }

        [Authorize]
        [HttpPost("tools/localization/save")]
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        public async Task<IActionResult> Save(LocalizationViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await translationAppService.Save(CurrentUserId, vm);

                if (!saveResult.Success)
                {
                    return Json(new OperationResultVo(saveResult.Message));
                }
                else
                {
                    string url = Url.Action("details", "localization", new { area = "tools", id = saveResult.Value });

                    if (isNew)
                    {
                        url = Url.Action("edit", "localization", new { area = "tools", id = saveResult.Value, pointsEarned = saveResult.PointsEarned });

                        if (EnvName.Equals(Constants.ProductionEnvironmentName))
                        {
                            await NotificationSender.SendTeamNotificationAsync("New Localization Project created!");
                        }
                    }

                    return Json(new OperationResultRedirectVo<Guid>(saveResult, url));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpDelete("tools/localization/delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                OperationResultVo saveResult = await translationAppService.Remove(CurrentUserId, id);

                if (saveResult.Success)
                {
                    string url = Url.Action("index", "localization", new { area = "tools" });

                    return Json(new OperationResultRedirectVo(saveResult, url));
                }
                else
                {
                    return Json(new OperationResultVo(false));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("tools/localization/gettranslation/{projectId:guid}")]
        public async Task<IActionResult> GetTranslation(Guid projectId, LocalizationLanguage language)
        {
            try
            {
                OperationResultVo result = await translationAppService.GetTranslations(CurrentUserId, projectId, language);

                if (result.Success)
                {
                    OperationResultListVo<LocalizationEntryViewModel> castResult = result as OperationResultListVo<LocalizationEntryViewModel>;

                    return Json(castResult);
                }
                else
                {
                    return Json(new OperationResultVo(false));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("tools/localization/savetranslation/{projectId:guid}")]
        public async Task<IActionResult> SaveTranslation(Guid projectId, bool currentUserIsOwner, bool currentUserHelped, LocalizationEntryViewModel vm)
        {
            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo result = await translationAppService.SaveEntry(CurrentUserId, projectId, currentUserIsOwner, currentUserHelped, vm);

                if (result.Success)
                {
                    OperationResultVo<LocalizationEntryViewModel> castResult = result as OperationResultVo<LocalizationEntryViewModel>;

                    return Json(castResult);
                }
                else
                {
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("tools/localization/saveentries/{projectId:guid}")]
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        public IActionResult SaveEntries(Guid projectId, LocalizationLanguage language, IEnumerable<LocalizationEntryViewModel> entries)
        {
            try
            {
                OperationResultVo result = translationAppService.SaveEntries(CurrentUserId, projectId, language, entries);

                if (result.Success)
                {
                    return Json(result);
                }
                else
                {
                    return Json(new OperationResultVo(false));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpGet("tools/localization/getterms/{projectId:guid}")]
        public async Task<IActionResult> GetTerms(Guid projectId)
        {
            try
            {
                OperationResultVo result = await translationAppService.GetTerms(CurrentUserId, projectId);

                if (result.Success)
                {
                    OperationResultVo<LocalizationViewModel> castResult = result as OperationResultVo<LocalizationViewModel>;

                    return PartialView("_Terms", castResult.Value);
                }
                else
                {
                    return Json(new OperationResultVo(false));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("tools/localization/saveterms/{projectId:guid}")]
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        [RequestSizeLimit(int.MaxValue)]
        public IActionResult SaveTerms(Guid projectId, IEnumerable<LocalizationTermViewModel> terms)
        {
            try
            {
                OperationResultVo result = translationAppService.SetTerms(CurrentUserId, projectId, terms);

                if (result.Success)
                {
                    return Json(result);
                }
                else
                {
                    return Json(new OperationResultVo(false));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("tools/localization/uploadterms/{projectId:guid}")]
        public async Task<IActionResult> UploadTerms(Guid projectId, IEnumerable<ExcelColumnViewModel> columns, IFormFile termsFile)
        {
            try
            {
                IEnumerable<KeyValuePair<int, LocalizationLanguage>> kvList = columns.ToKeyValuePairs();

                OperationResultVo result = await translationAppService.ReadTermsSheet(CurrentUserId, projectId, kvList, termsFile);

                if (result.Success)
                {
                    string url = Url.Action("edit", "localization", new { area = "tools", id = projectId });

                    return Json(new OperationResultRedirectVo<Guid>(projectId, url));
                }

                return Json(new OperationResultVo(false));
            }
            catch (Exception ex)
            {
                var json = new
                {
                    error = ex.Message
                };

                return Json(json);
            }
        }

        [Authorize]
        [HttpPost("tools/localization/entryreview/{projectId:guid}")]
        public IActionResult EntryReview(Guid projectId, Guid entryId, bool accept)
        {
            try
            {
                OperationResultVo result = translationAppService.EntryReview(CurrentUserId, projectId, entryId, accept);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        private void SetLocalization(LocalizationViewModel model)
        {
            SetLocalization(model, false);

            model.SetShareText(SharedLocalizer["Help translate {0}", model.Game.Title]);

            model.SetShareUrl(Url.Action("details", "localization", new { area = "tools", id = model.Id }));
        }

        private void SetLocalization(LocalizationViewModel model, bool editing)
        {
            if (model != null && !editing && string.IsNullOrWhiteSpace(model.Introduction))
            {
                model.Introduction = SharedLocalizer["No extra information."];
            }
        }

        public static byte[] GetZipArchive(List<InMemoryFileVo> files)
        {
            byte[] archiveFile;
            using (MemoryStream archiveStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (InMemoryFileVo file in files)
                    {
                        ZipArchiveEntry zipArchiveEntry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                        using (Stream zipStream = zipArchiveEntry.Open())
                        {
                            zipStream.Write(file.Contents, 0, file.Contents.Length);
                        }
                    }
                }

                archiveFile = archiveStream.ToArray();
            }

            return archiveFile;
        }
    }
}