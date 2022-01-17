using LuduStack.Application;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.Requests.Notification;
using LuduStack.Application.ViewModels.UserPreferences;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Helper;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Abstractions;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Enums;
using LuduStack.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers.Base
{
    public class SecureBaseController : BaseController
    {
        private IServiceProvider services;
        public IServiceProvider Services => services ??= HttpContext?.RequestServices.GetService<IServiceProvider>();

        private IWebHostEnvironment hostEnvironment;
        public IWebHostEnvironment HostEnvironment => hostEnvironment ??= Services.GetService<IWebHostEnvironment>();

        private INotificationSender notificationSender;
        public INotificationSender NotificationSender => notificationSender ??= Services.GetService<INotificationSender>();

        private UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager => _userManager ??= Services.GetService<UserManager<ApplicationUser>>();

        private IImageStorageService _imageStorageService;
        public IImageStorageService ImageStorageService => _imageStorageService ??= Services.GetService<IImageStorageService>();

        private ICookieMgrService _cookieMgrService;
        public ICookieMgrService CookieMgrService => _cookieMgrService ??= Services.GetService<ICookieMgrService>();

        private IProfileAppService profileAppService;
        public IProfileAppService ProfileAppService => profileAppService ??= Services.GetService<IProfileAppService>();

        private IUserPreferencesAppService userPreferencesAppService;

        public IUserPreferencesAppService UserPreferencesAppService => userPreferencesAppService ??= Services.GetService<IUserPreferencesAppService>();

        private IPlatformSettingAppService platformSettingAppService;

        public IPlatformSettingAppService PlatformSettingAppService => platformSettingAppService ??= Services.GetService<IPlatformSettingAppService>();

        public Guid CurrentUserId { get; set; }

        public bool CurrentUserIsAdmin { get; set; }

        public string CurrentLocale { get; set; }

        public string EnvName { get; private set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string notificationClicked = context.HttpContext.Request.Query["notificationclicked"].FirstOrDefault();
            if (notificationClicked != null)
            {
                Guid notificationId = Guid.Parse(notificationClicked);

                IMediator mediator = Services.GetRequiredService<IMediator>();

                await mediator.Send(new SendNotificationRequest(notificationId));
            }

            if (User != null && User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                CurrentUserId = new Guid(userId);

                ViewData["CurrentUserId"] = CurrentUserId;
                ViewData["ProfileImage"] = UrlFormatter.ProfileImage(CurrentUserId, 38);

                Claim userIsAdmin = User.FindFirst(x => x.Type == ClaimTypes.Role && x.Value.Equals(Roles.Administrator.ToString()));

                if (userIsAdmin != null)
                {
                    ViewData["user_is_admin"] = "true";
                    CurrentUserIsAdmin = true;
                }

                if (ViewData["Username"] == null)
                {
                    string username = User.FindFirstValue(ClaimTypes.Name);

                    await SetProfileOnSession(CurrentUserId, username);
                    ViewData["Username"] = username ?? Constants.DefaultUsername;
                }

                string msg = context.HttpContext.Request.Query["msg"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    TempData["Message"] = SharedLocalizer[msg];
                }

                string msgModal = context.HttpContext.Request.Query["msgModal"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(msgModal))
                {
                    TempData["MessageModal"] = msgModal;
                }

                string pointsEarned = context.HttpContext.Request.Query["pointsEarned"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(pointsEarned) && !pointsEarned.Equals("0"))
                {
                    TempData["MessagePoints"] = SharedLocalizer["You earned {0} points. Awesome!", pointsEarned].Value;
                }
            }

            CurrentLocale = await GetAspNetCultureCookie();
            ViewData["Locale"] = CurrentLocale;

            EnvName = string.Format("env-{0}", HostEnvironment.EnvironmentName);

            await SetAds();

            await base.OnActionExecutionAsync(context, next);
        }

        private async Task SetAds()
        {
            OperationResultVo<Application.ViewModels.PlatformSetting.PlatformSettingViewModel> showAdsResult = await PlatformSettingAppService.GetByElement(CurrentUserId, PlatformSettingElement.ShowAds);
            if (showAdsResult.Success)
            {
                ViewData["ShowAds"] = showAdsResult.Value.Value.Equals("1");
            }
        }

        protected async Task SetProfileOnSession(Guid userId, string userName)
        {
            string sessionUserName = GetSessionValue(SessionValues.Username);

            if (sessionUserName != null && !sessionUserName.Equals(userName))
            {
                SetSessionValue(SessionValues.Username, userName);
            }

            string sessionFullName = GetSessionValue(SessionValues.FullName);

            if (sessionFullName == null)
            {
                UserProfileEssentialVo profile = await ProfileAppService.GetEssentialUserProfileWithCache(userId);
                if (profile != null)
                {
                    SetSessionValue(SessionValues.FullName, profile.Name.Trim());
                    SetCookieValue(SessionValues.FullName.ToString(), profile.Name.Trim(), 365);
                }
            }
        }

        protected void SetEmailConfirmed(ApplicationUser user)
        {
            string sessionEmailConfirmed = user.EmailConfirmed.ToString();

            SetEmailConfirmed(sessionEmailConfirmed);
        }

        protected void SetEmailConfirmed()
        {
            string sessionEmailConfirmed = GetSessionValue(SessionValues.EmailConfirmed);
            if (string.IsNullOrWhiteSpace(sessionEmailConfirmed))
            {
                Task<ApplicationUser> task = UserManager.GetUserAsync(User);
                task.Wait();

                ApplicationUser user = task.Result;

                sessionEmailConfirmed = user?.EmailConfirmed.ToString() ?? "False";
            }

            SetEmailConfirmed(sessionEmailConfirmed);
        }

        private void SetEmailConfirmed(string sessionEmailConfirmed)
        {
            SetSessionValue(SessionValues.EmailConfirmed, sessionEmailConfirmed);

            ViewData["emailConfirmed"] = sessionEmailConfirmed;
        }

        protected string GetAvatar()
        {
            return GetCookieValue(SessionValues.UserProfileImageUrl);
        }

        protected void SetAvatar(string profileImageUrl)
        {
            SetCookieValue(SessionValues.UserProfileImageUrl, profileImageUrl, 7, true);
        }

        protected SupportedLanguage SetLanguageFromCulture(string languageCode)
        {
            switch (languageCode)
            {
                case "pt-BR":
                case "pt":
                    return SupportedLanguage.Portuguese;

                case "ru":
                case "ru-RU":
                    return SupportedLanguage.Russian;

                case "es":
                    return SupportedLanguage.Spanish;

                default:
                    return SupportedLanguage.English;
            }
        }

        protected void SetAspNetCultureCookie(SupportedLanguage language)
        {
            string locale = language.GetAttributeOfType<UiInfoAttribute>()?.Locale;

            locale = string.IsNullOrWhiteSpace(locale) ? "en-US" : locale;

            SetAspNetCultureCookie(new RequestCulture(locale));
        }

        private void SetAspNetCultureCookie(RequestCulture culture)
        {
            ViewData["Locale"] = culture;

            SetCookieValue(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(culture), 365, SameSiteMode.Strict);
        }

        protected async Task<string> GetAspNetCultureCookie()
        {
            RequestCulture requestLanguage = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture;

            string cookieValue = GetCookieValue(CookieRequestCultureProvider.DefaultCookieName);

            ProviderCultureResult cookie = CookieRequestCultureProvider.ParseCookieValue(cookieValue);

            if (!User.Identity.IsAuthenticated)
            {
                SetAspNetCultureCookie(requestLanguage);

                return requestLanguage.UICulture.Name;
            }
            else
            {
                if (cookie == null || !cookie.Cultures.Any())
                {
                    UserPreferencesViewModel userPrefs = await UserPreferencesAppService.GetByUserId(CurrentUserId);

                    if (userPrefs != null && userPrefs.Id != Guid.Empty)
                    {
                        SetAspNetCultureCookie(userPrefs.UiLanguage);
                    }
                    else
                    {
                        SetAspNetCultureCookie(requestLanguage);
                    }

                    return requestLanguage != null ? requestLanguage.UICulture.Name : "en-US";
                }
                else
                {
                    return cookie.Cultures.First().Value;
                }
            }
        }

        protected async Task<UserPreferencesViewModel> GetUserPreferences(Guid userId)
        {
            UserPreferencesViewModel preferences = await UserPreferencesAppService.GetByUserId(userId);
            if (preferences != null)
            {
                SetUserPreferences(preferences);
            }

            return preferences;
        }

        protected void SetUserPreferences(UserPreferencesViewModel preferences)
        {
            List<SupportedLanguage> userLanguages = preferences.Languages;
            if (userLanguages == null || !userLanguages.Any())
            {
                userLanguages = LanguageDomainHelper.FormatList(preferences.ContentLanguages).ToList();
            }

            if (preferences == null || preferences.Id == Guid.Empty)
            {
                RequestCulture requestLanguage = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture;
                SupportedLanguage lang = SetLanguageFromCulture(requestLanguage.UICulture.Name);

                SetCookieValue(SessionValues.PostLanguage, lang.ToString(), 7);
            }
            else
            {
                SetCookieValue(SessionValues.PostLanguage, preferences.UiLanguage.ToString(), 7);
                SetCookieValue(SessionValues.ContentLanguages, JsonSerializer.Serialize(userLanguages), 7);
                SetSessionValue(SessionValues.JobProfile, preferences.JobProfile.ToString());
            }
        }

        protected async Task<List<SupportedLanguage>> GetCurrentUserContentLanguage()
        {
            string languagesFromCookie = CookieMgrService.Get(SessionValues.ContentLanguages.ToString());
            if (string.IsNullOrWhiteSpace(languagesFromCookie))
            {
                return await UserPreferencesAppService.GetLanguagesByUserId(CurrentUserId);
            }
            else
            {
                return JsonSerializer.Deserialize<List<SupportedLanguage>>(languagesFromCookie);
            }
        }

        #region Upload Management

        #region Main Methods

        private async Task<UploadResultVo> UploadImage(Guid userId, string imageType, string filename, string extension, byte[] fileBytes, params string[] tags)
        {
            tags = tags.Where(x => x != null).ToArray();

            string newFilename = imageType.ToLower() + "_" + filename;

            UploadResultVo op = await ImageStorageService.StoreMediaAsync(userId.ToString(), newFilename, extension, fileBytes, tags);

            return op;
        }

        private string DeleteImage(Guid userId, string filename)
        {
            Task<string> op = ImageStorageService.DeleteImageAsync(userId.ToString(), filename);
            op.Wait();

            if (!op.IsCompletedSuccessfully)
            {
                throw op.Exception;
            }

            string url = op.Result;

            return url;
        }

        #endregion Main Methods

        protected async Task<UploadResultVo> UploadImage(Guid userId, ImageType imageType, string filename, string extension, byte[] fileBytes, params string[] tags)
        {
            string type = imageType.ToString();

            return await UploadImage(userId, type, filename, extension, fileBytes, tags);
        }

        protected async Task<UploadResultVo> UploadContentMedia(Guid userId, string filename, string extension, byte[] fileBytes, params string[] tags)
        {
            string type = ImageType.ContentImage.ToString().ToLower();

            return await UploadImage(userId, type, filename, extension, fileBytes, tags);
        }

        protected async Task<UploadResultVo> UploadFeaturedImage(Guid userId, string filename, string extension, byte[] fileBytes, params string[] tags)
        {
            string type = ImageType.FeaturedImage.ToString().ToLower();

            return await UploadImage(userId, type, filename, extension, fileBytes, tags);
        }

        protected string DeleteGameImage(Guid userId, ImageType type, string filename)
        {
            string result = DeleteImage(userId, filename);

            return result;
        }

        protected string DeleteFeaturedImage(Guid userId, string filename)
        {
            string result = DeleteImage(userId, filename);

            return result;
        }

        #endregion Upload Management

        #region Cookie Management

        protected string GetCookieValue(SessionValues key)
        {
            string value = CookieMgrService.Get(key.ToString());

            return value;
        }

        private string GetCookieValue(string key)
        {
            string value = CookieMgrService.Get(key);

            return value;
        }

        protected void SetCookieValue(SessionValues key, string value, int? expireTime)
        {
            SetCookieValue(key, value, expireTime, false);
        }

        protected void SetCookieValue(SessionValues key, string value, int? expireTime, bool isEssential)
        {
            CookieMgrService.Set(key.ToString(), value, expireTime, isEssential, null);
        }

        protected void SetCookieValue(string key, string value, int? expireTime)
        {
            SetCookieValue(key, value, expireTime, false);
        }

        protected void SetCookieValue(string key, string value, int? expireTime, bool isEssential)
        {
            CookieMgrService.Set(key, value, expireTime, isEssential, null);
        }

        protected void SetCookieValue(string key, string value, int? expireTime, bool isEssential, SameSiteMode sameSite)
        {
            CookieMgrService.Set(key, value, expireTime, isEssential, sameSite);
        }

        protected void SetCookieValue(string key, string value, int? expireTime, SameSiteMode sameSite)
        {
            CookieMgrService.Set(key, value, expireTime, false, sameSite);
        }

        #endregion Cookie Management
    }
}