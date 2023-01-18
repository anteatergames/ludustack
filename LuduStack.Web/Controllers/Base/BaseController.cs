using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.PlatformSetting;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuduStack.Web.Controllers.Base
{
    public class BaseController : Controller
    {
        private IServiceProvider services;
        public IServiceProvider Services => services ??= HttpContext?.RequestServices.GetService<IServiceProvider>();

        private IPlatformSettingAppService platformSettingAppService;
        public IPlatformSettingAppService PlatformSettingAppService => platformSettingAppService ??= Services.GetService<IPlatformSettingAppService>();

        private IStringLocalizer<SharedResources> _sharedLocalizer;
        public IStringLocalizer<SharedResources> SharedLocalizer => _sharedLocalizer ?? (_sharedLocalizer = (IStringLocalizer<SharedResources>)HttpContext?.RequestServices.GetService(typeof(IStringLocalizer<SharedResources>)));

        protected JsonSerializerOptions DefaultJsonSerializeOptions { get; }

        public BaseController()
        {
            DefaultJsonSerializeOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            ViewData["BaseUrl"] = GetBaseUrl();

            SetPlatformSettings();
        }
        private void SetPlatformSettings()
        {
            PlatformSettingViewModel showDonateButton = PlatformSettingAppService.GetByElement(Guid.Empty, PlatformSettingElement.ShowDonateButton).Result.Value;
            ViewData["ShowDonateButton"] = showDonateButton.Value.Equals("1") ? true : false;

            PlatformSettingViewModel showIdeaGenerator = PlatformSettingAppService.GetByElement(Guid.Empty, PlatformSettingElement.ShowHomePageIdeaGenerator).Result.Value;
            ViewData["ShowIdeaGenerator"] = showIdeaGenerator.Value.Equals("1") ? true : false;

            PlatformSettingViewModel showStore = PlatformSettingAppService.GetByElement(Guid.Empty, PlatformSettingElement.ShowStore).Result.Value;
            ViewData["ShowStore"] = showStore.Value.Equals("1") ? true : false;

            PlatformSettingViewModel showGames = PlatformSettingAppService.GetByElement(Guid.Empty, PlatformSettingElement.ShowGames).Result.Value;
            ViewData["ShowGames"] = showGames.Value.Equals("1") ? true : false;

            ViewData["FacebookUrl"] = PlatformSettingAppService.GetByElement(Guid.Empty, PlatformSettingElement.FacebookUrl).Result.Value.Value;

            ViewData["DiscordUrl"] = PlatformSettingAppService.GetByElement(Guid.Empty, PlatformSettingElement.DiscordUrl).Result.Value.Value;
        }

        protected string GetBaseUrl()
        {
            string hostUrl = WebUtility.UrlDecode($"{Request.Scheme}://{Request.Host}{Request.PathBase}");

            ViewData["protocol"] = Request.IsHttps ? "https" : "http";
            ViewData["host"] = Request.Host.ToString();

            return hostUrl;
        }

        protected string GetSessionValue(SessionValues key)
        {
            string value = HttpContext.Session.GetString(key.ToString());

            return value;
        }

        protected void SetSessionValue(SessionValues key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                HttpContext.Session.SetString(key.ToString(), value);
            }
        }

        protected static string GetFileExtension(IFormFile uploadedFile)
        {
            string extension = GetFileExtension(uploadedFile.FileName);

            return extension;
        }

        protected static string GetFileExtension(string fileUrl)
        {
            string[] split = fileUrl.Split('.');
            string extension = split.Length > 1 ? split[1] : "jpg";
            return extension;
        }

        protected JsonResult Json(OperationResultVo operationVo)
        {
            if (!string.IsNullOrWhiteSpace(operationVo.Message))
            {
                operationVo.Message = SharedLocalizer[operationVo.Message];
            }

            return Json((object)operationVo);
        }

        protected IActionResult RedirectToWithMessage(string action, string controller, string area, string message)
        {
            return RedirectToAction(action, controller, new { area, msg = SharedLocalizer[message] });
        }
    }
}